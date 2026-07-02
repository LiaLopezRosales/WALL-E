# WALL-E — Roadmap de Implementación

Plan unificado que integra todos los documentos de mejora:
[`IMPROVEMENT_PLAN.md`](./IMPROVEMENT_PLAN.md) · [`PERFORMANCE_PLAN.md`](./PERFORMANCE_PLAN.md) · [`ENHANCEMENTS.md`](./ENHANCEMENTS.md)

Cada paso indica su origen entre `[ ]`.

---

## Timeline consolidado

```
Semana  1  2  3  4  5  6  7  8  9  10  11  12  13  14  15  16
Fase 0  ████████
Fase 1      ██████████████
Fase 2          ██████████████████
Fase 3                  ██████████████
Fase 4                         ██████████████
Fase 5                                ████████████
Fase 6                                       ████████
```

**Total estimado**: ~16 semanas (~4 meses) a tiempo completo.
**Dependencia clave**: Fase 5 (tests) debe correr en CI desde el principio.

---

## Fase 0: Foundation & Critical Fixes

**Duración**: ~1.5 semanas · **Objetivo**: La app no congela, no crashea, UI responsiva.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 0.1 | Pipeline async: `ActionButton_Click` → `async Task`, `CancellationTokenSource` en UI | [PERF-F0] | 1 | `Form1.cs` |
| 0.2 | `CancellationToken` en Evaluator + ArchiveAnalysis, chequeo en cada nodo | [PERF-F0] | 1 | `Evaluator.cs`, `ArchiveAnallizer.cs`, `GeneralEvaluator.cs` |
| 0.3 | Botón Stop wireado a `cts.Cancel()` | [PERF-F0] | 0.5 | `Form1.cs` |
| 0.4 | `volatile` en flag Continue (mientras exista) | [PERF-§1.1] | 0.1 | `Form1.cs` |
| 0.5 | `MaxElements` global en `AbsSequence` (default 10,000). Toda iteración se detiene al alcanzarlo | [PERF-F1] | 1 | `Sequence.cs`, `Infinite Sequence.cs`, `Finite_Sequence.cs` |
| 0.6 | `Take<T>` wrapper + `take(expr, n)` en el DSL | [PERF-F1][ENH-§2.2] | 1 | `Lexer.cs`, `Parser.cs`, `TakenSequence.cs` (nuevo), `Evaluator.cs` |
| 0.7 | Concat segura: `Sum.cs` y `Sequence Concatenation.cs` con límite por secuencia | [PERF-F1] | 0.5 | `Sum.cs`, `Sequence Concatenation.cs` |
| 0.8 | Bug: `GenerateSamples` — nueva instancia de `Point` por iteración | [PERF-§3.5][IMPR-§2.2.E] | 0.2 | `Context.cs` |
| 0.9 | Bug: `ParseSum_O_Sub` — `NodeExpression = "-"` en else branch | [IMPR-§2.2.F] | 0.2 | `Parser.cs` |
| 0.10 | `Keys.Contains` → `ContainsKey` en todo `Evaluator.cs` | [IMPR-§2.2.H] | 0.3 | `Evaluator.cs` |
| 0.11 | Random estático thread-safe (`ThreadLocal<Random>`) | [PERF-F5] | 0.3 | `RandomProvider.cs` (nuevo), todos los `new Random()` |
| 0.12 | `Context.Clear()` entre ejecuciones. Reset de `ExistingPoints`, `ToDraw`, `Results`, etc. | [PERF-F5][IMPR-§3.5] | 0.3 | `Context.cs` |
| 0.13 | Límites en colecciones del Context (`ExistingPoints` ≤ 100k, etc.) | [PERF-F5] | 0.3 | `Context.cs` |
| 0.14 | Eliminar código muerto (~100 líneas comentadas en `Evaluator.cs`, `Form1.cs`, `Circle.cs`) | [IMPR-§2.2.G] | 0.3 | `Evaluator.cs`, `Form1.cs`, `Circle.cs` |
| 0.15 | GDI+ `Dispose`: marcar Pen/Brush para migración o agregar `using` | [IMPR-§2.6] | 0.3 | `Form1.cs` |

**Checkpoint**: `draw samples();` dibuja 10k pts y se detiene. Fractal no congela. Stop funciona. UI responsiva durante evaluación.

### Código clave: pipeline async

