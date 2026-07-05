# WALL-E — Roadmap de Implementación (v2 — Arquitectura Limpia)

Plan unificado que integra todos los documentos de mejora:
[`IMPROVEMENT_PLAN.md`](./IMPROVEMENT_PLAN.md) · [`PERFORMANCE_PLAN.md`](./PERFORMANCE_PLAN.md) · [`ENHANCEMENTS.md`](./ENHANCEMENTS.md)

---

## Timeline consolidado

```
Semana  1  2  3  4  5  6  7  8  9  10  11  12  13  14  15  16  17  18
Fase 0  ████████
Fase 1          ██████████████████
Fase 2                  ████████████████
Fase 3                          ██████████████████
Fase 4                                  ████████████████
Fase 5                                          ████████████
Fase 6                                                  ████████
```

**Total estimado**: ~18 semanas (~4.5 meses) a tiempo completo.

---

## Refinamiento arquitectónico (cambio clave v1→v2)

### Problemas identificados en v1

| Problema | Impacto |
|---|---|
| `Context` es god object (mezcla dominio + UI + resultados) | Viola SRP, difícil de testear |
| `object` como tipo de retorno en toda la evaluación | Type-unsafe, casts frágiles |
| Sin capas Domain/Application/Infrastructure | No hay separación de concerns |
| Sin MVVM real (solo code-behind) | Anti-patrón WinForms en Avalonia |
| Evaluador conoce Channel de infraestructura | Acoplamiento evaluación→streaming |
| State machine compleja (~40 casos en switch) | Menos extensible que Visitor Pattern |
| Sin sistema de Resultados monádico | Errores como strings, no composables |

### Arquitectura objetivo (Clean Architecture / Hexagonal)

```
┌──────────────────────────────────────────────────────────┐
│                    Wall-E.UI.Avalonia                      │
│  (MVVM: Views ↔ ViewModels, SkiaSharp rendering)          │
│  Depende de: Wall-E.Application                            │
├──────────────────────────────────────────────────────────┤
│                   Wall-E.Application                       │
│  (Orquestación: Pipeline, Caching, DSL, Import/Export)    │
│  Depende de: Wall-E.Domain                                 │
├──────────────────────────────────────────────────────────┤
│                   Wall-E.Domain                            │
│  (Lógica pura: AST, Figuras, Evaluación, Geometría)       │
│  Depende de: NADA (net6.0)                                 │
├──────────────────────────────────────────────────────────┤
│                 Wall-E.Infrastructure                      │
│  (IO: FileSystem, GeoLibrary, Persistencia)               │
│  Depende de: Wall-E.Application                            │
└──────────────────────────────────────────────────────────┘
```

### Patrones clave

| Patrón | Dónde | Beneficio |
|---|---|---|
| `INodeVisitor<T>` | Evaluación del AST | Cada operación en su método, extensible sin modificar existentes |
| `IEvaluationResult` sellado | Retorno del evaluador | Union type: `FigureValue`, `NumberValue`, `SequenceValue`, `ErrorValue` — sin `object` |
| `IProgress<Scene>` | Pipeline evaluación→render | Evaluador produce `Scene` immutable, no conoce Channel |
| MVVM puro | UI Avalonia | ViewModels sin referencia a Views, data binding, testable |
| `Result<T, E>` | Errores del pipeline | Monádico, composable, evita try/catch dispersos |
| `ISceneBuilder` | Construcción de escena | Separa lógica de dibujo de la evaluación |

---

## Fase 0: Foundation & Critical Fixes

**Duración**: ~1.5 semanas · **Estado**: ✅ COMPLETADA

| # | Tarea | Archivos |
|---|---|---|
| 0.1-0.4 | Pipeline async + CancellationToken + volatile | `Form1.cs`, `Evaluator.cs`, `ArchiveAnallizer.cs`, `GeneralEvaluator.cs` |
| 0.5-0.7 | MaxElements (10k) + TakenSequence + Safe concat | `Sequence.cs`, `Sum.cs`, `Sequence Concatenation.cs` |
| 0.8-0.10 | Bugs: GenerateSamples, ParseSum_O_Sub, ContainsKey | `Context.cs`, `Parser.cs`, `Evaluator.cs` |
| 0.11 | RandomProvider thread-safe | `RandomProvider.cs` (nuevo) |
| 0.12-0.13 | Context.Clear() + TryAdd limits | `Context.cs` |
| 0.14-0.15 | Dead code removal + GDI+ Dispose | `Evaluator.cs`, `Form1.cs`, `Parser.cs`, etc. |

