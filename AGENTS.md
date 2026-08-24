# WALL-E — AGENTS.md

## Project

GeoWall-E: geometric drawing interpreter for a small DSL (points, lines, circles, sequences, functions). Pipeline: Lexer → Parser → Evaluator → Canvas.
Two codebases coexist in this repo:

| | Legacy | New architecture |
|---|---|---|
| Project | root `Wall-E.csproj` (`net6.0-windows`, WinForms) | `src/Wall-E.{Domain,Application,Infrastructure}` (`net8.0`) |
| Status | fully working, runs on Windows only | Fase 0–1 complete; Fase 2 (Avalonia UI) not started |
| Solution | root `Wall-E.sln` (contains ONLY legacy project) | `src/Wall-E.sln` |

## Critical environment gotcha

**`dotnet` SDK is NOT on the default PATH** — it is installed user-mode at `~/.dotnet` (8.0.424). Every shell session needs `export PATH="$HOME/.dotnet:$PATH"` before any dotnet command. Legacy project additionally requires Windows (WinForms) and a `net6.0-windows` SDK.

## Commands

```bash
export PATH="$HOME/.dotnet:$PATH"                          # required first, every session
./Wall-E.sh                                                # build + run legacy WinForms app
dotnet build src/Wall-E.sln                                # build new architecture (Domain+App+Infra)
dotnet test tests/Wall-E.Application.Tests/...csproj       # characterization test suite (43 tests)
dotnet build Wall-E.csproj                                 # build legacy (needs net6.0-windows SDK)
```

No CI or lint/format config exists. Don't invent commands.

## Commit discipline (hard rule)

After every meaningful set of changes — completed task, bug fix, refactor, feature — **commit immediately**, do not batch unrelated changes, do not leave uncommitted work overnight. Conventional commits format:

```
feat: add hex color support (#rrggbb)
fix: GenerateSamples now creates unique Point instances
refactor: extract StoreVariable, eliminate 35 duplications
```

Rationale: portfolio project; commit history is what recruiters audit.

## Known typos (search carefully)

| Intended | Actual in codebase |
|---|---|
| `Environment` | `Enviroment` (missing `n`) |
| `Function` | `Fuction` (missing `n`) |
| `Analyze`/`Analyzer` | `Analize` / `Anallizer` |

These appear in class names, enums, and namespaces — renaming them breaks references everywhere.

## Architecture (new, `src/`)

Dependency direction: Infrastructure → Application → Domain. Domain has zero external deps.

```
Wall-E.Domain/              AST/, Figures/, Evaluation/ (EvaluationContext,
                            EvaluationResult sealed records, Result<T,E> monad),
                            Geometry/, RandomProvider.cs, Scope.cs, Function.cs
Wall-E.Application/         DSL/ (lexers+parsers), Pipeline/PipelineOrchestrator,
                            Caching/ExpressionCache, Interfaces/ (ILexer, IParser, IPipeline, IEvaluator)
Wall-E.Infrastructure/      FileSystem/GeoLibraryLoader
```

### Migration state (important)

- **Fase 1 is COMPLETE**: `EvaluatorVisitor` (Domain/Evaluation/EvaluatorVisitor.cs) implements every reachable node type directly via `INodeVisitor<EvaluationResult>`; the legacy-fallback bridge and the adapted 1933-line `Evaluator` class were deleted from `src/`. Full history in `MIGRATION_LOG.md`.
- Known post-migration debts (see MIGRATION_LOG.md "Deudas conocidas"): broken let-in grammar, property shadowing (`new count`) between `GenericSequence<T>` and `AbsSequence`, `import` not wired to `GeoLibraryLoader`, expression nodes (`Sum` etc.) still self-evaluate internally.
- `EvaluationContext`, `FigureRepository`, `RenderScene` replaced the old god-object `Context`.
- Characterization tests: `dotnet test tests/Wall-E.Application.Tests/...csproj` — keep them green; they are the regression net for behavior changes.
- Wall-E.UI.Avalonia (MVVM + SkiaSharp) is planned but **does not exist yet** — don't reference or build it.

## Architecture quirks (legacy project only)

- Non-UI classes inherit `Form` (`Figure`, `ArchiveAnalysis`, `Evaluator`).
- Single `Node` class + `NodeType` enum (~82 values); the `Expression`/`Binary` subclasses are not used by the pipeline.
- Import system: `.geo` files must live in `GeoLibrary/`; filename must be unique across subdirectories.
- Errors are collected per pipeline phase and shown via `MessageBox` popups (blocks execution until closed).

## Infinite-sequence safety invariant

Generators like `GenerateRandoms`/`GenerateSamples`/`GeneratePointsInFigure` are unbounded `while(true)` by design. Safety relies on every consumption path going through `AbsSequence` subclasses whose constructors apply `.Take(MaxElements)` (MaxElements=10000). **Any new code that consumes a sequence must go through the `AbsSequence` wrapper or apply its own `.Take()` bound** — otherwise it hangs forever.

## Deferred gaps (per ROADMAP.md)

- `ArchiveAnalysis : Form` + `MessageBox` → Fase 2 (Avalonia UI)
- Post-migration debts → **sprint planned in `DEBT_SPRINT.md`** (shadowing root fix, let-in grammar, import wiring, GlobalSeq tests; expression self-evaluation deferred)
- CI → Fase 5 (characterization tests already exist)
- Planning docs: `ROADMAP.md` (unified plan), `MIGRATION_LOG.md`, `DEBT_SPRINT.md`, `IMPROVEMENT_PLAN.md`, `PERFORMANCE_PLAN.md`, `ENHANCEMENTS.md`

## Other

- README and source comments are in Spanish (Cuban).
- Nullable reference types enabled; ImplicitUsings enabled; no explicit NuGet packages (SDK-implicit only).
- DSL semantics quirk: `{seq} + undefined` returns the first sequence unchanged (concatenation behavior), by design.