```csharp
// Form1.cs — antes: síncrono, bloquea UI
private void ActionButton_Click(object sender, EventArgs e) { ... }

// Form1.cs — después: async, UI responsiva
private CancellationTokenSource _cts = new();

private async void ActionButton_Click(object sender, EventArgs e)
{
    _cts = new CancellationTokenSource();
    try
    {
        var result = await Task.Run(() => ProcessCode(_cts.Token), _cts.Token);
        await RenderResultAsync(result, _cts.Token);
    }
    catch (OperationCanceledException) { /* user cancelled */ }
}

private void StopButton_Click(object sender, EventArgs e) => _cts.Cancel();
```

### Código clave: secuencia limitada

```csharp
// Sequence.cs
public abstract class AbsSequence
{
    public const long DefaultMaxElements = 10000;
    public long MaxElements { get; set; } = DefaultMaxElements;
    public bool IsInfinite => count < 0;
}

// Sum.cs — GenerateSafe con límite
IEnumerable<object> GenerateSafe(AbsSequence r, AbsSequence l)
{
    long limit = r.IsInfinite ? r.MaxElements : r.count;
    long taken = 0;
    foreach (object item in r.Sequence!)
        if (taken++ >= limit) break; else yield return item;
    taken = 0;
    limit = l.IsInfinite ? l.MaxElements : l.count;
    foreach (object item in l.Sequence!)
        if (taken++ >= limit) break; else yield return item;
}
```

---

## Fase 1: Core Extraction & Architecture

**Duración**: ~2.5 semanas · **Objetivo**: Lógica separada de UI. Evaluator state machine. Cachable, testeable.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 1.1 | Separar `Figure`, `ArchiveAnalysis`, `Evaluator` de `Form` (anti-patrón crítico) | [IMPR-§2.1] | 2 | `Figure.cs`, `ArchiveAnallizer.cs`, `Evaluator.cs` |
| 1.2 | Crear `Wall-E.Core` (class library, net6.0, sin Windows). Mover Lexer/Parser/AST/Evaluation/Enviroment | [IMPR-§5.3] | 1 | `Wall-E.Core.csproj` (nuevo), mover archivos |
| 1.3 | Extraer `StoreVariable(name, value)` — elimina ~35 duplicaciones | [IMPR-§2.2] | 1 | `Evaluator.cs` |
| 1.4 | Dividir `GeneralEvaluation` en métodos por tipo + `switch` expression | [IMPR-§2.3] | 3 | `Evaluator.cs` |
| 1.5 | State machine evaluator (stack explícito, sin recursión real) | [PERF-F3] | 3 | `StateMachineEvaluator.cs` (nuevo), `EvaluationFrame.cs` (nuevo) |
| 1.6 | Expression caching para expresiones puras (sin efectos secundarios) | [PERF-F6] | 1.5 | `ExpressionCache.cs` (nuevo), `Evaluator.cs` |
| 1.7 | Interfaces `IEvaluator`, `ILexer`, `IParser` + DI básica | [IMPR-§4.2-3] | 1 | interfaces nuevas, modificar constructores |
| 1.8 | Solución `Wall-E.sln` con `src/` + estructura profesional | [IMPR-§5.3] | 0.3 | `Wall-E.sln` (nuevo) |
| 1.9 | Build en Linux: `dotnet build Wall-E.Core` pasa | — | 0.3 | csproj adjustments |

**Checkpoint**: Core compila independiente de WinForms. Evaluator es state machine. Cache funciona en fractales recursivos.

### Código clave: state machine

```csharp
public readonly struct EvaluationFrame
{
    public readonly Node Node;
    public readonly int State;       // 0=pendiente, 1=left listo, 2=both listos
    public readonly object? LeftResult;
    public readonly object? RightResult;
}

public class StateMachineEvaluator
{
    private readonly Stack<EvaluationFrame> _stack = new();
    public const int MaxStepsPerBatch = 100;

    public bool Step(int maxSteps, CancellationToken ct)
    {
        for (int i = 0; i < maxSteps; i++)
        {
            ct.ThrowIfCancellationRequested();
            if (_stack.Count == 0) return false;
            StepOne();
        }
        return _stack.Count > 0;
    }

    private void StepOne()
    {
        var frame = _stack.Pop();
        switch (frame.State, frame.Node.Type)
        {
            case (0, NodeType.Sum):
                _stack.Push(frame);
                _stack.Push(new EvaluationFrame(frame.Node.Branches[0]));
                break;
            case (1, NodeType.Sum):
                _stack.Push(frame.WithLeftResult(result));
                _stack.Push(new EvaluationFrame(frame.Node.Branches[1]));
                break;
            case (2, NodeType.Sum):
                double sum = (double)frame.LeftResult! + (double)frame.RightResult!;
                ReturnToParent(sum);
                break;
            // ... ~40 casos más
        }
    }
}
```