**Checkpoint**: App no congela. Stop button funciona. UI responsiva.

---

## Fase 1: Clean Architecture Extraction

**Duración**: ~3 semanas · **Objetivo**: Separar en 4 capas limpias. Domain sin dependencias. Application con interfaces.

| # | Tarea | Días | Archivos |
|---|---|---|---|
| 1.1 | **Crear `Wall-E.Domain`** (class library net6.0, 0 dependencias externas) | 0.5 | `Wall-E.Domain.csproj` (nuevo) |
| 1.2 | Mover a Domain: `AST/` (Node, NodeType), `AST/Geometric Objects/` (Figure, Point, Circle, etc.) | 1 | mover archivos, actualizar namespaces |
| 1.3 | Mover a Domain: `Evaluation/` (Evaluator, Scope, Context → split), `Enviroment/Error.cs` | 1 | mover + refactor |
| 1.4 | **Split `Context` en 3 responsabilidades** | 2 | `Wall-E.Domain` |
| | `EvaluationContext` — symbol table, scope stack, functions | | `EvaluationContext.cs` (nuevo) |
| | `FigureRepository` — ExistingPoints, Circles, Lines (solo figuras, sin UI) | | `FigureRepository.cs` (nuevo) |
| | `RenderScene` — ToDraw + Colors (resultado plano para render) | | `RenderScene.cs` (nuevo) |
| 1.5 | **`IEvaluationResult` sellado** — reemplazar `object` return | 2 | `EvaluationResult.cs` (nuevo), `Evaluator.cs` |
| | `NumberResult(double)`, `PointResult(Point)`, `FigureResult(Figure)`, `SequenceResult(IEnumerable)`, `StringResult(string)`, `ErrorResult(Error)` | | |
| 1.6 | **`INodeVisitor<T>` + visitor concreto** — reemplazar switch monolítico | 3 | `INodeVisitor.cs`, `EvaluatorVisitor.cs` (nuevos) |
| 1.7 | **Crear `Wall-E.Application`** (net6.0, ref a Domain) | 0.5 | `Wall-E.Application.csproj` (nuevo) |
| 1.8 | Mover a Application: `Lexer/`, `Parser/`, `DSL/` (procesamiento texto→AST) | 1 | mover archivos |
| 1.9 | Mover a Application: `Pipeline/PipelineOrchestrator.cs`, `Caching/ExpressionCache.cs` | 1 | mover + refactor |
| 1.10 | **Interfaces** `ILexer`, `IParser`, `IEvaluator`, `IPipeline` | 1 | `Wall-E.Application/Interfaces/` |
| 1.11 | **Crear `Wall-E.Infrastructure`** (net6.0, ref a Application) | 0.3 | `Wall-E.Infrastructure.csproj` (nuevo) |
| 1.12 | Mover a Infrastructure: `GeoLibrary` loader, file I/O | 0.5 | mover |
| 1.13 | **`Result<T, E>` monádico** para el pipeline completo | 2 | `Result.cs` (nuevo) |
| 1.14 | Solución `Wall-E.sln` con estructura `src/` profesional | 0.3 | `Wall-E.sln` (nuevo) |
| 1.15 | Build en Linux: `dotnet build` pasa en Domain + Application + Infrastructure | 0.5 | csproj adjustments |

**Checkpoint**: `dotnet build` pasa en Linux. Domain no referencia Windows. Evaluator usa visitor pattern. Return type es `IEvaluationResult` sellado. Context dividido en 3.

### Código clave: `IEvaluationResult`

```csharp
public abstract record EvaluationResult;

public sealed record NumberResult(double Value) : EvaluationResult;
public sealed record PointResult(Point Value) : EvaluationResult;
public sealed record FigureResult(Figure Value) : EvaluationResult;
public sealed record SequenceResult(IEnumerable Value, long Count) : EvaluationResult;
public sealed record StringResult(string Value) : EvaluationResult;
public sealed record ErrorResult(Error Value) : EvaluationResult;
public sealed record VoidResult : EvaluationResult;
```

### Código clave: `INodeVisitor<T>`

