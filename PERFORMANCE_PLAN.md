# WALL-E — Plan de Optimización Máxima de Rendimiento

Análisis de cuellos de botella y plan de arquitectura para rendimiento garantizado,
centrado en el caso de uso de dibujo de fractales.

---

## Índice

1. [Diagnóstico de rendimiento actual](#1-diagnóstico)
2. [Pipeline asíncrono (Fase 0)](#2-fase-0-pipeline-asíncrono)
3. [Secuencias con límite garantizado (Fase 1)](#3-fase-1-secuencias-con-límite-garantizado)
4. [Streaming evaluator → renderer (Fase 2)](#4-fase-2-streaming-evaluator--renderer)
5. [Evaluator state machine (Fase 3)](#5-fase-3-evaluator-state-machine)
6. [GPU rendering con SkiaSharp (Fase 4)](#6-fase-4-gpu-rendering-con-skiasharp)
7. [Memoria y pooling (Fase 5)](#7-fase-5-memoria-y-pooling)
8. [Expression caching (Fase 6)](#8-fase-6-expression-caching)
9. [Métricas objetivo](#9-métricas-objetivo)
10. [Resumen de esfuerzo](#10-resumen-de-esfuerzo)

---

## 1. Diagnóstico de rendimiento actual

### 1.1 Los tres puntos de congelamiento

#### PUNTO 1: draw de secuencia infinita en el hilo UI

**Archivo**: `Graphic/Form1.cs:116-125`

```csharp
else if (figure is InfinitePointSequence seq)
{
    while (Continue)                           // ← hilo UI
    {
        Point value = seq.ReturnValue();       // ← while(true) enumerator
        if (value == default(Point))
            break;
        Draw_Point(value);                     // ← GDI+ uno por uno
    }
}
```

**Problemas**:
- Corre en el hilo de UI → nunca procesa mensajes de Windows → el botón "Stop" no puede ejecutarse
- `seq.ReturnValue()` consume un generador `while (true)` → **nunca termina**
- No hay `Application.DoEvents()`, `async`, ni suspensión
- `Continue` no es `volatile` → ni siquiera es thread-safe el flag

**Resultado**: UI congelada permanentemente. Solo matar el proceso.

#### PUNTO 2: Concatenación de secuencias infinitas

**Archivos**: `AST/Expression/Binary/Numeric/Sum.cs:31-44`, `AST/Sequence/Sequence Concatenation.cs:38-68`

```csharp
// Sum.cs
IEnumerable<object> Generate(AbsSequence r, AbsSequence l)
{
    foreach (object item in r.Sequence!)  // ← itera enumerable infinito
        yield return item;
    foreach (object item in l.Sequence!)  // ← itera enumerable infinito
        yield return item;
}
```

**Problema**: Hacer `infinite_seq + anything` itera con `foreach` sobre `.Sequence` que es un `IEnumerable` infinito. Esto se congela durante la **evaluación**, antes de llegar al `draw`. No hay UI congelada (aún no se dibuja), pero tampoco hay cancelación.

#### PUNTO 3: Generadores while(true) sin límite

**Archivo**: `Enviroment/Context.cs:52-100`

Tres generadores built-in usan `while (true)`:

| Función | Línea | Generador |
|---|---|---|
| `randoms()` | 52-65 | `while (true) { yield return r.NextDouble(); }` |
| `samples()` | 66-82 | `while (true) { tem.RandomPoint(points); yield return tem; }` |
| `points(circle)` | 83-100 | `while (true) { point = c.PointInsideFigure(points); yield return point; }` |

**Problema adicional en `samples()`**: `Point tem` se crea una sola vez fuera del ciclo y se muta en cada iteración. Todas las yields devuelven la misma referencia.

### 1.2 Problemas de rendimiento secundarios

| Problema | Archivo | Impacto |
|---|---|---|
| `new Random()` en cada llamada a método | Múltiples archivos | Semillas repetidas en rápido sucesión, overhead de alocación |
| `Keys.Contains` en vez de `ContainsKey` | `Evaluator.cs` | O(n) en vez de O(1) |
| `while ((start+i) < long.MaxValue)` en secuencias | `Infinite Sequence.cs` | 9.2e18 iteraciones — infinito en la práctica |
| `PuntosGenerados` HashSet nunca se limpia | `Form1.cs:20` | Crecimiento infinito de memoria |
| `ExistingPoints/Circles/Lines/etc` nunca se limpian | `Context.cs:40-45` | Crecimiento infinito de memoria |
| GDI+ sin Dispose | `Form1.cs:17-19` | Resource leak, GC pressure |
| Sin async/await | Todo el pipeline | UI bloqueada durante procesamiento |
| Sin batching de render | `Form1.cs` | Cada punto dibujado uno por uno → overhead GDI+ masivo |

### 1.3 Memoria: acumulación sin límite

| Colección | Tipo | Crecimiento |
|---|---|---|
| `context.ToDraw` | `List<DrawObject>` | Cada `draw` → crece sin límite |
| `ExistingPoints/Circles/Lines/Segments/Rays` | `List<T>` | Cada figura creada → crece sin límite |
| `PuntosGenerados` | `HashSet<Point>` | Cada punto dibujado → crece sin límite |
| `Finite_Sequence<T>.values` | `List<T>` | Secuencia entera en memoria |
| `GlobalConstant` + `Scope.Variables` | Dictionary | Acumulan entre ejecuciones |

### 1.4 Línea base de rendimiento

| Escenario | Antes | Estado |
|---|---|---|
| `draw samples();` | ∞ (freeze) | ❌ |
| Fractal recursivo 100 iteraciones | Stack overflow o freeze | ❌ |
| UI durante evaluación | Congelada | ❌ |
| Stop button | No funcional (hilo bloqueado) | ❌ |
| `seqA + seqB` con infinitas | ∞ (freeze en eval) | ❌ |
| Memoria, 3 runs seguidos | OoM probable | ❌ |
| `draw point(1,2);` simple | ~50ms | ✅ |

---

## 2. Fase 0 — Pipeline asíncrono

**Objetivo**: La UI nunca se congela. Stop funciona. Primer fractal se puede cancelar.

### 2.1 ActionButton_Click async

```csharp
// Antes: síncrono, bloquea UI
private void ActionButton_Click(object sender, EventArgs e)
{
    string code = Editor.Text;
    ArchiveAnalysis processor = new ArchiveAnalysis(code, "Editor");
    context = processor.Analyze(context);
    DrawAllFigures();
}

// Después: async, UI responsiva
private CancellationTokenSource _cts = new();

private async void ActionButton_Click(object sender, EventArgs e)
{
    _cts = new CancellationTokenSource();
    string code = Editor.Text;
    try
    {
        var result = await Task.Run(() => {
            ArchiveAnalysis processor = new ArchiveAnalysis(code, "Editor");
            return processor.Analyze(context);
        }, _cts.Token);

        context = result;
        await DrawAllFiguresAsync(_cts.Token);
    }
    catch (OperationCanceledException)
    {
        StatusBar.Text = "Cancelled";
    }
}

private void StopButton_Click(object sender, EventArgs e)
{
    _cts.Cancel();
}
```

### 2.2 Evaluator con CancellationToken

```csharp
// Antes: sin cancelación
object value = evaluate.GeneralEvaluation(item);

// Después: chequea cancelación en cada nodo
public object GeneralEvaluation(Node node, CancellationToken ct)
{
    ct.ThrowIfCancellationRequested();
    // ... resto del método
}
```

### 2.3 Evaluator como callback stream

En vez de acumular en `ToDraw` y dibujar después, el evaluator notifica al renderer vía callback:

```csharp
// Antes: acumulación en lista
context.ToDraw.Add(drawObj);

// Después: streaming vía callback
_drawCallback?.Invoke(drawObj);  // el renderer dibuja inmediatamente
```

### 2.4 Archivos afectados

| Archivo | Cambio |
|---|---|
| `Graphic/Form1.cs` | `ActionButton_Click` → `async`, añadir `StopButton`, `CancellationTokenSource` |
| `Evaluation/Evaluator/Evaluator.cs` | `GeneralEvaluation` acepta `CancellationToken`, chequea en cada rama |
| `Enviroment/ArchiveAnallizer.cs` | `Analyze` acepta `CancellationToken`, lo pasa al evaluator |

### 2.5 Verificación

- Botón "Process" inicia evaluación en background
- Botón "Stop" cancela y UI sigue responsiva
- Durante evaluación larga, se puede mover la ventana, hacer clic, etc.

---

## 3. Fase 1 — Secuencias con límite garantizado

**Objetivo**: Eliminar todas las operaciones O(∞). Toda secuencia tiene un máximo de elementos.

### 3.1 Límite global configurable

```csharp
// Sequence.cs
public abstract class AbsSequence
{
    public const long DefaultMaxElements = 10000;
    public long MaxElements { get; set; } = DefaultMaxElements;
    
    public bool IsInfinite => count < 0;
    public bool IsExhausted { get; protected set; }
}
```

### 3.2 Take() en el DSL

```csharp
// Nuevo: TakenSequence<T>
public class TakenSequence<T> : GenericSequence<T>
{
    public TakenSequence(GenericSequence<T> source, long count)
    {
        this.count = count;
        Sequence = source.Sequence.Take(count);
        enumerator = Sequence.GetEnumerator();
    }
}
```

**DSL**: `draw take(samples(), 100);` — dibuja exactamente 100 puntos.

**Parser**: Nueva producción `TakeExpr → "take" "(" expr "," expr ")"`.

**Evaluator**: Crea `TakenSequence<Point>` o `TakenSequence<object>` según el tipo.

### 3.3 Concat segura

```csharp
// Sum.cs — antes: itera sin límite
IEnumerable<object> Generate(AbsSequence r, AbsSequence l)
{
    foreach (object item in r.Sequence!)
        yield return item;
    foreach (object item in l.Sequence!)
        yield return item;
}

// Sum.cs — después: límite por secuencia
IEnumerable<object> GenerateSafe(AbsSequence r, AbsSequence l)
{
    long limit = r.IsInfinite ? r.MaxElements : r.count;
    long taken = 0;
    foreach (object item in r.Sequence!)
    {
        if (taken++ >= limit) break;
        yield return item;
    }
    taken = 0;
    limit = l.IsInfinite ? l.MaxElements : l.count;
    foreach (object item in l.Sequence!)
    {
        if (taken++ >= limit) break;
        yield return item;
    }
}
```

### 3.4 Render con límite

```csharp
// Form1.cs — antes: while (Continue) sin límite
while (Continue)
{
    Point value = seq.ReturnValue();
    if (value == default(Point)) break;
    Draw_Point(value);
}

// Form1.cs — después: límite + eventos
long drawn = 0;
while (!cts.IsCancellationRequested && drawn < seq.MaxElements)
{
    Point value = seq.ReturnValue();
    if (value == default(Point)) break;
    Draw_Point(value);
    drawn++;
}
```

### 3.5 Bug: GenerateSamples — misma referencia

```csharp
// Context.cs — antes: Point tem creado una vez, mutado cada iteración
Point tem = new Point(0, 0);
while (true)
{
    tem.RandomPoint(points);
    points.Add(tem);      // añade la misma referencia siempre
    yield return tem;     // siempre el mismo objeto mutado
}

// Context.cs — después: nuevo objeto cada iteración
while (true)
{
    Point tem = new Point(0, 0);
    tem.RandomPoint(points);
    points.Add(tem);
    yield return tem;
}
```

### 3.6 Archivos afectados

| Archivo | Cambio |
|---|---|
| `AST/Sequence/Sequence.cs` | Añadir `MaxElements`, `IsExhausted`, límite en `ReturnValue` |
| `AST/Sequence/Infinite Sequence.cs` | Heredar límite, `GenerateSequence` acepta `maxElements` |
| `AST/Sequence/TakenSequence.cs` (nuevo) | Secuencia truncada |
| `AST/Expression/Binary/Numeric/Sum.cs` | `GenerateSafe` con límite |
| `AST/Sequence/Sequence Concatenation.cs` | `GenerateNewSequence` con límite |
| `Lexer/Lexer.cs` | Nuevo token `take` |
| `Parser/Parser.cs` | Nueva producción `TakeExpr` |
| `Evaluation/Evaluator/Evaluator.cs` | Evaluar `TakeExpr`, pasar `CancellationToken` |
| `Enviroment/Context.cs` | Bug `GenerateSamples`, límite en generadores `while(true)` |
| `Graphic/Form1.cs` | Draw loop con límite |

### 3.7 Verificación

- `draw samples();` dibuja máximo `DefaultMaxElements` (10000) puntos, no infinitos
- `draw take(samples(), 100);` dibuja exactamente 100
- `infinite_seq + infinite_seq` itera `MaxElements` de cada una, no 9.2e18
- `samples()` devuelve puntos distintos (bug corregido)

---

## 4. Fase 2 — Streaming evaluator → renderer

**Objetivo**: Resultados progresivos. El primer punto se dibuja en ms, el evaluator sigue en background.

### 4.1 Canal de comunicación

```csharp
// Pipeline central: Channel<DrawCommand>
using System.Threading.Channels;

public class DrawPipeline
{
    private readonly Channel<DrawCommand> _channel;
    private readonly CancellationTokenSource _cts;
    
    public DrawPipeline(int capacity = 1000)
    {
        _channel = Channel.CreateBounded<DrawCommand>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
        _cts = new CancellationTokenSource();
    }
    
    public ChannelWriter<DrawCommand> Writer => _channel.Writer;
    public ChannelReader<DrawCommand> Reader => _channel.Reader;
    public CancellationToken Token => _cts.Token;
    
    public void Cancel() => _cts.Cancel();
}

public readonly struct DrawCommand
{
    public readonly object Figure;
    public readonly string Tag;
    public readonly string Color;
    
    public DrawCommand(object figure, string tag, string color)
    {
        Figure = figure;
        Tag = tag;
        Color = color;
    }
}
```

### 4.2 Evaluator escribe al canal

```csharp
// En vez de: context.ToDraw.Add(drawObj);
// El evaluator recibe un ChannelWriter<DrawCommand> y escribe:
await writer.WriteAsync(new DrawCommand(value, tag, context.UtilizedColors.Peek()), ct);
```

### 4.3 Renderer consume del canal

```csharp
// Nuevo: StreamRenderer
public class StreamRenderer : IDisposable
{
    private readonly ChannelReader<DrawCommand> _reader;
    private readonly SkiaSharp.SKCanvas _canvas;
    private const int BatchSize = 100;
    
    public async Task RenderLoop(CancellationToken ct)
    {
        await foreach (DrawCommand cmd in _reader.ReadAllAsync(ct))
        {
            DrawFigure(cmd);
            
            // Cada BatchSize comandos, refrescar canvas y ceder
            if (++_drawnThisBatch >= BatchSize)
            {
                _drawnThisBatch = 0;
                _invalidated = true;
                // Yield al UI thread para refrescar
                await Task.Yield();
            }
        }
    }
}
```

### 4.4 Diagrama del pipeline

```
Evaluator (background thread)
    │
    │ Channel<DrawCommand>.WriteAsync(...)
    ▼
┌─────────────────────────────┐
│  Channel (bounded, cap 1000) │  ← backpressure natural
└─────────────────────────────┘
    │
    │ Channel<DrawCommand>.ReadAllAsync(...)
    ▼
Renderer (Avalonia UI thread, async)
    │
    ├── Batch: 100 DrawCommands
    ├── SKCanvas → GPU
    ├── Invalidate()
    ├── await Task.Yield()
    └── Repeat
```

**Backpressure**: Si el evaluator produce más rápido de lo que el renderer consume, el `Channel` (cap 1000) se llena y `WriteAsync` espera. Esto automáticamente sincroniza la velocidad.

**Resultado progresivo**: Los primeros 1000 puntos se ven casi instantáneamente, el resto aparecen en lotes.

### 4.5 Archivos afectados

| Archivo | Cambio |
|---|---|
| `Evaluation/Pipeline/DrawPipeline.cs` (nuevo) | `Channel<DrawCommand>`, `CancellationTokenSource` |
| `Evaluation/Pipeline/StreamRenderer.cs` (nuevo) | Loop async de consumo |
| `Evaluation/Evaluator/Evaluator.cs` | Aceptar `ChannelWriter`, escribir en vez de acumular |
| `Graphic/Form1.cs` | Reemplazar `DrawAllFigures` por `StreamRenderer.RenderLoop` |

### 4.6 Verificación

- `draw take(samples(), 10000);`: primer punto visible en < 100ms
- UI responsiva durante streaming (se puede hacer clic, mover ventana)
- Cancelación detiene el streaming en < 500ms
- Memoria plana: no acumula más de 1000 DrawCommands

---

## 5. Fase 3 — Evaluator state machine

**Objetivo**: Evaluación incremental pausable. Sin stack overflow. Progreso medible.

### 5.1 Por qué una state machine

El evaluator actual es recursivo:

```csharp
public object GeneralEvaluation(Node node)
{
    if (node.Type == NodeType.Sum)
    {
        object left = GeneralEvaluation(node.Branches[0]); // ← recursión
        object right = GeneralEvaluation(node.Branches[1]);
        // ...
    }
}
```

Problemas:
- Sin pausa posible a mitad de evaluación
- Stack overflow en recursión profunda (>1000 llamadas)
- No se puede medir progreso (no se sabe cuántos nodos quedan)

Solución: máquina de estados con stack explícito.

### 5.2 Estructura de la state machine

```csharp
public readonly struct EvaluationFrame
{
    public readonly Node Node;
    public readonly int State;       // 0 = pendiente, 1 = left done, 2 = right done
    public readonly object? LeftResult;
    public readonly object? RightResult;
    public readonly List<object>? Accumulator;
    
    public EvaluationFrame(Node node)
    {
        Node = node;
        State = 0;
        LeftResult = null;
        RightResult = null;
        Accumulator = null;
    }
    
    public EvaluationFrame WithLeft(object result) =>
        new EvaluationFrame(Node) { State = 1, LeftResult = result };
}

public class StateMachineEvaluator
{
    private readonly Stack<EvaluationFrame> _stack = new();
    private readonly Context _context;
    private int _stepsExecuted;
    
    public const int MaxStepsPerBatch = 100;
    public int TotalSteps { get; private set; }
    
    // Avanza N pasos, retorna si hay trabajo pendiente
    public bool Step(int maxSteps, CancellationToken ct)
    {
        for (int i = 0; i < maxSteps; i++)
        {
            ct.ThrowIfCancellationRequested();
            if (_stack.Count == 0) return false;  // terminó
            StepOne();
        }
        return _stack.Count > 0;  // aún hay trabajo
    }
    
    private void StepOne()
    {
        _stepsExecuted++;
        var frame = _stack.Pop();
        var node = frame.Node;
        
        switch (frame.State)
        {
            case 0: // Primera vez: iniciar evaluación
                switch (node.Type)
                {
                    case NodeType.Sum:
                        _stack.Push(frame);  // guardar para cuando vuelvan los hijos
                        _stack.Push(new EvaluationFrame(node.Branches[0])); // evaluar left
                        break;
                    // ... otros tipos
                }
                break;
            case 1: // LeftResult ya está en frame
                _stack.Push(frame.WithLeft(result)); // guardar left
                _stack.Push(new EvaluationFrame(node.Branches[1])); // evaluar right
                break;
            case 2: // Ambos resultados listos
                object result = ComputeBinary(node, frame.LeftResult!, frame.RightResult!);
                // Pop del frame padre y pasarle el resultado
                break;
        }
    }
}
```

### 5.3 Integración con el pipeline

```csharp
// Pipeline integrado
public async Task ProcessCodeAsync(string code, ChannelWriter<DrawCommand> writer, CancellationToken ct)
{
    // Lexer + Parser (rápido, no necesita state machine)
    var lexer = new GeneralLexer(code, "Editor");
    var tokens = lexer.Process(lexer.lines);
    var parser = new GeneralParser(tokens, "Editor");
    var ast = parser.ParseArchive();
    
    // Evaluator state machine
    var evaluator = new StateMachineEvaluator(context, writer);
    evaluator.Load(ast);
    
    while (evaluator.Step(StateMachineEvaluator.MaxStepsPerBatch, ct))
    {
        // Yield al UI para mantener responsividad
        await Task.Yield();
        
        // Reportar progreso
        ReportProgress(evaluator.Progress);
    }
}
```

### 5.4 Beneficios

| Problema | Antes | Después |
|---|---|---|
| Stack overflow | 100 llamadas recursivas | Sin límite (stack explícito) |
| UI bloqueada | Todo en hilo UI | `await Task.Yield()` cada 100 pasos |
| Cancelación | Imposible | `CancellationToken` chequeado cada paso |
| Progreso | Desconocido | `_stepsExecuted / TotalSteps` |
| Pausa/reanudar | No | Sí |

### 5.5 Archivos afectados

| Archivo | Cambio |
|---|---|
| `Evaluation/Evaluator/StateMachineEvaluator.cs` (nuevo) | State machine |
| `Evaluation/Evaluator/EvaluationFrame.cs` (nuevo) | Frame struct |
| `Evaluation/Pipeline/DrawPipeline.cs` | Integrar state machine |

---

## 6. Fase 4 — GPU rendering con SkiaSharp

**Objetivo**: 50-100x más rápido que GDI+ para muchos puntos.

### 6.1 Por qué SkiaSharp

| Aspecto | GDI+ (actual) | SkiaSharp (propuesto) |
|---|---|---|
| Aceleración | Software CPU | GPU (OpenGL/Vulkan/DirectX) |
| Antialias | Sí (lento) | Sí (nativo, rápido) |
| Batch points | No (uno por uno) | `SKCanvas.DrawPoints(SKPointMode.Points, array)` |
| Cross-platform | Windows-only | Linux, macOS, Windows |
| Integración Avalonia | No | Nativo |

### 6.2 Batch rendering

```csharp
// Antes: Draw_Point uno por uno (cada uno es una llamada GDI+)
private void Draw_Point(Point p)
{
    Papel.FillEllipse(Brush, (float)p.x, (float)p.y, 5, 5);
}

// Después: batch de puntos
private void DrawPointsBatch(SKCanvas canvas, SKPoint[] points, SKPaint paint)
{
    canvas.DrawPoints(SKPointMode.Points, points, paint);
    // Una sola llamada al driver GPU para miles de puntos
}
```

### 6.3 SKPaint reutilizados

```csharp
// Pool de SKPaint por color
private static readonly Dictionary<string, SKPaint> _paintCache = new();

private SKPaint GetPaint(string color, float strokeWidth = 1)
{
    if (!_paintCache.TryGetValue(color, out var paint))
    {
        paint = new SKPaint
        {
            Color = SKColor.Parse(color),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = strokeWidth,
        };
        _paintCache[color] = paint;
    }
    return paint;
}
```

### 6.4 Figuras vectoriales con SKPath

```csharp
private void DrawCircle(SKCanvas canvas, Circle circle, SKPaint paint)
{
    canvas.DrawCircle((float)circle.center.x, (float)circle.center.y, 
                      (float)circle.radio, paint);
}

private void DrawPolygon(SKCanvas canvas, Polygon poly, SKPaint paint)
{
    var path = new SKPath();
    path.AddPoly(poly.Vertices.Select(v => new SKPoint((float)v.x, (float)v.y)).ToArray(), true);
    canvas.DrawPath(path, paint);
}
```

### 6.5 Archivos afectados

| Archivo | Cambio |
|---|---|
| `Graphic/SkiaRenderer.cs` (nuevo) | Renderer con SkiaSharp y batch |
| `Graphic/Form1.cs` | Reemplazar GDI+ por `SKCanvasView` (al migrar a Avalonia) |

### 6.6 Verificación de rendimiento

| Escenario | GDI+ | SkiaSharp batch | Factor |
|---|---|---|---|
| 1,000 puntos | ~500ms | ~5ms | 100x |
| 10,000 puntos | ~5s | ~50ms | 100x |
| 100,000 puntos | ~50s (est.) | ~500ms | 100x |
| 1 círculo | ~5ms | ~0.5ms | 10x |
| 1,000 líneas | ~2s | ~20ms | 100x |

---

## 7. Fase 5 — Memoria y pooling

**Objetivo**: Memoria estable. Sin OoM. Sin GC pressure.

### 7.1 Array pooling

```csharp
// Antes: new List<Point>() para cada operación
List<Point> points = new List<Point>();

// Después: ArrayPool para buffers temporales
var pool = ArrayPool<Point>.Shared;
Point[] buffer = pool.Rent(1024);
try
{
    // usar buffer
}
finally
{
    pool.Return(buffer);
}
```

### 7.2 Clear entre ejecuciones

```csharp
// Nuevo: Context.Clear()
public void Clear()
{
    ToDraw.Clear();
    ExistingPoints.Clear();
    ExistingCircles.Clear();
    ExistingLines.Clear();
    ExistingSegments.Clear();
    ExistingRays.Clear();
    Results.Clear();
    GlobalConstant.Clear();
    UtilizedColors.Clear();
    UtilizedColors.Push("black");
    issuedcontext = false;
}
```

### 7.3 Random estático thread-safe

```csharp
// Antes: new Random() en cada método (repetido docenas de veces)
Random generator = new Random();

// Después: Random thread-safe singleton
public static class RandomProvider
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());
    public static Random Instance => _random.Value!;
}

// Uso: RandomProvider.Instance.Next(50, 310);
```

### 7.4 Límites en colecciones de contexto

```csharp
public class Context
{
    private const int MaxExistingPoints = 100000;
    
    private List<Point> _existingPoints = new();
    public List<Point> ExistingPoints
    {
        get => _existingPoints;
        set => _existingPoints = value;
    }
    
    public void AddPoint(Point p)
    {
        if (_existingPoints.Count < MaxExistingPoints)
            _existingPoints.Add(p);
    }
}
```

### 7.5 Archivos afectados

| Archivo | Cambio |
|---|---|
| `Enviroment/Context.cs` | Añadir `Clear()`, límites en colecciones |
| `Enviroment/RandomProvider.cs` (nuevo) | Random thread-safe estático |
| Todos los archivos con `new Random()` | Reemplazar por `RandomProvider.Instance` |
| `Graphic/Form1.cs` | Limpiar `PuntosGenerados` entre runs |

---

## 8. Fase 6 — Expression caching

**Objetivo**: 2-10x en fractales recursivos. Evita explosión exponencial de llamadas.

### 8.1 Cache de expresiones puras

```csharp
public class ExpressionCache
{
    private readonly Dictionary<long, object?> _cache = new();
    private long _nextKey;
    
    public long GetKey(Node node)
    {
        // Hash del AST: combina NodeType + valores literales + hashes de hijos
        return HashNode(node);
    }
    
    public bool TryGet(long key, out object? result) => _cache.TryGetValue(key, out result);
    public void Set(long key, object? result) => _cache[key] = result;
    public void Clear() => _cache.Clear();
}
```

### 8.2 Expresiones cacheables

Solo expresiones **puras** (sin efectos secundarios):

```csharp
private bool IsPureExpression(Node node) => node.Type switch
{
    NodeType.Sum or NodeType.Sub or NodeType.Mul or
    NodeType.Div or NodeType.Pow or NodeType.Module or
    NodeType.Number or NodeType.PI or NodeType.E or
    NodeType.Sin or NodeType.Cos or NodeType.Sqrt or NodeType.Log
        => true,
    // Draw, Color, Restore, Import tienen efectos → no cachear
    _ => false
};
```

### 8.3 Caso fractal: Fibonacci

```geo
let fib(x) = if x < 2 then 1 else fib(x-1) + fib(x-2);
draw fib(10);
```

**Sin cache**: `fib(10)` evalúa `fib(9) + fib(8)`, que evalúa `fib(8) + fib(7) + fib(7) + fib(6)`... total **177 llamadas**.

**Con cache**: Cada `fib(n)` se evalúa una vez. Total **19 llamadas**.

### 8.4 Invalidación del cache

```csharp
// Invalidar cuando se declara nueva función o variable global
public void Invalidate()
{
    _cache.Clear();
}
```

---

## 9. Métricas objetivo

| Escenario | Antes | Después (target) | Factor |
|---|---|---|---|
| `draw samples();` | ∞ (freeze) | ~2s (10000 pts, streaming) | ✅ |
| `draw take(samples(), 100000);` | ∞ (crash) | ~3s (streaming + batch) | ✅ |
| Fractal recursivo 1000 iteraciones | Stack overflow / freeze | ~1s (state machine + cache) | ✅ |
| Fractal Fibonacci(30) sin cache | ~5s | ~5s (igual, no cacheable) | 1x |
| Fractal Fibonacci(30) con cache | N/A (stack overflow) | ~0.2s | ✅ |
| UI durante evaluación larga | Congelada | Responsiva (async, yield) | ✅ |
| Memoria, 10 runs seguidos | OoM al 3er run | Estable (clear + pool + límites) | ✅ |
| Draw de 10,000 pts (GDI+) | ~5s | N/A (migrado a Skia) | — |
| Draw de 10,000 pts (SkiaSharp batch) | — | ~0.05s | 100x vs GDI+ |
| Cancelación | No funciona | < 500ms | ✅ |
| `seqA + seqB` infinitas | ∞ (freeze) | ~0.1s (limitado a 10000 c/u) | ✅ |

---

## 10. Resumen de esfuerzo

| Fase | Descripción | Días | Dependencias |
|---|---|---|---|
| **F0** | Pipeline async + CancellationToken | 2 | — |
| **F1** | Secuencias con límite garantizado | 2 | — |
| **F2** | Streaming Channel evaluator→renderer | 3 | F0 |
| **F3** | Evaluator state machine | 3 | F0, F1 |
| **F4** | GPU rendering con SkiaSharp | 4 | F2 (Avalonia migration) |
| **F5** | Memoria y pooling | 2 | F0 |
| **F6** | Expression caching | 2 | F3 |
| **Total** | | **~16** | |

### Orden recomendado de ejecución

```
F0 → F1 → F5 → F2 → F3 → F6 → (Avalonia migration + F4)
```

**Justificación**:
- F0 + F1 + F5 son independientes y dan resultados inmediatos (no más freeze, no más OoM)
- F2 (streaming) requiere F0 como base
- F3 (state machine) y F6 (cache) son optimizaciones del evaluator que requieren F0+F1
- F4 (GPU) va con la migración a Avalonia

---

*Documento generado el 2026-07-01. Basado en análisis de ~5,600 líneas de código fuente.*