### Código clave: expression cache

```csharp
public class ExpressionCache
{
    private readonly Dictionary<long, object?> _cache = new();

    public long Key(Node node) => HashNode(node);
    public bool TryGet(long key, out object? result) => _cache.TryGetValue(key, out result);
    public void Set(long key, object? result) => _cache[key] = result;
    public void Clear() => _cache.Clear();
    public void Invalidate() => Clear();  // al declarar nueva función/variable

    private long HashNode(Node node) => HashCode.Combine(
        (int)node.Type,
        node.NodeExpression?.GetHashCode() ?? 0,
        node.Branches?.Count ?? 0,
        node.Branches?.Sum(b => HashNode(b)) ?? 0
    );
}
```

---

## Fase 2: Avalonia Migration + Streaming

**Duración**: ~3.5 semanas · **Objetivo**: Nueva UI con streaming progresivo.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 2.1 | Nuevo proyecto `Wall-E.Avalonia` con referencia a `Wall-E.Core` | [IMPR-§5.1] | 0.5 | `Wall-E.Avalonia.csproj` (nuevo) |
| 2.2 | Ventana principal: editor TextBox + canvas `SKCanvasView` + botones Process/Stop/Clean | [IMPR-§5.1-2] | 2 | `MainWindow.axaml`, `MainWindow.axaml.cs` |
| 2.3 | Channel pipeline: `Channel<DrawCommand>` (bounded, cap 1000) | [PERF-F2] | 1.5 | `DrawPipeline.cs` (nuevo) |
| 2.4 | Evaluator escribe al channel (producer). Renderer consume con `ReadAllAsync` (consumer) | [PERF-F2] | 1 | `DrawPipeline.cs`, `Evaluator.cs`, `MainWindow.axaml.cs` |
| 2.5 | Batch rendering: cada 100 DrawCommands, invalidar canvas + `await Task.Yield()` | [PERF-F2][IMPR-§5.4] | 1 | `StreamRenderer.cs` (nuevo) |
| 2.6 | Grid cartesiano con labels en ejes X/Y | [IMPR-§5.3] | 1 | `GridRenderer.cs` (nuevo) |
| 2.7 | Zoom (scroll wheel) + Pan (click+arrastrar) + reset doble-click | [IMPR-§5.4] | 2 | `ViewState.cs` (nuevo), eventos en canvas |
| 2.8 | Sistema de coordenadas: origen configurable + escala | [ENH-§4.3] | 1 | `CoordinateSystem.cs` (nuevo) |
| 2.9 | Color picker visual (Avalonia `ColorPicker`) | [ENH-§1.11] | 1 | toolbar button + dialog |
| 2.10 | StatusBar con coordenadas del cursor en vivo | [IMPR-§5.4] | 0.5 | `MainWindow.axaml` |

**Checkpoint**: App Avalonia corriendo. Dibuja con streaming progresivo. Grid + zoom + pan funcionales.

### Código clave: streaming pipeline

```csharp
// DrawPipeline.cs
public class DrawPipeline
{
    private readonly Channel<DrawCommand> _channel = Channel.CreateBounded<DrawCommand>(
        new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.Wait });

    public ChannelWriter<DrawCommand> Writer => _channel.Writer;
    public ChannelReader<DrawCommand> Reader => _channel.Reader;
    public CancellationTokenSource Cts { get; } = new();
}

// Evaluator escribe al channel
await writer.WriteAsync(new DrawCommand(figure, tag, color), ct);

// Renderer consume del channel
await foreach (DrawCommand cmd in reader.ReadAllAsync(ct))
{
    DrawOne(cmd);
    if (++_batch >= 100) { _batch = 0; canvas.Invalidate(); await Task.Yield(); }
}
```

### Arquitectura de ventana Avalonia