```csharp
public interface INodeVisitor<T>
{
    T VisitCircle(Node node);
    T VisitPoint(Node node);
    T VisitLine(Node node);
    T VisitSegment(Node node);
    T VisitRay(Node node);
    T VisitArc(Node node);
    T VisitSum(Node node);
    T VisitSub(Node node);
    T VisitNumber(Node node);
    T VisitVariable(Node node);
    T VisitFunctionCall(Node node);
    // ... uno por cada NodeType (~40 métodos)
}

public class EvaluatorVisitor : INodeVisitor<EvaluationResult>
{
    private readonly EvaluationContext _context;
    private readonly FigureRepository _figures;
    private readonly ExpressionCache _cache;

    public EvaluationResult VisitCircle(Node node) { /* 10-30 líneas */ }
    public EvaluationResult VisitSum(Node node) { /* 10-30 líneas */ }
    // cada método es pequeño, testeable aisladamente
}
```

### Código clave: `Result<T, E>`

```csharp
public readonly struct Result<T, E>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public E Error { get; }

    public static Result<T, E> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T, E> Fail(E error) => new() { IsSuccess = false, Error = error };

    public Result<TNext, E> Map<TNext>(Func<T, TNext> map) =>
        IsSuccess ? Result<TNext, E>.Ok(map(Value)) : Result<TNext, E>.Fail(Error);

    public Result<TNext, E> Bind<TNext>(Func<T, Result<TNext, E>> bind) =>
        IsSuccess ? bind(Value) : Result<TNext, E>.Fail(Error);
}
```

---

## Fase 2: Professional Avalonia UI + MVVM

**Duración**: ~3 semanas · **Objetivo**: UI moderna con MVVM puro, streaming progresivo, grid.

| # | Tarea | Días | Archivos |
|---|---|---|---|
| 2.1 | **Crear `Wall-E.UI.Avalonia`** (net6.0, ref a Application + Infrastructure) | 0.5 | `Wall-E.UI.Avalonia.csproj` (nuevo) |
| 2.2 | **MVVM Base**: `ViewModelBase`, `RelayCommand`, `INotifyPropertyChanged` | 1 | `ViewModels/` base |
| 2.3 | **`MainViewModel`**: Properties: `Code` (string), `IsProcessing` (bool), `StatusMessage` (string), `CurrentTheme`. Commands: `ProcessCommand`, `StopCommand`, `ClearCommand`, `ToggleThemeCommand` | 1.5 | `MainViewModel.cs` (nuevo) |
| 2.4 | **`CanvasViewModel`**: Zoom, Pan, cursor coordinates, hovered figure. Binding a canvas events | 1.5 | `CanvasViewModel.cs` (nuevo) |
| 2.5 | **`StatusBarViewModel`**: Processing status, figure count, cursor position | 0.5 | `StatusBarViewModel.cs` (nuevo) |
| 2.6 | **MainWindow.axaml**: Layout: editor panel (izquierda) + canvas (derecha) + toolbar + statusbar. Pure XAML + binding | 2 | `MainWindow.axaml` |
| 2.7 | **`ProcessCommand`**: async relay command → `Task.Run(() => pipeline.Execute(code, ct))` → `Scene` result → bind to canvas | 1 | `MainViewModel.cs` |
| 2.8 | **`IProgress<Scene>` pipeline**: evaluador produce `Scene` (snapshot inmutable de figures + colors). Channel vive en Application | 1 | `Scene.cs`, `PipelineOrchestrator.cs` |
| 2.9 | **Streaming progresivo**: `Channel<Scene>` con batch cada 100 figures → `await Task.Yield()` → canvas.Invalidate() | 1.5 | `StreamRenderer.cs` (nuevo) |
| 2.10 | **SkiaSharp canvas**: `SKCanvasView` con suscripción a `PaintSurface` | 1 | `CanvasView.axaml.cs` |
| 2.11 | **Grid cartesiano** con labels en ejes X/Y + anti-aliasing | 1 | `GridRenderer.cs` (nuevo) |
| 2.12 | **Zoom** (scroll wheel, centrado en cursor) + **Pan** (click+arrastrar) + reset doble-click | 2 | `ViewState.cs`, eventos en canvas |
| 2.13 | **Sistema de coordenadas**: origen configurable + escala + Y invertido (cartesiano) | 1 | `CoordinateSystem.cs` (nuevo) |
| 2.14 | **Color picker visual** (Avalonia `ColorPicker`) en toolbar | 1 | toolbar + binding |
| 2.15 | **StatusBar**: coordenadas del cursor en vivo, conteo de figuras | 0.5 | binding a CanvasViewModel |

