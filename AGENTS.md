# WALL-E — AGENTS.md

## Project

GeoWall-E: geometric drawing interpreter.  
Target architecture: Clean Architecture (4 layers).  
Target framework: `net6.0` (cross-platform after Fase 1).

## Commands

```bash
dotnet build src/Wall-E.Domain/Wall-E.Domain.csproj       # build domain (Linux OK)
dotnet build src/Wall-E.Application/Wall-E.Application.csproj   # build application
dotnet build src/Wall-E.Infrastructure/Wall-E.Infrastructure.csproj # build infrastructure
dotnet build src/Wall-E.UI.Avalonia/Wall-E.UI.Avalonia.csproj    # build UI (Windows)
dotnet build src/Wall-E.sln                                # build new architecture
./Wall-E.sh                                                # build + run (legacy WinForms)
dotnet clean                                               # clean
```

Until Fase 1 is complete, the legacy build still works:
```bash
dotnet build Wall-E.csproj
dotnet run --project Wall-E.csproj
```

## Commit discipline (hard rule)

After every meaningful set of changes — a completed task, a fixed bug, a refactor, a new feature — **create and push a commit immediately**. Do not batch unrelated changes into one commit. Do not leave uncommitted work overnight. Each commit message must follow conventional commits format:

```
feat: add hex color support (#rrggbb)
fix: GenerateSamples now creates unique Point instances
refactor: extract StoreVariable, eliminate 35 duplications
perf: add MaxElements limit to infinite sequences
test: add EvaluatorTests for arithmetic operations
docs: translate README to English
```

Rationale: this is a portfolio project. Commit history is the only proof of work recruiters can audit.

## Known typos (search carefully)

| Intended | Actual in codebase |
|---|---|
| `Environment` | `Enviroment` (missing `n`) |
| `Function` | `Fuction` (missing `n`) |
| `Analyze`/`Analyzer` | `Analize` / `Anallizer` |
| `issuedContext` | `issuedcontext` |

## Architecture (completed — Fase 1)

```
Wall-E.Domain/              → 0 external dependencies
  AST/                      Node, NodeType, INodeVisitor<T>
  Figures/                  Point, Line, Circle, Line, Segment, Ray, Arc, Measure
  Evaluation/               EvaluationContext, EvaluationResult (sealed), EvaluatorVisitor
  Geometry/                 GeometricTools
  RandomProvider.cs, Scope.cs, Function.cs, DrawObject.cs, Error.cs, Location.cs

Wall-E.Application/         → depends on Domain
  Interfaces/               ILexer, IParser, IPipeline
  DSL/                      GeneralLexer, Lexer, Token, TokenStream, GeneralParser, Parser
  Pipeline/                 PipelineOrchestrator

Wall-E.Infrastructure/      → depends on Application
  FileSystem/               GeoLibraryLoader

Wall-E.UI.Avalonia/         → depends on Application + Infrastructure (TODO - Fase 2)
  ViewModels/               MainViewModel, CanvasViewModel
  Views/                    MainWindow.axaml
  Rendering/                SkiaRenderer, StreamRenderer, GridRenderer
```

## Architecture quirks (pre-refactor — legacy project `Wall-E.csproj` only)

- **Many non-UI classes inherit `Form`** (`Figure`, `ArchiveAnalysis`, `Evaluator` all extend `System.Windows.Forms.Form`).
- **Pipeline**: `GeneralLexer` (regex line-by-line) → `GeneralParser` (recursive descent) → `GeneralEvaluation` (single 1320+ line `if/else` chain).
- **AST node types**: nearly all use a single `Node` class with `NodeType` enum (82 values). The `Expression`/`Binary`/`Unit`/`Ternary` subclasses exist but are **not used** by the pipeline.
- **Evaluator** at `Evaluation/Evaluator/Evaluator.cs` handles all node types in one massive method.
- **Import system**: `.geo` files must be placed in `GeoLibrary/`; filename must be unique across subdirectories.

## Fase 0 & 1 — Audit post-completion (2026-07-06)

All items from the ROADMAP were verified against actual code. Results:

### Fase 0 — ✅ All 15 items verified COMPLETE

| # | Item | Status | File |
|---|---|---|---|
| 0.1 | `volatile bool Continue` | ✅ | `Graphic/Form1.cs:15` |
| 0.2 | Async pipeline (`async void` + `CancellationTokenSource _cts`) | ✅ | `Graphic/Form1.cs:16,32` |
| 0.3 | Stop button → `_cts.Cancel()` | ✅ | `Graphic/Form1.cs:346-350` |
| 0.4 | `GeneralEvaluation` → CancellationToken prop | ✅ | `Evaluation/Evaluator.cs:12` |
| 0.5 | MaxElements = 10000 default in AbsSequence | ✅ | `AST/Sequence/GeneralSequence.cs:3` |
| 0.6 | All infinite sequence constructors use `.Take(MaxElements)` | ✅ | All `Infinite Sequence.cs` constructors |
| 0.7 | TakenSequence wrapper | ✅ | `AST/Sequence/TakenSequence.cs` |
| 0.8 | Safe iteration in Sum.cs | ✅ | `AST/Expression/Binary/Numeric/Sum.cs:31-41` |
| 0.9 | Safe iteration in Sequence Concatenation.cs | ✅ | `AST/Sequence/Sequence Concatenation.cs:38-48` |
| 0.10 | GenerateSamples unique Point instances | ✅ | `Enviroment/Context.cs:128-131` |
| 0.11 | ParseSum_O_Sub NodeExpression fix | ✅ | `Parser/Parser.cs` |
| 0.12 | ContainsKey replaces Keys.Contains | ✅ | `Evaluation/Evaluator/Evaluator.cs` (all lookups) |
| 0.13 | RandomProvider ThreadLocal | ✅ | `Enviroment/RandomProvider.cs`, `Domain/RandomProvider.cs` |
| 0.14 | Context.Clear() + TryAdd limits | ✅ | `Enviroment/Context.cs:57-69,71-100` |
| 0.15 | Dead code + GDI+ Dispose | ✅ | All 7 files cleaned; `Form1.Designer.cs:20-25` |