```
┌─────────────────────────────────────────────────────┐
│  Wall-E.Avalonia                                     │
│  ┌──────────────┐  ┌──────────────────────────────┐ │
│  │ Editor Panel  │  │ SKCanvasView (SkiaSharp)     │ │
│  │ ┌──────────┐ │  │  ┌─────────────────────────┐ │ │
│  │ │ Comandos  │ │  │  │ Grid + Axes + Labels    │ │ │
│  │ │ (TextBox) │ │  │  │ Figures + Colors + Tags │ │ │
│  │ └──────────┘ │  │  │ Zoom/Pan overlay         │ │ │
│  │ [Process]    │  │  └─────────────────────────┘ │ │
│  │ [Stop]       │  │                              │ │
│  │ [Clean]      │  │  StatusBar: "drawing..."     │ │
│  └──────────────┘  └──────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

---

## Fase 3: GPU Rendering + Estética Premium

**Duración**: ~2.5 semanas · **Objetivo**: Rendimiento GPU, look profesional.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 3.1 | Batch de puntos con `SKCanvas.DrawPoints(SKPointMode.Points, array)` | [PERF-F4] | 1 | `SkiaRenderer.cs` |
| 3.2 | Pool de `SKPaint` reutilizados por color | [PERF-F4] | 0.5 | `PaintPool.cs` (nuevo) |
| 3.3 | `SKPath` para figuras (DrawCircle, DrawLine, DrawArc nativos de Skia) | [PERF-F4] | 1 | `SkiaRenderer.cs` |
| 3.4 | Anti-aliasing + sombras suaves en figuras | [IMPR-§5.3] | 1 | `SkiaRenderer.cs` |
| 3.5 | Temas claro/oscuro premium con paleta profesional | [IMPR-§5.3] | 1 | `Theme.cs` (nuevo) |
| 3.6 | Sistema de capas (z-order): `layer N; draw ...;` | [ENH-§4.1] | 1.5 | `Context.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 3.7 | Selector de color visual en toolbar | [ENH-§1.11] | 1 | `MainWindow.axaml` |
| 3.8 | Snap a grid configurable: `snap 0.5;` | [ENH-§4.4] | 0.5 | `Evaluator.cs`, `CoordinateSystem.cs` |
| 3.9 | Estilos de línea (dashed, dotted, dash-dot) + grosor | [ENH-§3.4] | 1 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 3.10 | Relleno de figuras (solid, gradient, none) | [ENH-§3.5] | 1.5 | `DrawObject.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 3.11 | Ocultar/mostrar figuras: `hide l;` / `show l;` | [ENH-§4.2] | 0.5 | `Context.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 3.12 | Exportar PNG: `SKCanvas.Snapshot()` → `SKImage.Encode(Png, 100)` → `File.WriteAllBytes` | [IMPR-§5.4] | 0.5 | `ExportService.cs` (nuevo) |

**Checkpoint**: 100k puntos en < 0.5s. Temas, capas, estilos, relleno. Export PNG.

### Código clave: batch GPU

```csharp
// Antes: GDI+ uno por uno
private void Draw_Point(Point p) =>
    Papel.FillEllipse(Brush, (float)p.x, (float)p.y, 5, 5);

// Después: SkiaSharp batch
private void DrawPointsBatch(SKCanvas canvas, SKPoint[] points, SKPaint paint)
{
    canvas.DrawPoints(SKPointMode.Points, points, paint);
    // Una llamada al driver GPU para miles de puntos
}
```

### Código clave: tema claro/oscuro

```csharp
public record Theme
{
    public static readonly Theme Light = new()
    {
        Background = SKColors.WhiteSmoke,
        GridLines  = new SKColor(0, 0, 0, 30),
        GridAxis   = new SKColor(0, 0, 0, 100),
        AxisLabels = new SKColor(80, 80, 80),
    };

    public static readonly Theme Dark = new()
    {
        Background = new SKColor(30, 30, 30),
        GridLines  = new SKColor(255, 255, 255, 20),
        GridAxis   = new SKColor(255, 255, 255, 80),
        AxisLabels = new SKColor(180, 180, 180),
    };

    public SKColor Background { get; init; }
    public SKColor GridLines  { get; init; }
    public SKColor GridAxis   { get; init; }
    public SKColor AxisLabels { get; init; }
}
```

---