**Checkpoint**: App Avalonia con MVVM puro corriendo. Process/Stop/Clean via Commands. Grid + zoom + pan. Streaming progresivo.

### Código clave: MVVM puro

```csharp
public class MainViewModel : ViewModelBase
{
    public MainViewModel(IPipeline pipeline, IRenderer renderer)
    {
        ProcessCommand = new RelayCommand(async () =>
        {
            IsProcessing = true;
            var scene = await pipeline.ExecuteAsync(Code, _cts.Token);
            await renderer.RenderAsync(scene, _cts.Token);
            IsProcessing = false;
        }, () => !IsProcessing);
    }

    public string Code { get => _code; set => SetProperty(ref _code, value); }
    public bool IsProcessing { get => _isProcessing; set { SetProperty(ref _isProcessing, value); ProcessCommand.RaiseCanExecuteChanged(); } }
    public IRelayCommand ProcessCommand { get; }
    // → Sin code-behind. Testing: new MainViewModel(mockPipeline, mockRenderer).ProcessCommand.Execute().
}
```

### Arquitectura de ventana Avalonia

```
┌────────────────────────────────────────────────────────┐
│  MainWindow.axaml                                       │
│  ┌──────────────────────┐  ┌─────────────────────────┐ │
│  │  Editor Panel         │  │  SKCanvasView           │ │
│  │  ┌──────────────────┐│  │  ┌───────────────────┐  │ │
│  │  │ TextBox (Code)    ││  │  │ Grid + Axes       │  │ │
│  │  │ (bind: MainVM    ││  │  │ Figures (Scene)    │  │ │
│  │  │  .Code)          ││  │  │ Zoom/Pan overlay   │  │ │
│  │  └──────────────────┘│  │  └───────────────────┘  │ │
│  │  [Process] [Stop]    │  │  (bind: CanvasVM)       │ │
│  │  [Clean] [🌙 Toggle] │  └─────────────────────────┘ │
│  │  [🎨 ColorPicker]    │                               │
│  └──────────────────────┘  StatusBar: "120 figures"    │
└────────────────────────────────────────────────────────┘
```

---

## Fase 3: GPU Rendering + Expression Cache + Estética Premium

**Duración**: ~2.5 semanas · **Objetivo**: Rendimiento GPU, look profesional, cache.

| # | Tarea | Días | Archivos |
|---|---|---|---|
| 3.1 | **Expression Cache** en Application (hash del AST, invalidación en writes) | 1.5 | `ExpressionCache.cs` |
| 3.2 | **PaintPool**: `SKPaint` reutilizados por color (evita alloc por frame) | 0.5 | `PaintPool.cs` (nuevo) |
| 3.3 | **Batch GPU**: `SKCanvas.DrawPoints(SKPointMode.Points, array)` para puntos | 1 | `SkiaRenderer.cs` |
| 3.4 | `SKPath` para figuras (DrawCircle, DrawLine, DrawArc nativos Skia) | 1 | `SkiaRenderer.cs` |
| 3.5 | Anti-aliasing + sombras suaves + glows en figuras | 1 | `SkiaRenderer.cs` |
| 3.6 | **Temas claro/oscuro** premium con paleta profesional | 1.5 | `Theme.cs`, toggle command |
| 3.7 | **Sistema de capas** (z-order): `layer N; draw ...;` | 1.5 | `RenderScene.cs`, `SkiaRenderer.cs` |
| 3.8 | **Selector de color** visual integrado en toolbar | 1 | toolbar |
| 3.9 | **Snap a grid** configurable: `snap 0.5;` | 0.5 | `CoordinateSystem.cs` |
| 3.10 | **Estilos de línea**: dashed, dotted, dash-dot, grosor | 1.5 | `Lexer.cs`, `Parser.cs`, `SkiaRenderer.cs` |
| 3.11 | **Relleno de figuras**: solid, gradient (linear/radial), none | 1.5 | `DrawObject.cs`, `SkiaRenderer.cs` |
| 3.12 | **Ocultar/mostrar figuras**: `hide l;` / `show l;` | 0.5 | `RenderScene.cs` |
| 3.13 | **Exportar PNG**: `canvas.Snapshot()` → `SKImage.Encode(Png, 100)` → `File.WriteAllBytes` | 0.5 | `ExportService.cs` |

**Checkpoint**: 100k puntos en < 0.5s. Cache acelera fractales recursivos. Temas, capas, estilos, relleno. Export PNG.

---

## Fase 4: Language Enhancements

