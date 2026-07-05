# WALL-E — Plan de Mejora Integral

Plan estratégico para transformar GeoWall-E de proyecto escolar a portafolio profesional listo para entrevistas.

---

## Índice

1. [Resumen ejecutivo y roadmap](#1-resumen-ejecutivo)
2. [Diagnóstico por áreas](#2-diagnóstico-por-áreas)
3. [Refactor de código y eliminación de bugs](#3-refactor-de-código-y-eliminación-de-bugs)
4. [Optimizaciones de ejecución](#4-optimizaciones)
5. [Migración de UI y estética premium](#5-migración-de-ui-y-estética-premium)
6. [Prácticas profesionales](#6-prácticas-profesionales)
7. [Documentación y otros](#7-documentación-y-otros)

---

## 1. Resumen Ejecutivo

### Estado actual

- **5,595 líneas** de C# en un solo proyecto `net6.0-windows`
- **0 tests**, 0 CI, 0 linters
- Monolito de 1,991 líneas (`Evaluator.cs`)
- Windows Forms (obsoleto para portafolio)
- ~35 instancias de código duplicado (variable-storage copiado una y otra vez)
- Bug concreto en `GenerateSamples` y `ParseSum_O_Sub`
- Sin async, sin DI, sin separación de capas

### Roadmap (7 fases, ~18 semanas)

| Fase | Duración | Resultado |
|---|---|---|
| **F0: Foundation** | 1.5 semanas | ✅ App no congela. Async pipeline. MaxElements. Bugs corregidos |
| **F1: Clean Architecture** | 3 semanas | Domain/Application/Infrastructure separados. Visitor Pattern. `IEvaluationResult`. `Result<T,E>` |
| **F2: Avalonia + MVVM** | 3 semanas | UI moderna con MVVM puro, streaming progresivo, grid, zoom/pan |
| **F3: GPU + Cache** | 2.5 semanas | SkiaSharp batch rendering. Expression Cache. Temas claro/oscuro. Estilos |
| **F4: Language++** | 3 semanas | Colores completos, polígono, elipse, bucles, animación |
| **F5: Tests + CI** | 2.5 semanas | xUnit tests, CI, analyzers, .editorconfig |
| **F6: Portfolio** | 1.5 semanas | README, CLI, demo online, syntax highlighting |

**Arquitectura objetivo**: Clean Architecture en 4 capas:

```
┌──────────────────────────────┐
│   Wall-E.UI.Avalonia (MVVM)  │  → SkiaSharp, ViewModels, Themes
├──────────────────────────────┤
│   Wall-E.Application         │  → Pipeline, Caching, DSL Parsing
├──────────────────────────────┤
│   Wall-E.Domain              │  → AST, Figures, Evaluation, Geometry
├──────────────────────────────┤
│   Wall-E.Infrastructure      │  → File I/O, Persistence
└──────────────────────────────┘
```

Cambios clave respecto al plan original:
- `Context` se divide en `EvaluationContext` + `FigureRepository` + `RenderScene`
- `object` return se reemplaza por `IEvaluationResult` sellado
- Switch monolítico se reemplaza por `INodeVisitor<T>`
- Evaluador produce `Scene` inmutable (no conoce Channel)
- MVVM puro con ViewModels inyectados (no code-behind)
- `Result<T, E>` monádico para errores composables

---

## 2. Diagnóstico por Áreas

### 2.1 Funcionalidades

**Fortalezas actuales**

El intérprete DSL ya implementa un conjunto respetable de features: puntos, líneas, segmentos, rayos, círculos, arcos, secuencias (finitas/infinitas/acotadas), operaciones aritméticas, booleanas, trigonometría (sin, cos, sqrt, log), condicional if-then-else, let-in, funciones definidas por usuario, imports, intersección de figuras, manejo de color, y un sistema de dibujo.

**Carencias que más se notan en portafolio**

| Feature | Impacto |
|---|---|
| Renderizado de rejilla/ejes (grilla cartesiana) | Hace que el drawing canvas parezca profesional |
| Zoom + Pan en el canvas | Esperado en cualquier herramienta de dibujo |
| Exportar dibujo (PNG, SVG, PDF) | Demuestra manejo de I/O y formatos |
| Syntax highlighting en el editor de código | Impacto visual inmediato |
| Undo/redo en comandos | Muestra diseño orientado a estado |
| Arrastrar puntos interactivamente (click & drag) | Diferencia entre intérprete escolar y herramienta real |
| Auto-completado de palabras clave del DSL | UX profesional |
| Ejemplos incluidos (demo.geo, sample.geo) | Onboarding inmediato del reclutador |
| Línea de comandos (procesar .geo sin GUI) | Útil para testing y scripting |
| Tooltips, status bar, atajos de teclado | Madurez de UX |

### 2.2 Optimizaciones, Bugs y Limpieza de Código

**Problemas graves**

**A. Duplicación masiva en el Evaluator**  
Las líneas `Evaluator.cs:62-211` (creación de Circle, Point, Line, Segment, Ray) son 5 copias casi exactas del mismo patrón: crear figura aleatoria, validar constantes, almacenar en scope/context, devolver string. Luego el `GlobalSeq` (líneas 399-1001) repite el mismo bloque de variable-storage para 7 tipos de secuencia distintos. La función `Count` (líneas 1243-1315) hace otro tanto.

**Refactor**: Extraer métodos como:
- `StoreVariable(string name, object value)` — 1 vez, no 35
- `CreateRandomFigure(FigureType type)` — 1 vez, no 5
- `CountSequenceElements(object seq)` — 1 vez, no 7

**B. Monolito de 1991 líneas**  
`GeneralEvaluation()` es un solo método con un `if/else if` de ~60 ramas. Imposible de testear, mantener, o incluso leer de corrido.

**C. Jerarquía de clases fantasma**  
`Expression`, `Binary`, `Unit`, `Ternary` existen en `AST/Expression/` pero **no los usa** ni el lexer, ni el parser, ni el evaluator. Todo vive en `Node` con un enum `NodeType` de 82 valores y un `object? NodeExpression`. Esto es código muerto que confunde.

**D. GDI+ Resource Leaks**  
`Form1.cs:17` — `pictureBox1.CreateGraphics()` sin Dispose. `Pen` y `SolidBrush` sin Dispose. En un app WinForms de ciclo corto puede no notarse, pero es mala práctica.

**E. Bug en GenerateSamples** (`Context.cs:66-82`)
```csharp
Point tem = new Point(0,0); // creado UNA vez fuera del ciclo
while (true) {
    tem.RandomPoint(points); // modifica el MISMO objeto cada iteración
    points.Add(tem);         // añade la misma referencia siempre
    yield return tem;        // todos los elementos del sequence son el mismo objeto
}
```
Cada `yield return` devuelve la misma instancia de `Point` mutada. Si alguien guarda los puntos para dibujarlos después, todos serán el último valor.

**F. Bug en ParseSum_O_Sub** (`Parser.cs:922-924`)
```csharp
if (whatkind == Token.TokenType.sum) {
    sus.Type = Node.NodeType.Sum;
    sus.NodeExpression = "+";   // OK
} else sus.Type = Node.NodeType.Sub;
sus.NodeExpression = "-";        // ← se ejecuta SIEMPRE, incluso si es Sum
```
`NodeExpression` queda como `"-"` aunque la operación sea suma. El evaluador no usa `NodeExpression` para operadores (solo los `Branches`), pero es incorrecto y confunde.

**G. Comentarios enormes de código muerto**  
`Evaluator.cs` tiene ~70 líneas de código comentado (stack overflow checking, contextos viejos). `Form1.cs` tiene funciones enteras comentadas (Draw_Point sin parámetros, Draw_Segment). Esto infla el código y despista.

**H. Uso incorrecto de Keys.Contains**  
En todo el `Evaluator.cs`:
```csharp
if (context.GlobalConstant.Keys.Contains(name)) // ← O(n) lookup
```
Debe ser:
```csharp
if (context.GlobalConstant.ContainsKey(name)) // ← O(1) lookup
```

**Problemas medios**

- **Nombres inconsistentes**: mezcla español (`Lapiz`, `Papel`, `PuntosGenerados`), inglés (`DrawObjects`, `ExistingPoints`), y spanglish (`Generar_Punto`)
- **Errores en string con formato de CSV**: `Error.ToString()` retorna `"Invalid, argument, MainFile, 0, 0,"` — formato feísimo para el usuario
- **Sin async/await**: El UI se congela mientras procesa (es un problema real si hay archivos `.geo` grandes o secuencias infinitas)

### 2.3 UI / Interfaz y Aspecto Visual

**Migración obligada: WinForms → algo moderno**

WinForms es una tecnología muerta para reclutadores externos a Microsoft. Para un portafolio, es una señal de alerta. Recomiendo **Avalonia UI** por estas razones:

| Aspecto | Avalonia | MAUI | WinForms (actual) |
|---|---|---|---|
| Cross-platform | ✅ Linux, macOS, Windows, iOS, Android, WebAssembly | ✅ (limitado en Linux) | ❌ Windows-only |
| MVVM nativo | ✅ | ✅ | ❌ |
| XAML binding | ✅ Data-binding fuerte | ✅ | ❌ |
| Popularidad portafolio | Alta (rising) | Media | Muy baja |
| Dibujo 2D | DrawingContext (Skia) | GraphicsView | System.Drawing |
| Dificultad migración | Media | Media | — |

**Implementación**: Un `Avalonia.Controls.Canvas` con un `SKCanvas` de SkiaSharp reemplaza al `PictureBox`. Los objetos geométricos se renderizan con `SKPaint` en lugar de `System.Drawing.Pen/Brush`.

**Arquitectura MVVM pura**:

```
MainViewModel (code, isProcessing, status)
  → EditorViewModel (code text, syntax highlighting)
  → CanvasViewModel (scene, zoom, pan, cursor)
  → StatusBarViewModel (figure count, cursor position)
```

Cada ViewModel se inyecta via constructor (DI manual o `Microsoft.Extensions.DependencyInjection`).  
Zero code-behind en lo posible. Toda la lógica de UI está en ViewModels testables.

**Mejoras visuales que impactan**

| Mejora | Qué demuestra | Dificultad |
|---|---|---|
| Grid cartesiano con labels en ejes X/Y | Matemática + render | Baja |
| Zoom (scroll wheel) + Pan (click+arrastrar) | Interacción avanzada | Media |
| Anti-aliasing en figuras geométricas | Calidad visual | Baja (Skia lo da gratis) |
| Selector de color visual (color picker) en vez de escribir `color blue;` | UX moderna | Media |
| Syntax highlighting en el editor del DSL | Ingeniería de lenguajes | Alta (vale la pena) |
| Tema claro/oscuro | Atención al detalle | Baja |
| Canvas responsivo (redimensionar ventana) | Layout profesional | Baja |
| Toolbar con iconos (Draw, Clean, Zoom In/Out, Export) | Arquitectura MVVM | Media |

### 2.4 Prácticas Profesionales y de Trabajo

**Lo que más pesa en una entrevista**

| Práctica | Estado actual | Impacto en portafolio |
|---|---|---|
| Tests unitarios | ❌ Cero tests | **Crítico** — sin tests no hay ingeniero serio |
| CI/CD | ❌ No existe | Alto |
| SOLID | ❌ Monolito + sin interfaces | Alto |
| Conventional commits | ❌ Commits sin formato | Medio |
| PRs / code review | ❌ Solo branch master | Medio |
| .editorconfig | ❌ No existe | Medio |
| Null safety | ✅ Nullable enable | Bueno |
| ImplicitUsings | ✅ | Bueno |

**Plan de profesionalización**

1. **Tests (prioridad #1 global)**
   - Proyecto de test aparte: `Wall-E.Tests.csproj` con xUnit
   - Testear el evaluator figura por figura: `circle c;` → exists in context
   - Testear operaciones matemáticas: `2 + 2` → `4`
   - Testear el lexer: `draw point(1, 2);` → tokens correctos
   - Testear el parser: árbol AST correcto
   - Testear funciones: `f(x) = x + 1; f(5)` → `6`
   - Mock de `Form` para el evaluator (o extraer la lógica fuera de Form)
   - Cobertura inicial modesta (~40%) es mejor que 0%

2. **Separación de responsabilidades**
   - Extraer `Evaluator`, `ArchiveAnalysis` de `Form` — **nunca debieron heredar de Form**
   - Las clases `Figure`, `Evaluator`, `ArchiveAnalysis` heredan de `Form` sin razón. Sacarlas de esa jerarquía es el refactor más importante.
   - Aplicar interfaz `IEvaluator`, `ILexer`, `IParser`

3. **Inyección de dependencias básica**
   ```csharp
   var lexer = new GeneralLexer(code);
   var parser = new GeneralParser(lexer.Tokenize());
   var evaluator = new Evaluator(parser.Parse());
   var context = evaluator.Evaluate();
   ```
   Cada fase recibe la salida de la anterior, sin acoplamiento.

4. **CI con GitHub Actions**
   Workflow mínimo:
   ```yaml
   on: [push, pull_request]
   jobs:
     build:
       runs-on: ubuntu-latest  # solo build; test necesita Windows
       steps:
         - uses: actions/checkout@v4
         - uses: actions/setup-dotnet@v4
           with:
             dotnet-version: '6.0'
         - run: dotnet build
   ```
   Idealmente agregar un job Windows para tests:
   ```yaml
   test:
     runs-on: windows-latest
     steps:
       - uses: actions/checkout@v4
       - run: dotnet test
   ```

5. **Conventional Commits**: `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:`

### 2.5 Documentación y Otros

**Documentación**

| Documento | Estado | Acción |
|---|---|---|
| README.md | ✅ Existe en español | Traducir a **inglés** + agregar screenshot + ejemplos de código + badge de build |
| AGENTS.md | ✅ Creado recién | Mantener, actualizar con nuevos patrones |
| XML doc comments | ❌ No existe | Documentar API pública (`/// <summary>`, `<param>`, `<returns>`) |
| License (MIT) | ❌ No tiene | Agregar `LICENSE` — necesario para que reclutadores vean que es open source serio |
| CONTRIBUTING.md | ❌ | Baja prioridad |
| CHANGELOG.md | ❌ | Baja prioridad |
| Wiki / docs del DSL | ❌ No existe | Media — una página con la sintaxis del lenguaje |
| Ejemplos `.geo` | ❌ `GeoLibrary/` vacío (gitignored) | Agregar ejemplos y quitar `*.geo` del gitignore |

**Mejoras menores de alto impacto visual en portafolio**

- **Screenshot en README** del app funcionando con figuras geométricas
- **Badges**: `dotnet build passing`, `license MIT`, `.NET 6.0`
- **Link a GitHub Pages** con WebAssembly build (Avalonia + Wasm) — demo online

---

## 3. Refactor de Código y Eliminación de Bugs

> Continuación del diagnóstico detallado en [2.2](#22-optimizaciones-bugs-y-limpieza-de-código).

### 2.1 Separar todas las clases de `Form` (⚠️ crítica)

Actualmente `Figure`, `ArchiveAnalysis`, y `Evaluator` heredan de `System.Windows.Forms.Form` **sin necesitarlo**. Esto:
- Acopla toda la lógica del dominio a la UI
- Impide testear la lógica sin un formulario
- Es un anti-patrón que un reclutador detecta al instante

**Acción**: Convertir a clases `public` planas.

```csharp
// Antes
public abstract class Figure : Form { ... }
public class ArchiveAnalysis : Form { ... }
public class Evaluator : Form { ... }

// Después
public abstract class Figure { ... }
public class ArchiveAnalysis { ... }
public class Evaluator { ... }
```

`Figure` solo necesita ser una clase base abstracta con métodos geométricos. `ArchiveAnalysis` orquesta el pipeline. `Evaluator` evalúa el AST. Ninguno necesita herencia de Form.

### 2.2 Extraer método `StoreVariable` del Evaluator

El patrón se repite **~35 veces** en `GeneralEvaluation`. Cada rama de creación de figura/seq/var replica este bloque:

```csharp
if (CurrentScope.Parent == null) {
    if (context.GlobalConstant.Keys.Contains(name)) {
        Semantic_Errors.Add(...);
    } else context.GlobalConstant.Add(name, value);
} else {
    if (CurrentScope.Variables.Keys.Contains(name) && !CurrentScope.InFunction) {
        Semantic_Errors.Add(...);
    } else if (...) { ... }
}
```

**Refactor**:

```csharp
private void StoreVariable(string name, object value) {
    if (CurrentScope.Parent == null) {
        if (context.GlobalConstant.ContainsKey(name))
            AddError("constants can't be modified");
        else
            context.GlobalConstant[name] = value;
    } else {
        if (CurrentScope.Variables.ContainsKey(name)) {
            if (!CurrentScope.InFunction)
                AddError("constants can't be modified");
            else
                CurrentScope.Variables[name] = value;
        } else {
            CurrentScope.Variables[name] = value;
        }
    }
}
```

### 2.3 Dividir `GeneralEvaluation` en métodos por tipo

**Antes**: 1,991 líneas, 60+ ramas `if/else if` en un solo método.

**Después**:

```csharp
public object GeneralEvaluation(Node node) => node.Type switch {
    NodeType.Circle       => EvaluateCircle(node),
    NodeType.Point        => EvaluatePoint(node),
    NodeType.Sum          => EvaluateSum(node),
    NodeType.Sub          => EvaluateSubtraction(node),
    NodeType.Var          => EvaluateVariable(node),
    NodeType.Declared_Fuc => EvaluateFunctionCall(node),
    NodeType.Let_exp      => EvaluateLetIn(node),
    NodeType.Count        => EvaluateCount(node),
    // ... ~30 casos más, cada uno en su propio método
    _                     => AddError("Unknown operation"), "end"
};
```

Cada `EvaluateXxx` tiene 10-40 líneas en lugar de 2,000. Esto permite:
- Testear cada operación de forma aislada
- Entender el flujo sin scroll infinito
- Añadir nuevos tipos sin tocar código existente

### 2.4 Eliminar la jerarquía fantasma `Expression`/`Binary`/`Unit`/`Ternary`

Estas clases existen en `AST/Expression/` pero **nadie las usa**. El parser, el lexer y el evaluator solo crean instancias de `Node` con `NodeType` enum.

**Opción A (recomendada)**: Eliminar las clases. Quedarse con `Node` + `NodeType` enum (como está). Simplifica.

**Opción B (ideal para portafolio)**: Migrar el AST a usar la jerarquía real — `Binary` con `Left`/`Right` tipados, `Number` con `double Value`, etc. Esto demuestra diseño OO sólido.

### 2.5 Bugs concretos

#### Bug 1: `GenerateSamples` — misma referencia de Point

**Archivo**: `Enviroment/Context.cs:66-82`

```csharp
// ANTES (bug): todas las iteraciones yield return el mismo objeto mutado
Point tem = new Point(0,0);
while (true) {
    tem.RandomPoint(points);
    points.Add(tem);
    yield return tem;
}

// DESPUÉS: nuevo objeto cada iteración
while (true) {
    Point tem = new Point(0, 0);
    tem.RandomPoint(points);
    points.Add(tem);
    yield return tem;
}
```

#### Bug 2: `ParseSum_O_Sub` — NodeExpression incorrecto

**Archivo**: `Parser/Parser.cs:918-924`

```csharp
// ANTES (bug): NodeExpression siempre "-" incluso para Sum
if (whatkind == Token.TokenType.sum) {
    sus.Type = Node.NodeType.Sum;
    sus.NodeExpression = "+";
} else sus.Type = Node.NodeType.Sub;
sus.NodeExpression = "-";  // ← se ejecuta siempre

// DESPUÉS
if (whatkind == Token.TokenType.sum) {
    sus.Type = Node.NodeType.Sum;
    sus.NodeExpression = "+";
} else {
    sus.Type = Node.NodeType.Sub;
    sus.NodeExpression = "-";
}
```

#### Bug 3: `Keys.Contains` → `ContainsKey`

Buscar y reemplazar en todo `Evaluator.cs`:
```
context.GlobalConstant.Keys.Contains(name)  →  context.GlobalConstant.ContainsKey(name)
CurrentScope.Variables.Keys.Contains(name)  →  CurrentScope.Variables.ContainsKey(name)
```

`ContainsKey` es O(1) vs O(n) de `Keys.Contains`.

### 2.6 Resource leaks en GDI+

**Archivo**: `Graphic/Form1.cs`

```csharp
// Antes: recursos no liberados
Papel = pictureBox1.CreateGraphics();
Lapiz = new Pen(Color.Black);
Brush = new SolidBrush(Color.Black);

// Después: IDisposable
// (Esto desaparece con la migración a Avalonia donde SkiaSharp maneja el ciclo de vida)
```

En la migración a Avalonia/SkiaSharp, el `SKCanvas` se obtiene por evento y se descarta automáticamente.

### 2.7 Código muerto

Eliminar/comentar código comentado:
- `Evaluator.cs`: ~70 líneas de stack overflow check comentado (~46-61, 402-404, 1736-1738, 1805)
- `Form1.cs`: ~30 líneas de Draw_Point/Draw_Segment sin parámetros (comentadas)
- `Circle.cs`: `IntersectCircleB` y `IntersectCircleL` — dos implementaciones antiguas, solo `IntersectCircle` se usa
- `Parser.cs`: comentarios como `//Revisar este point`, `//Revisar este`, `//Check this`

---

## 4. Optimizaciones de Ejecución

### 3.1 Dictionary O(1) vs O(n) lookups

**Impacto**: Medio. En archivos grandes con muchas figuras, cada evaluación de variable hace lookup. Con `Keys.Contains` cada búsqueda es O(n) sobre el número de claves.

**Solución**: Todas las búsquedas existentes usan `ContainsKey` (ver Bug 3). Adicionalmente, considerar `TryGetValue` para patrones de "contains + get":

```csharp
// Antes
if (context.GlobalConstant.ContainsKey(name))
    return context.GlobalConstant[name];

// Después
if (context.GlobalConstant.TryGetValue(name, out object? value))
    return value;
```

### 3.2 Cache de resultados de expresiones puras

El evaluador actual re-evalúa sub-expresiones idénticas cada vez que aparecen. Por ejemplo, en `draw circle(point(1,2), 5 + 3)`, la sub-expresión `5 + 3` se evalúa una vez (en el árbol la atraviesa una vez). Pero en `f(g(x)) + f(g(x))`, `f(g(x))` se evalúa dos veces completamente.

**Optimización**: Cache por nodo (hash del AST). Para expresiones sin efectos secundarios (funciones trigonométricas, aritmética, etc.), cachear el resultado:

```csharp
private Dictionary<Node, object> _cache = new();

public object GeneralEvaluation(Node node) {
    if (IsPureExpression(node) && _cache.TryGetValue(node, out object? cached))
        return cached;
    object result = Evaluate(node);
    if (IsPureExpression(node))
        _cache[node] = result;
    return result;
}

private bool IsPureExpression(Node node) => node.Type switch {
    NodeType.Sum or NodeType.Sub or NodeType.Mul or
    NodeType.Div or NodeType.Pow or NodeType.Module or
    NodeType.Number or NodeType.PI or NodeType.E or
    NodeType.Sin or NodeType.Cos or NodeType.Sqrt or NodeType.Log => true,
    _ => false
};
```

**Advertencia**: No cachear expresiones con efectos secundarios (functions calls, color changes, draw).

### 3.3 Lazy evaluation de secuencias (ya existe, no romperlo)

El sistema actual usa `yield return` en `IEnumerable<T>` para secuencias infinitas, lo que es inherentemente lazy. **No cambiar**. Solo asegurarse de que los iteradores no se materialicen innecesariamente en listas. Actualmente hay materializaciones en:

- `Finite_Sequence<Point>` en intersecciones
- `AlternativeSeq` en `GlobalSeq`
- `Count` itera toda la secuencia

**Acción**: En `Count` y `GlobalSeq`, evitar `ToList()`/`ToList()`. Usar `foreach` directo sobre el enumerable.

### 3.4 Evitar `new Random()` repetido

En casi cada método de figuras aparece `new Random()`. El constructor de `Random` usa el reloj del sistema como seed; si se crean muchos en rápida sucesión pueden tener la misma seed.

**Optimización**: Un `Random` estático/shared thread-safe:

```csharp
public static class RandomProvider {
    private static readonly ThreadLocal<Random> _random = new(() => new Random());
    public static Random Instance => _random.Value!;
}
```

Reemplazar todos los `new Random()` por `RandomProvider.Instance`.

### 3.5 Reducir boxing/unboxing

El evaluador usa `object` para todos los valores (retorno de `GeneralEvaluation` devuelve `object`). Cada operación aritmética hace:
1. Evaluar → `object` (boxing si es `double`)
2. `left is double` → unboxing check
3. `(double)left` → unboxing
4. Operación → `double`
5. Retornar como `object` (boxing)

**Optimización parcial**: Los `Binary` subclases (`Sum`, `Mult`, etc.) podrían usar genéricos:

```csharp
public class Sum {
    public object? Value { get; private set; }
    public void Evaluate(object left, object right) {
        if (left is double l && right is double r)
            Value = l + r;
        // ...
    }
}
```

El cambio completo requeriría repensar el sistema de tipos. Para portafolio inicial, el impacto es bajo (el bottleneck no está aquí).

### 3.6 Perfil: El verdadero bottleneck

El cuello de botella real en archivos `.geo` grandes será **el parseo** y la **evaluación de secuencias infinitas**. Archivos pequeños (típicos de este DSL) no mostrarán diferencia.

**Prioridad**: Baja. No optimizar prematuramente. Solo aplicar las optimizaciones O(1) dictionary y el bug de `new Random()`.

---

## 5. Migración de UI y Estética Premium

> Continuación del diagnóstico detallado en [2.3](#23-ui--interfaz-y-aspecto-visual).

### 5.1 Estrategia de migración: WinForms → Avalonia

**Por qué Avalonia y no otra**:

| Framework | Cross-platform | MVVM | Dibujo 2D | Madurez | Portafolio |
|---|---|---|---|---|---|
| **Avalonia** | ✅ Linux/macOS/Windows/Wasm | ✅ | SkiaSharp | Alta | ⭐ Excelente |
| MAUI | ⚠️ Linux limitado | ✅ | GraphicsView | Media | Buena |
| WinUI 3 | ❌ Windows-only | ✅ | Win2D | Alta | ⚠️ Windows-only |
| Blazor Hybrid | ✅ | ✅ | HTML Canvas | Alta | Diferente perfil |

**Plan de migración**:

```
Fase A: CLI First
  - Extraer toda la lógica de Form1.cs a un pipeline puro
  - Crear Wall-E.Core (class library) con Lexer/Parser/Evaluator/Figures
  - Mantener WinForms como host temporal

Fase B: Avalonia host
  - Nuevo proyecto: Wall-E.Avalonia
  - Copiar Wall-E.Core como referencia
  - Implementar ventana principal con:
    - Avalonia.Controls.TextBox (editor de comandos)
    - Avalonia.Controls.Canvas + SkiaSharp (lienzo)
    - Botones
  - NO tocar Wall-E.Core durante esta fase

Fase C: Migrar dibujo a SkiaSharp
  - Reemplazar System.Drawing con SkiaSharp en el renderer
  - Implementar ISkiaDrawable o suscribirse a PaintSurface
```

### 5.2 Arquitectura de la nueva UI

```
┌─────────────────────────────────────────────────────┐
│  Wall-E.Avalonia (UI)                               │
│  ┌──────────────┐  ┌──────────────────────────────┐ │
│  │ Editor Panel  │  │ Drawing Canvas (SkiaSharp)   │ │
│  │ ┌──────────┐ │  │  ┌─────────────────────────┐ │ │
│  │ │ Comandos  │ │  │  │ Grid + Axes + Labels    │ │ │
│  │ │ (TextBox) │ │  │  │ Figures + Colors + Tags │ │ │
│  │ └──────────┘ │  │  │ Zoom/Pan overlay         │ │ │
│  │ [Process]    │  │  └─────────────────────────┘ │ │
│  │ [Clean]      │  │                              │ │
│  │ [Jump Seq]   │  │  StatusBar: "Point p1 drawn" │ │
│  └──────────────┘  └──────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

### 5.3 Estética premium — Sistema de renderizado

#### Grid cartesiano profesional

```csharp
void DrawGrid(SKCanvas canvas, SKImageInfo info, ViewState state) {
    using var gridPaint = new SKPaint {
        Color = new SKColor(200, 200, 200, 80),
        StrokeWidth = 0.5f,
        IsAntialias = true
    };
    using var axisPaint = new SKPaint {
        Color = new SKColor(60, 60, 60),
        StrokeWidth = 1.5f,
        IsAntialias = true
    };
    using var labelPaint = new SKPaint {
        Color = new SKColor(100, 100, 100),
        TextSize = 11,
        IsAntialias = true
    };

    float gridSpacing = 50 * state.Zoom;
    PointF offset = state.Offset;

    // Líneas verticales
    for (float x = offset.X % gridSpacing; x < info.Width; x += gridSpacing) {
        canvas.DrawLine(x, 0, x, info.Height, gridPaint);
    }
    // Líneas horizontales
    for (float y = offset.Y % gridSpacing; y < info.Height; y += gridSpacing) {
        canvas.DrawLine(0, y, info.Width, y, gridPaint);
    }
    // Ejes X/Y más gruesos
    canvas.DrawLine(0, offset.Y, info.Width, offset.Y, axisPaint);
    canvas.DrawLine(offset.X, 0, offset.X, info.Height, axisPaint);
    // Labels "x", "y", números en ejes
}
```

#### Sistema de zoom y pan

```csharp
public class ViewState {
    public float Zoom { get; set; } = 1.0f;
    public float PanX { get; set; } = 0;
    public float PanY { get; set; } = 0;

    public SKPoint WorldToScreen(double x, double y, int canvasWidth, int canvasHeight) {
        return new SKPoint(
            (float)(x * Zoom) + PanX + canvasWidth / 2f,
            (float)(-y * Zoom) + PanY + canvasHeight / 2f  // Y invertido = sistema cartesiano
        );
    }
}
```

Eventos del canvas:
- **Scroll wheel**: `Zoom *= factor` (1.1x / 0.9x), centrado en cursor
- **Click + arrastrar**: `PanX/PanY += delta`
- **Doble click**: Reset Zoom/Pan

#### Anti-aliasing, suavizado y sombras

SkiaSharp da anti-aliasing por defecto. Mejoras adicionales:

```csharp
// Sombra suave para figuras seleccionadas
using var shadowPaint = new SKPaint {
    Color = new SKColor(0, 0, 0, 30),
    MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 4),
    IsAntialias = true
};
canvas.DrawCircle(centerX + 2, centerY + 2, radius, shadowPaint);

// Gradiente para círculos/arcos (relleno semitransparente)
using var fillPaint = new SKPaint {
    Shader = SKShader.CreateRadialGradient(
        center, (float)radius,
        new[] { color.WithAlpha(40), color.WithAlpha(10) },
        SKShaderTileMode.Clamp
    ),
    IsAntialias = true
};
```

#### Sistema de capas para renderizado ordenado

```csharp
void RenderFrame(SKCanvas canvas, SKImageInfo info) {
    // Capa 0: Fondo
    canvas.Clear(theme.Background);

    // Capa 1: Grid
    DrawGrid(canvas, info, state);

    // Capa 2: Figuras
    foreach (var drawable in scene.Figures)
        DrawFigure(canvas, drawable);

    // Capa 3: Selección/Hover
    if (hoveredFigure != null)
        DrawHighlight(canvas, hoveredFigure);

    // Capa 4: Medidas y decoraciones
    DrawMeasurements(canvas, scene.Measurements);

    // Capa 5: Tooltips / información
    DrawStatusInfo(canvas, info);
}
```

#### Temas claro/oscuro premium

```csharp
public class Theme {
    public static readonly Theme Light = new() {
        Background      = SKColors.WhiteSmoke,
        GridLines       = new SKColor(0, 0, 0, 30),
        GridAxis        = new SKColor(0, 0, 0, 100),
        AxisLabels      = new SKColor(80, 80, 80),
        FigureStroke    = new SKColor(30, 30, 30),
        SelectionGlow   = new SKColor(30, 144, 255, 120),
        Shadow          = new SKColor(0, 0, 0, 25),
    };

    public static readonly Theme Dark = new() {
        Background      = new SKColor(30, 30, 30),
        GridLines       = new SKColor(255, 255, 255, 20),
        GridAxis        = new SKColor(255, 255, 255, 80),
        AxisLabels      = new SKColor(180, 180, 180),
        FigureStroke    = new SKColor(220, 220, 220),
        SelectionGlow   = new SKColor(30, 144, 255, 100),
        Shadow          = new SKColor(0, 0, 0, 60),
    };
}
```

### 5.4 Funcionalidades premium de UI

| Funcionalidad | Implementación | Impacto visual |
|---|---|---|
| **Syntax highlighting** | `AvaloniaEdit` (control editor) o resaltado manual con `FormattedText` de Skia | ⭐⭐⭐ |
| **Selector de color visual** | `ColorPicker` de Avalonia + preview en canvas | ⭐⭐⭐ |
| **Exportar PNG** | `canvas.Snapshot()` → `SKImage.Encode(SKEncodedImageFormat.Png, 100)` → `File.WriteAllBytes` | ⭐⭐⭐ |
| **Exportar SVG** | SkiaSharp SVG canvas o `SvgDocument` manual | ⭐⭐ |
| **Animación de dibujo** | Las figuras aparecen secuencialmente con `await Task.Delay(200)` entre cada una | ⭐⭐⭐ |
| **Hover highlight** | `PointerMoved` → detectar figura bajo cursor → glow | ⭐⭐ |
| **Snap to grid** | Redondear coordenadas a múltiplo del grid spacing | ⭐ |
| **Coordenadas en vivo** | StatusBar muestra coordenadas del cursor en el sistema cartesiano | ⭐ |
| **Toolbar con iconos** | Botones con iconos SVG (Process, Clean, Zoom In/Out, Export, Theme toggle) | ⭐⭐ |

### 5.5 Esquema de color para el DSL (syntax highlighting)

```csharp
public class DslHighlighting {
    static readonly Dictionary<string, SKColor> Keywords = new() {
        ["point"]    = SKColors.DodgerBlue,
        ["line"]     = SKColors.DodgerBlue,
        ["circle"]   = SKColors.DodgerBlue,
        ["segment"]  = SKColors.DodgerBlue,
        ["ray"]      = SKColors.DodgerBlue,
        ["arc"]      = SKColors.DodgerBlue,
        ["draw"]     = SKColors.MediumSeaGreen,
        ["color"]    = SKColors.OrangeRed,
        ["restore"]  = SKColors.OrangeRed,
        ["let"]      = SKColors.MediumPurple,
        ["in"]       = SKColors.MediumPurple,
        ["if"]       = SKColors.MediumPurple,
        ["then"]     = SKColors.MediumPurple,
        ["else"]     = SKColors.MediumPurple,
        ["import"]   = SKColors.SteelBlue,
        ["sequence"] = SKColors.Coral,
        ["PI"]       = SKColors.Goldenrod,
        ["E"]        = SKColors.Goldenrod,
        ["sin"]      = SKColors.Teal,
        ["cos"]      = SKColors.Teal,
        ["sqrt"]     = SKColors.Teal,
        ["log"]      = SKColors.Teal,
        ["not"]      = SKColors.IndianRed,
        ["and"]      = SKColors.IndianRed,
        ["or"]       = SKColors.IndianRed,
        ["randoms"]  = SKColors.SlateBlue,
        ["samples"]  = SKColors.SlateBlue,
        ["points"]   = SKColors.SlateBlue,
        ["count"]    = SKColors.SlateBlue,
        ["intersect"]= SKColors.SlateBlue,
        ["measure"]  = SKColors.SlateBlue,
    };

    static readonly SKColor NumberColor  = new(0, 150, 136);  // Teal
    static readonly SKColor StringColor  = new(200, 100, 50); // Burnt Orange
    static readonly SKColor CommentColor = new(100, 100, 100); // Gray
    static readonly SKColor DefaultColor = SKColors.Black;
}
```

---

## 6. Prácticas Profesionales

> Continuación del diagnóstico detallado en [2.4](#24-prácticas-profesionales-y-de-trabajo).

### 6.1 Tests (Prioridad #1 absoluta)

**Framework**: xUnit + `Shouldly` (assertions legibles).

**Proyecto**: `Wall-E.Core.Tests.csproj` (target: `net6.0`, sin Windows dependency).

**Plan de tests**:

```
tests/
  LexerTests.cs           # Tokenización correcta de cada token type
  ParserTests.cs          # AST correcto para cada statement
  EvaluatorTests.cs       # Evaluación de cada operación
  FigureTests.cs          # Intersecciones, Contains, distances
  SequenceTests.cs        # Finite, infinite, enclosed, concatenation
  IntegrationTests.cs     # Archivos .geo completos
```

**Tests mínimos para impacto en portafolio**:

```csharp
public class EvaluatorTests {
    [Fact]
    public void Sum_of_two_numbers_returns_correct_result() {
        var code = "2 + 3;";
        var context = Execute(code);
        context.Results.ShouldContain(5.0);
    }

    [Fact]
    public void Point_creation_adds_to_existing_points() {
        var code = "point p1;";
        var context = Execute(code);
        context.ExistingPoints.ShouldHaveSingleItem();
    }

    [Fact]
    public void Division_by_zero_returns_error() {
        var code = "5 / 0;";
        var context = Execute(code);
        context.issuedcontext.ShouldBeTrue();
    }
}
```

### 6.2 CI/CD con GitHub Actions

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
    runs-on: windows-latest  # necesario para tests de figuras
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '6.0' }
      - run: dotnet test -c Release --verbosity normal
```

### 6.3 Estructura de proyecto profesional

```
WALL-E/
  Wall-E.sln
  src/
    Wall-E.Domain/              # 0 dependencias externas
      AST/                      # Node, NodeType, INodeVisitor<T>
      Figures/                  # Point, Line, Circle, etc.
      Evaluation/               # EvaluationContext, EvaluationResult, IEvaluator
      Geometry/                 # Intersections, measures
      Wall-E.Domain.csproj      # net6.0

    Wall-E.Application/         # → Domain
      Interfaces/               # ILexer, IParser, IPipeline, ISceneBuilder
      DSL/                      # Lexer, Parser, GeneralLexer, GeneralParser
      Pipeline/                 # PipelineOrchestrator, Scene, RenderCommand
      Caching/                  # ExpressionCache
      Wall-E.Application.csproj # net6.0

    Wall-E.Infrastructure/      # → Application
      FileSystem/               # GeoLibrary loader
      Wall-E.Infrastructure.csproj

    Wall-E.UI.Avalonia/         # → Application + Infrastructure
      ViewModels/               # MainViewModel, CanvasViewModel, etc.
      Views/                    # MainWindow.axaml, EditorView, CanvasView
      Rendering/                # SkiaRenderer, StreamRenderer, GridRenderer
      Theme.cs, PaintPool.cs
      Wall-E.UI.Avalonia.csproj

  tests/
    Wall-E.Domain.Tests/
    Wall-E.Application.Tests/
    Wall-E.UI.Tests/

  GeoLibrary/
  docs/
  README.md
  AGENTS.md
  IMPROVEMENT_PLAN.md
  LICENSE
```

### 6.4 Git workflow

| Práctica | Acción |
|---|---|
| **Conventional Commits** | `feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:` |
| **Ramas** | `main` (estable), `develop`, `feat/avalon-ui`, `fix/evaluator-bugs` |
| **PRs** | Template con checklist: tests, build, screenshot si aplica |
| **Tags** | `v1.0.0`, `v1.1.0` |

### 6.5 Code Quality

```bash
# Agregar analyzers
dotnet add package Roslynator.Analyzers
dotnet add package SonarAnalyzer.CSharp

# .editorconfig (crear desde template de dotnet)
dotnet new editorconfig
```

Reglas de .editorconfig a personalizar:

```ini
# Wall-E conventions
dotnet_naming_rule.private_fields_should_be_camelCase.style = camel_case
csharp_style_var_for_built_in_types = true:warning
csharp_style_pattern_matching_over_is_with_cast_check = true:warning
csharp_style_throw_expression = true:warning
```

---

## 7. Documentación y Otros

> Continuación del diagnóstico detallado en [2.5](#25-documentación-y-otros).

### 7.1 README.md profesional (inglés)

```markdown
# GeoWall-E

![Build](https://img.shields.io/github/actions/workflow/status/...)
![License](https://img.shields.io/github/license/...)
![.NET](https://img.shields.io/badge/.NET-6.0-512BD4)

A geometric drawing interpreter. Write geometric constructions in
a simple DSL and render them on an interactive canvas.

## Quick Start

```bash
git clone https://github.com/your-username/WALL-E.git
cd WALL-E
./Wall-E.sh
```

## DSL Example

```
point p1;
point p2;
line l(p1, p2);
draw l;
```

## Features

- Points, Lines, Segments, Rays, Circles, Arcs
- Arithmetic, boolean, and trigonometric operations
- User-defined functions
- Let-in expressions, conditionals
- Infinite and finite sequences
- Color management
- Import system for modular files
- Interactive canvas with zoom, pan, and grid

## Architecture

[diagram of pipeline]

## License

MIT
```

### 7.2 Screenshots de alta calidad

- Demo con varias figuras geométricas coloreadas
- Grid visible, etiquetas, ejes
- Tema oscuro
- Syntax highlighting en el editor

### 7.3 Licencia MIT

Agregar `LICENSE` (texto estándar MIT). Sin licencia el proyecto no es "open source" legalmente — un reclutador que quiera reusar código no puede.

### 7.4 Archivos `.geo` de ejemplo

Crear en `GeoLibrary/`:

```
# basics.geo
point p1;
line l(p1, point(5, 5));
draw l "my line";

# fractals.geo
function snowflake(p1, p2) = ...
```

### 7.5 Demo online (bonus)

Avalonia soporta WebAssembly. Publicar en GitHub Pages:

```bash
dotnet publish -c Release -o deploy -r browser-wasm
# subir deploy/ a gh-pages branch
```

---

## Checklist de prioridad para portafolio

```
[ ] Tests (xUnit)
[ ] Separar clases de Form
[ ] Extraer StoreVariable
[ ] Bug GenerateSamples
[ ] Bug ParseSum_O_Sub
[ ] Migrar a Avalonia
[ ] Grid + Zoom + Pan
[ ] Exportar PNG
[ ] README en inglés
[ ] LICENSE (MIT)
[ ] CI (GitHub Actions)
[ ] .editorconfig
[ ] Syntax highlighting
[ ] Temas claro/oscuro
[ ] Screenshots
[ ] Ejemplos .geo
```

---

*Documento generado el 2026-07-01. Prioridades basadas en análisis de ~5,600 líneas de código fuente.*