## Fase 4: Language Enhancements

**Duración**: ~2.5 semanas · **Objetivo**: DSL rico — colores completos, figuras nuevas, loops.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 4.1 | Color `#hex` (16M colores) | [ENH-§1.2] | 1 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 4.2 | Color `rgb(r,g,b)` + `rgba(r,g,b,a)` | [ENH-§1.3-4] | 1 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs` |
| 4.3 | Color `hsl(h,s%,l%)` | [ENH-§1.5] | 1 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs` |
| 4.4 | Colores CSS completos (~140 nombres) | [ENH-§1.6] | 0.5 | `ColorTable.cs` (nuevo), `Evaluator.cs` |
| 4.5 | Gradientes en fill: `fill linear(red, blue)` / `fill radial(red, blue)` | [ENH-§1.7] | 1.5 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 4.6 | Operaciones cromáticas: `lighten`, `darken`, `mix`, `complement` | [ENH-§1.8] | 1.5 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs` |
| 4.7 | Comentarios en DSL (`#` / `//`) | [ENH-§2.1] | 0.3 | `Lexer.cs` |
| 4.8 | `seed(n)` para random determinista | [ENH-§2.2] | 0.3 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs`, `RandomProvider.cs` |
| 4.9 | `print` / `debug` para output en statusbar | [ENH-§2.3] | 1 | `Lexer.cs`, `Parser.cs`, `Evaluator.cs`, `MainWindow.axaml.cs` |
| 4.10 | Polígono regular: `polygon(center, radius, n)` | [ENH-§3.1] | 2 | `Polygon.cs` (nuevo), `Parser.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 4.11 | Elipse: `ellipse(center, rx, ry)` | [ENH-§3.2] | 1.5 | `Ellipse.cs` (nuevo), `Parser.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 4.12 | Texto/etiquetas: `label(point, "text", size=14)` | [ENH-§3.3] | 1.5 | `LabelFigure.cs` (nuevo), `Parser.cs`, `Evaluator.cs`, `SkiaRenderer.cs` |
| 4.13 | Bucles: `for i in seq { ... }`, `repeat(n) { ... }` | [ENH-§2.7] | 3 | `AST.cs`, `Parser.cs`, `StateMachineEvaluator.cs` |
| 4.14 | Constantes extra (`phi`, `sqrt2`) + funciones math (`tan`, `atan`, `abs`, `floor`, `ceil`) | [ENH-§2.5-6] | 0.5 | `Lexer.cs`, `Context.cs` |
| 4.15 | Animación paramétrica: `animate(t from 0 to 2*PI) { ... }` | [ENH-§4.5] | 2.5 | `AST.cs`, `Parser.cs`, `StateMachineEvaluator.cs`, `StreamRenderer.cs` |

**Checkpoint**: DSL completo. Colores ilimitados. Polígono, elipse, texto, loops, animación.

---

## Fase 5: Professional Polish

**Duración**: ~2 semanas · **Objetivo**: Tests, CI, calidad de código profesional.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 5.1 | Proyecto `Wall-E.Core.Tests` con xUnit | [IMPR-§4.1] | 1 | `Wall-E.Core.Tests.csproj` (nuevo) |
| 5.2 | Tests de lexer: tokenización correcta de cada token type | [IMPR-§4.1] | 1 | `LexerTests.cs` |
| 5.3 | Tests de parser: AST correcto para cada statement | [IMPR-§4.1] | 1 | `ParserTests.cs` |
| 5.4 | Tests de evaluator: cada operación (aritmética, figuras, draw, color, funciones) | [IMPR-§4.1] | 3 | `EvaluatorTests.cs` |
| 5.5 | Tests de secuencias: finite, infinite (limitado), concatenación, take, count | [IMPR-§4.1] | 1.5 | `SequenceTests.cs` |
| 5.6 | Tests de intersección: cada combinación de figuras | [IMPR-§4.1] | 2 | `IntersectionTests.cs` |
| 5.7 | Tests de integración: archivos `.geo` completos | [IMPR-§4.1] | 1 | `IntegrationTests.cs` |
| 5.8 | GitHub Actions CI: build en ubuntu + test en windows-latest | [IMPR-§4.4] | 1 | `.github/workflows/ci.yml` (nuevo) |
| 5.9 | `.editorconfig` con reglas del proyecto | [IMPR-§5.5] | 0.3 | `.editorconfig` (nuevo) |
| 5.10 | Roslynator + SonarAnalyzer configurados | [IMPR-§5.5] | 0.5 | `Directory.Build.props` (nuevo) |
| 5.11 | XML doc comments en API pública de `Wall-E.Core` | [IMPR-§5.5] | 1 | todos los archivos de Core |
| 5.12 | Git workflow: conventional commits, branch strategy, PR template | [IMPR-§4.5] | 0.5 | `.github/PULL_REQUEST_TEMPLATE.md` |

**Checkpoint**: `dotnet test` pasa en CI. Código analizado en cada PR. Documentación XML pública.

### Workflow CI

```yaml
# .github/workflows/ci.yml
name: CI
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '6.0' }
      - run: dotnet restore
      - run: dotnet build --no-restore -c Release
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '6.0' }
      - run: dotnet test -c Release --verbosity normal