**Duración**: ~3 semanas · **Objetivo**: DSL rico — colores completos, figuras nuevas, bucles, animación.

| # | Tarea | Días | Archivos |
|---|---|---|---|
| 4.1 | Color `#hex` (16M colores) | 1 | Lexer/Parser/Evaluator |
| 4.2 | Color `rgb(r,g,b)` + `rgba(r,g,b,a)` | 1 | Lexer/Parser/Evaluator |
| 4.3 | Color `hsl(h,s%,l%)` | 1 | Lexer/Parser/Evaluator |
| 4.4 | Colores CSS completos (~140 nombres) | 0.5 | `ColorTable.cs` |
| 4.5 | Gradientes en fill: `fill linear(red, blue)` / `fill radial(red, blue)` | 1.5 | Lexer/Parser/Evaluator + Skia |
| 4.6 | Operaciones cromáticas: `lighten`, `darken`, `mix`, `complement` | 1.5 | Lexer/Parser/Evaluator |
| 4.7 | Comentarios en DSL (`#` / `//`) | 0.3 | `Lexer.cs` |
| 4.8 | `seed(n)` para random determinista | 0.3 | `RandomProvider.cs` |
| 4.9 | `print` / `debug` para output en statusbar | 1 | Lexer/Parser/Evaluator + UI |
| 4.10 | **Polígono regular**: `polygon(center, radius, n)` | 2 | `Polygon.cs`, Parser, Evaluator, Renderer |
| 4.11 | **Elipse**: `ellipse(center, rx, ry)` | 1.5 | `Ellipse.cs`, Parser, Evaluator, Renderer |
| 4.12 | **Texto/etiquetas**: `label(point, "text", size=14)` | 1.5 | `LabelFigure.cs`, Parser, Evaluator, Renderer |
| 4.13 | **Bucles**: `for i in seq { ... }`, `repeat(n) { ... }` | 3 | AST, Parser, Visitor |
| 4.14 | Constantes extra (`phi`, `sqrt2`) + funciones math (`tan`, `atan`, `abs`, `floor`, `ceil`) | 0.5 | `Context.cs` |
| 4.15 | **Animación paramétrica**: `animate(t from 0 to 2*PI) { ... }` | 2.5 | AST, Parser, Visitor, StreamRenderer |

**Checkpoint**: DSL completo. Colores ilimitados. Polígono, elipse, texto, loops, animación.

---

## Fase 5: Professional Polish (Tests + CI)

**Duración**: ~2.5 semanas · **Objetivo**: Tests, CI, calidad de código profesional.

| # | Tarea | Días | Archivos |
|---|---|---|---|
| 5.1 | Proyecto `Wall-E.Domain.Tests` + `Wall-E.Application.Tests` con xUnit | 1 | csprojs nuevos |
| 5.2 | Tests de lexer: cada token type | 1 | `LexerTests.cs` |
| 5.3 | Tests de parser: AST correcto para cada statement | 1 | `ParserTests.cs` |
| 5.4 | Tests de evaluator: cada operación vía visitor | 3 | `EvaluatorTests.cs` |
| 5.5 | Tests de secuencias: finite, infinite (limitado), concat, take, count | 1.5 | `SequenceTests.cs` |
| 5.6 | Tests de intersección: cada combinación de figuras | 2 | `IntersectionTests.cs` |
| 5.7 | Tests de integración: archivos `.geo` completos | 1 | `IntegrationTests.cs` |
| 5.8 | Tests de ViewModel: ProcessCommand, Theme toggle, etc. | 1.5 | `MainViewModelTests.cs` |
| 5.9 | GitHub Actions CI: build ubuntu + test windows-latest | 1 | `.github/workflows/ci.yml` |
| 5.10 | `.editorconfig` con reglas del proyecto | 0.3 | `.editorconfig` |
| 5.11 | Roslynator + SonarAnalyzer configurados | 0.5 | `Directory.Build.props` |
| 5.12 | XML doc comments en API pública de `Wall-E.Domain` + `Application` | 1 | todos los archivos |

**Checkpoint**: `dotnet test` pasa en CI. Cobertura > 40%. Análisis estático en cada PR.

---

## Fase 6: Portfolio Finalization

**Duración**: ~1.5 semanas · **Objetivo**: Presentación profesional.