**Note on `while(true)` generators**: `GenerateRandoms`, `GenerateSamples`, `GeneratePointsInFigure` in `Context.cs` are unbounded at the generator level (raw `IEnumerable<T>`) but all **consumption** goes through `AbsSequence` subclasses which apply `.Take(MaxElements=10000)` in their constructors. This is safe by design — the cap lives at the sequence wrapper, not the generator.

### Fase 1 — ✅ All 14 items verified COMPLETE

| # | Item | Status | Notes |
|---|---|---|---|
| 1.1 | Domain csproj (net6.0, 0 deps, Nullable, ImplicitUsings) | ✅ | `Wall-E.Domain.csproj` |
| 1.2 | AST + Figures moved to Domain | ✅ | Namespace `Wall_E.Domain`, no Form inheritance |
| 1.3 | Evaluation + Enviroment moved to Domain | ✅ | `Evaluator.cs`, `Scope.cs`, `Function.cs`, `Error.cs` |
| 1.4 | Context split into 3 classes | ✅ | `EvaluationContext`, `FigureRepository`, `RenderScene` |
| 1.5 | IEvaluationResult sealed hierarchy | ✅ | `EvaluationResult.cs` — 6 sealed records + implicit conversions |
| 1.6 | INodeVisitor<T> + EvaluatorVisitor | ✅ | 72 methods in interface; visitor delegates to adapted Evaluator via fallback |
| 1.7 | Application csproj | ✅ | `Wall-E.Application.csproj` → Domain |
| 1.8 | Lexer/Parser moved to DSL/ | ✅ | `GeneralLexer`, `Lexer`, `Token`, `TokenStream`, `GeneralParser`, `Parser` |
| 1.9 | PipelineOrchestrator + ExpressionCache | ✅ | `PipelineOrchestrator.cs`, `ExpressionCache.cs` |
| 1.10 | Interfaces ILexer, IParser, IPipeline, IEvaluator | ✅ | 4 interfaces in `Application/Interfaces/` |
| 1.11 | Infrastructure csproj | ✅ | `Wall-E.Infrastructure.csproj` → Application |
| 1.12 | GeoLibraryLoader | ✅ | `FileSystem/GeoLibraryLoader.cs` |
| 1.13 | Result<T,E> monádico | ✅ | `Domain/Evaluation/Result.cs` — Ok/Fail/Map/Bind/ValueOr |
| 1.14 | Solution file | ✅ | `src/Wall-E.sln` with all 3 projects |

### Clean architecture check — WinForms dependency scan

All `.cs` files in `src/` (Domain, Application, Infrastructure) were scanned for `System.Windows.Forms`, `System.Drawing`, `: Form`, and `MessageBox`. **Zero violations.** The new architecture is clean.

### Gaps found and resolved during audit

| Gap | Found | Resolution |
|---|---|---|
| `GeneralEvaluator.cs` in Domain referenced old `Context` class and 2-param `Evaluator` constructor | During audit | **Removed** — orchestration is in `Application/Pipeline/PipelineOrchestrator.cs` |

### Known gaps explicitly deferred (per ROADMAP)

| Gap | Deferred to | Reason |
|---|---|---|
| `ArchiveAnalysis : Form` + `MessageBox` calls | Fase 2 (Avalonia UI) | Requires UI layer; Domain/Application have no UI |
| `EvaluatorVisitor` 53+ methods delegate to old `Evaluator` | Progressive migration (Fase 1+) | Transitional bridge; each method gets independent impl over time |
| Tests (unit, integration, CI) | Fase 5 | Planned after architecture is stable |
| `net6.0` SDK not available on dev machine | Infrastructure setup | `dotnet build` cannot run; verification by code review only |

## Performance bottlenecks (pre-fix, all resolved in Fase 0)

- **Draw loop blocks UI thread** — `Form1.cs:116-125` consumes infinite sequences in `while(Continue)` on the UI thread with no yield.
- **No consumption limit on infinite sequences** — `foreach` over infinite `.Sequence` in `Sum.cs:31-44` / `Sequence Concatenation.cs:38-68` never terminates.
- **Three `while(true)` generators** in `Context.cs:56,71,89` (`randoms`, `samples`, `points`).
- **No async, no CancellationToken** anywhere. `Continue` flag is not `volatile`.
- **Bug**: `GenerateSamples` (`Context.cs:66-82`) reuses the same `Point` reference.
- **Bug**: `ParseSum_O_Sub` (`Parser.cs:922-924`) sets `NodeExpression = "-"` even for Sum.

All resolved in Fase 0. See `PERFORMANCE_PLAN.md` for the optimization roadmap.  
See `ROADMAP.md` for the unified implementation plan across all areas.

## Other

- README and source comments are in **Spanish** (Cuban).
- No `.editorconfig`, no StyleCop, no explicit NuGet package references (SDK-implicit only).
- Nullable reference types enabled (`<Nullable>enable</Nullable>`).
- Error collection per pipeline phase, displayed via `MessageBox` popups.