```

### Estructura final del proyecto

```
WALL-E/
  Wall-E.sln
  src/
    Wall-E.Core/
      Lexer/        Lexer.cs, TokenStream.cs
      Parser/       Parser.cs, GeneralParser.cs
      AST/
        Node.cs
        Expression/
        Geometric Objects/  Point.cs, Line.cs, Circle.cs, Arc.cs, Polygon.cs, Ellipse.cs
        Sequence/    Sequence.cs, Infinite Sequence.cs, Finite_Sequence.cs, TakenSequence.cs
      Evaluation/
        Evaluator.cs, StateMachineEvaluator.cs, ExpressionCache.cs
        GeneralEvaluator.cs
      Enviroment/
        Context.cs, Scope.cs, DrawObject.cs, Error.cs, Function.cs, RandomProvider.cs
      Wall-E.Core.csproj (net6.0)
    Wall-E.Avalonia/
      MainWindow.axaml
      MainWindow.axaml.cs
      Rendering/    SkiaRenderer.cs, StreamRenderer.cs, GridRenderer.cs, ViewState.cs
      Pipeline/     DrawPipeline.cs, DrawCommand.cs
      Theme.cs, PaintPool.cs, ExportService.cs
      Wall-E.Avalonia.csproj (net6.0)
  tests/
    Wall-E.Core.Tests/
      LexerTests.cs, ParserTests.cs, EvaluatorTests.cs
      SequenceTests.cs, IntersectionTests.cs, IntegrationTests.cs
      Wall-E.Core.Tests.csproj (net6.0)
  GeoLibrary/       demo.geo, fractal.geo, samples.geo
  .github/
    workflows/ci.yml
    PULL_REQUEST_TEMPLATE.md
  .editorconfig
  README.md
  LICENSE
  AGENTS.md
  IMPROVEMENT_PLAN.md
  PERFORMANCE_PLAN.md
  ENHANCEMENTS.md
  ROADMAP.md