| # | Tarea | Días | Archivos |
|---|---|---|---|
| 6.1 | README.md en inglés con badges + ejemplos + screenshot | 1 | `README.md` |
| 6.2 | Screenshots de alta calidad: demo, grid, dark theme | 1 | `docs/screenshots/` |
| 6.3 | Archivos `.geo` de ejemplo (demo, fractal, samples) | 1 | `GeoLibrary/` |
| 6.4 | LICENSE (MIT) | 0.1 | `LICENSE` |
| 6.5 | CLI tool: `Wall-E.Cli` (headless, input.geo → output.png) | 2 | `Wall-E.Cli.csproj` |
| 6.6 | Exportar SVG desde la escena | 2 | `SvgExporter.cs` |
| 6.7 | Demo online: publicar Avalonia Wasm en GitHub Pages | 1.5 | `docs/` + GH Pages |
| 6.8 | Syntax highlighting en editor DSL (AvaloniaEdit o Skia custom) | 2 | `DslHighlighting.cs` |

**Checkpoint**: README profesional, ejemplos, CLI, demo online, LICENSE.

---

## Estructura final del proyecto

```
WALL-E/
  Wall-E.sln
  src/
    Wall-E.Domain/
      AST/
        Node.cs, NodeType.cs, INodeVisitor.cs, INodeVisitorExtensions.cs
      Figures/
        Figure.cs (abstract), Point.cs, Line.cs, Circle.cs,
        Segment.cs, Ray.cs, Arc.cs, Polygon.cs, Ellipse.cs, LabelFigure.cs
      Evaluation/
        EvaluationContext.cs, Scope.cs
        EvaluationResult.cs, ErrorResult.cs
        IEvaluator.cs
      Geometry/
        IntersectionHelper.cs, MeasureHelper.cs
      Wall-E.Domain.csproj (net6.0, 0 dependencies)

    Wall-E.Application/
      Interfaces/
        ILexer.cs, IParser.cs, IPipeline.cs, ISceneBuilder.cs
      DSL/
        Lexer/  (Lexer.cs, Token.cs, TokenStream.cs, GeneralLexer.cs)
        Parser/ (Parser.cs, GeneralParser.cs)
      Pipeline/
        PipelineOrchestrator.cs, Scene.cs, RenderCommand.cs
      Caching/
        ExpressionCache.cs
      Services/
        GeoImporter.cs, ExportService.cs
      Wall-E.Application.csproj (net6.0, → Domain)

    Wall-E.Infrastructure/
      FileSystem/
        GeoLibraryLoader.cs
      Wall-E.Infrastructure.csproj (net6.0, → Application)

    Wall-E.UI.Avalonia/
      ViewModels/
        ViewModelBase.cs, RelayCommand.cs
        MainViewModel.cs, CanvasViewModel.cs
        StatusBarViewModel.cs, EditorViewModel.cs
      Views/
        MainWindow.axaml + .cs
        EditorView.axaml, CanvasView.axaml
      Rendering/
        SkiaRenderer.cs, StreamRenderer.cs
        GridRenderer.cs, SceneBuilder.cs
        PaintPool.cs, ViewState.cs
        CoordinateSystem.cs
      Theme.cs, ExportService.cs
      Wall-E.UI.Avalonia.csproj (net6.0, → Application + Infrastructure)

  tests/
    Wall-E.Domain.Tests/
      EvaluatorTests.cs, FigureTests.cs
      SequenceTests.cs, IntersectionTests.cs
    Wall-E.Application.Tests/
      LexerTests.cs, ParserTests.cs
      PipelineTests.cs, IntegrationTests.cs
    Wall-E.UI.Tests/
      MainViewModelTests.cs

  GeoLibrary/    (demo.geo, fractal.geo, samples.geo)
  .github/workflows/ci.yml
  .editorconfig
  README.md, LICENSE, AGENTS.md
  IMPROVEMENT_PLAN.md, PERFORMANCE_PLAN.md, ENHANCEMENTS.md, ROADMAP.md
```

---

## Lo que NO entra en este plan (sin cambios vs v1)

| Mejora | Motivo |
|---|---|
| Undo/redo en comandos | ~5d, impacto medio |
| Arrastrar puntos interactivamente | Hit testing complejo |
| Auto-completado del DSL | Desvío del core |
| Evaluación incremental (diff AST) | Mejora marginal vs streaming |
| Import/export DXF / GeoJSON | Nicho, esfuerzo alto |
| Macros en el DSL | Complejo, poca visibilidad |
| Tuplas, mapas, records en DSL | Cambio profundo (~8d) |
| Break/continue en loops | Baja prioridad vs loops básicos |
