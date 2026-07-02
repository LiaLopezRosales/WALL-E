# WALL-E — AGENTS.md

## Project

GeoWall-E: geometric drawing interpreter with a Windows Forms GUI.  
Target: `net6.0-windows` (Windows-only). No tests, no CI, no linter/formatter.

## Commands

```bash
dotnet build Wall-E.csproj      # build
dotnet run --project Wall-E.csproj  # run
./Wall-E.sh                     # build + run
dotnet clean                    # clean
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

## Architecture quirks

- **Many non-UI classes inherit `Form`** (`Figure`, `ArchiveAnalysis`, `Evaluator` all extend `System.Windows.Forms.Form`).
- **Pipeline**: `GeneralLexer` (regex line-by-line) → `GeneralParser` (recursive descent) → `GeneralEvaluation` (single 1320+ line `if/else` chain).
- **AST node types**: nearly all use a single `Node` class with `NodeType` enum (82 values). The `Expression`/`Binary`/`Unit`/`Ternary` subclasses exist but are **not used** by the pipeline.
- **Evaluator** at `Evaluation/Evaluator/Evaluator.cs` handles all node types in one massive method.
- **Import system**: `.geo` files must be placed in `GeoLibrary/`; filename must be unique across subdirectories.

## Performance bottlenecks (freeze/crash on fractals)

- **Draw loop blocks UI thread** — `Form1.cs:116-125` consumes infinite sequences in `while(Continue)` on the UI thread with no yield. Stop button cannot fire.
- **No consumption limit on infinite sequences** — `foreach` over infinite `.Sequence` in `Sum.cs:31-44` / `Sequence Concatenation.cs:38-68` never terminates.
- **Three `while(true)` generators** in `Context.cs:56,71,89` (`randoms`, `samples`, `points`).
- **No async, no CancellationToken** anywhere. `Continue` flag is not `volatile`.
- **Bug**: `GenerateSamples` (`Context.cs:66-82`) reuses the same `Point` reference — all yielded elements are the last mutated value.
- **Bug**: `ParseSum_O_Sub` (`Parser.cs:922-924`) sets `NodeExpression = "-"` even for Sum.

See `PERFORMANCE_PLAN.md` for the optimization roadmap.
See `ROADMAP.md` for the unified implementation plan across all areas.

## Other

- README and source comments are in **Spanish** (Cuban).
- No `.editorconfig`, no StyleCop, no explicit NuGet package references (SDK-implicit only).
- Nullable reference types enabled (`<Nullable>enable</Nullable>`).
- Error collection per pipeline phase, displayed via `MessageBox` popups.