```

---

## Fase 6: Portfolio Finalization

**Duración**: ~1 semana · **Objetivo**: Presentación profesional lista para reclutadores.

| # | Tarea | Origen | Días | Archivos |
|---|---|---|---|---|
| 6.1 | README.md en inglés con badges (build, license, .NET) + ejemplos de código + screenshot | [IMPR-§6.1] | 1 | `README.md` |
| 6.2 | Screenshots de alta calidad: demo figuras, grid, tema oscuro, highlighting | [IMPR-§6.2] | 1 | `docs/screenshots/` |
| 6.3 | Archivos `.geo` de ejemplo (demo, fractales, samples) en GeoLibrary | [IMPR-§6.4] | 1 | `GeoLibrary/demo.geo`, `fractal.geo` |
| 6.4 | LICENSE (MIT) | [IMPR-§6.3] | 0.1 | `LICENSE` (nuevo) |
| 6.5 | CLI tool: `Wall-E.Cli` para procesar .geo headless (`input.geo -o output.png`) | [ENH-§6.4] | 2 | `Wall-E.Cli.csproj` (nuevo), `Program.cs` |
| 6.6 | Exportar SVG: generar XML SVG desde figuras | [ENH-§6.1] | 2 | `SvgExporter.cs` (nuevo) |
| 6.7 | Demo online: publicar Avalonia Wasm en GitHub Pages | [IMPR-§6.5] | 1 | `docs/` folder + GH Pages config |
| 6.8 | Syntax highlighting en el editor del DSL (AvaloniaEdit o Skia custom) | [IMPR-§5.5][ENH-§1] | 2 | `DslHighlighting.cs` (nuevo), `MainWindow.axaml` |

**Checkpoint**: README profesional, ejemplos, CLI, demo online, LICENSE. Listo para compartir.

### Badges para README

```markdown
![Build](https://img.shields.io/github/actions/workflow/status/user/WALL-E/ci.yml?branch=main)
![License](https://img.shields.io/github/license/user/WALL-E)
![.NET](https://img.shields.io/badge/.NET-6.0-512BD4)
![Platform](https://img.shields.io/badge/platform-win%20%7C%20linux%20%7C%20mac-lightgrey)
```

---

## Tabla de trazabilidad completa

| Origen | Se asigna a |
|---|---|
| `PERFORMANCE_PLAN.md` F0 | Fase 0 (#0.1-0.4) |
| `PERFORMANCE_PLAN.md` F1 | Fase 0 (#0.5-0.7) |
| `PERFORMANCE_PLAN.md` F2 | Fase 2 (#2.3-2.5) |
| `PERFORMANCE_PLAN.md` F3 | Fase 1 (#1.5) |
| `PERFORMANCE_PLAN.md` F4 | Fase 3 (#3.1-3.3) |
| `PERFORMANCE_PLAN.md` F5 | Fase 0 (#0.11-0.13) |
| `PERFORMANCE_PLAN.md` F6 | Fase 1 (#1.6) |
| `IMPROVEMENT_PLAN.md` §2.2 (bugs) | Fase 0 (#0.8-0.10, 0.14-0.15) |
| `IMPROVEMENT_PLAN.md` §2.1 (classes from Form) | Fase 1 (#1.1) |
| `IMPROVEMENT_PLAN.md` §2.2 (refactor) | Fase 1 (#1.3-1.4) |
| `IMPROVEMENT_PLAN.md` §3 (optimizaciones) | Fase 0 (#0.11-0.13), Fase 1 (#1.6) |
| `IMPROVEMENT_PLAN.md` §5 (migración UI) | Fase 2 (#2.1-2.10) |
| `IMPROVEMENT_PLAN.md` §5.3 (estética) | Fase 3 (#3.4-3.12) |
| `IMPROVEMENT_PLAN.md` §4 (prácticas) | Fase 5 (#5.1-5.12) |
| `IMPROVEMENT_PLAN.md` §6 (documentación) | Fase 6 (#6.1-6.4, 6.7) |
| `ENHANCEMENTS.md` §1 (color) | Fase 4 (#4.1-4.6), Fase 3 (#3.7) |
| `ENHANCEMENTS.md` §2 (DSL) | Fase 4 (#4.7-4.9, 4.13-4.14) |
| `ENHANCEMENTS.md` §3 (figuras) | Fase 4 (#4.10-4.12) |
| `ENHANCEMENTS.md` §4 (dibujo) | Fase 3 (#3.6, 3.8, 3.11), Fase 4 (#4.15) |
| `ENHANCEMENTS.md` §5 (performance) | Fase 0 (#0.5-0.6) |
| `ENHANCEMENTS.md` §6 (I/O) | Fase 6 (#6.5-6.6) |

---

## Lo que NO entra en este plan

| Mejora | Motivo de exclusión |
|---|---|
| Undo/redo en comandos | Esfuerzo alto (~5d), impacto medio en entrevista |
| Arrastrar puntos interactivamente | Requiere sistema de selección + hit testing complejo |
| Auto-completado del DSL | IA/búsqueda, desvío del core del proyecto |
| Evaluación incremental (diff AST) | Alta complejidad, mejora marginal vs streaming |
| Import/export DXF / GeoJSON | Nicho, esfuerzo alto por formato |
| Macros en el DSL | Complejo, poca visibilidad en portafolio |
| System clock / env vars | Irrelevante para geometría |
| Tuplas, mapas, records en DSL | Cambio profundo en sistema de tipos (~8d) |
| Break/continue en loops | Baja prioridad vs loops básicos |

---

*Documento generado el 2026-07-01. Integra IMPROVEMENT_PLAN.md, PERFORMANCE_PLAN.md y ENHANCEMENTS.md en un roadmap unificado de 7 fases y ~16 semanas.*
