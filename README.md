# GeoWall-E

[![CI](https://github.com/LiaLopezRosales/WALL-E/actions/workflows/ci.yml/badge.svg)](https://github.com/LiaLopezRosales/WALL-E/actions/workflows/ci.yml)
![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)

**GeoWall-E** is a geometric drawing interpreter: you write short DSL programs
(points, lines, circles, sequences, functions, animation) and it renders them to
a canvas. Built around a classic compiler pipeline — **Lexer → Parser →
Evaluator → Canvas** — with a desktop UI, an animated-rendering engine, and a
headless CLI that exports PNG/SVG.

This is a complete, test-driven project demonstrating compiler engineering,
clean architecture, and modern .NET — not just a demo wrapper.

## Why this project

- **Full interpreter**, not a thin wrapper: a hand-written lexer + parser produce
  an abstract syntax tree, evaluated by a **Visitor** over ~90 node kinds, with an
  extended DSL (functions, sequences, control flow, `animate`, imports, colors).
- **Clean / Onion Architecture** (`Domain → Application → Infrastructure`) with
  a strict dependency direction and **zero external dependencies in Domain**.
- **Functional patterns**: a `Result<T, E>` monad for error handling and sealed
  records throughout evaluation.
- **301 passing tests** (characterization + unit + headless UI) with **CI coverage
  gated at >40% line**, enforced by GitHub Actions.
- **Deterministic by design**: infinite sequence generators are safety-bounded, so
  random/repeated figures render reproducibly via a seeded RNG.

![Basic shapes](docs/screenshots/basic.png)

## Quick start

Requires the .NET 8 SDK (installed at `~/.dotnet` in this repo's environment):

```bash
export PATH="$HOME/.dotnet:$PATH"

# Build everything
dotnet build src/Wall-E.sln

# Desktop UI (Avalonia)
dotnet run --project src/Wall-E.UI.Avalonia

# Render a .geo file headlessly to PNG or SVG
dotnet run --project src/Wall-E.CLI -- GeoLibrary/basic.geo output.png
dotnet run --project src/Wall-E.CLI -- GeoLibrary/colors.geo output.svg
dotnet run --project src/Wall-E.CLI -- GeoLibrary/samples.geo --width 1200 --height 800
```

## The DSL

```geo
// Points, lines, circles
A = point(100, 300);
B = point(300, 100);
draw line(A, B);
draw circle(point(200, 200), 80) "circle";

// Colors, gradients and fills
color red;
fill;
draw polygon(point(300, 300), 120, 6) "hexagon";
fill linear(red, blue);
draw circle(point(300, 300), 60) "gradient";

// Loops, functions and parametric animation
f(x) = x * x;
repeat(5) { draw circle(point(300, 300), f(30)); }
animate(t from 0 to 100) { draw circle(point(300, t), 40 + t / 4); }
```

## Features

| Category | Features |
|---|---|
| **Figures** | `point`, `line`, `segment`, `ray`, `circle`, `arc`, `polygon`, `ellipse` |
| **Draw & labels** | `draw fig;`, `draw fig "label";`, `label(pos, "text", size)` |
| **Colors** | 148 CSS names, `rgb()`, `rgba()`, `hsl()`, hex `#RGB`/`#RRGGBB`/`#RRGGBBAA` |
| **Chromatic ops** | `lighten(n)`, `darken(n)`, `mix(color, ratio)`, `complement()` |
| **Fills** | `fill`, `unfill`, `fill linear(c1, c2)`, `fill radial(c1, c2)` |
| **Line styles** | `dashed`, `dotted`, `dashdot`, `solid`, `thickness(n)` |
| **Control flow** | `repeat(n) {…}`, `for i in seq {…}`, `if(cond) {…}` |
| **Animation** | `animate(t from A to B) {…}` — precomputed frames with Play/Pause |
| **Functions** | `f(x) = expr;`, `let x = expr in expr` |
| **Sequences** | `{1,2,3}`, `seq s = a..b`, `randoms`, `points`, `samples` |
| **Math** | `sin`, `cos`, `sqrt`, `abs`, `floor`, `ceil`, `phi`, `sqrt2`, `PI` |
| **Organization** | `layer n`, `hide label`, `show label`, `snap n` |
| **I/O** | `print(expr)`, `seed(n)`, `import "file"` |
| **Export** | PNG and SVG via the headless CLI |

![Colors and fills](docs/screenshots/colors.png)

## Architecture

```
Wall-E.Domain         AST, figures, evaluation, Result<T,E>, function/scope — zero deps
Wall-E.Application    DSL lexers/parsers, PipelineOrchestrator, interfaces (ILexer, IParser, …)
Wall-E.Infrastructure File system, GeoLibrary loader (IGeoLibrarySource injection)
Wall-E.CLI            Headless renderer (PNG/SVG via SkiaSharp)
Wall-E.UI.Avalonia    Desktop UI (Avalonia 11 + SkiaSharp, MVVM, syntax highlighting)
```

Dependency direction is strictly **Infrastructure → Application → Domain**
(no cycles, no external packages in Domain). The evaluator applies the **Visitor
pattern** over a discriminated AST, replacing the original monolithic evaluator
(note: an earlier WinForms version lives in `legacy/` for history only — it is
**not** part of the active codebase).

## Tests

301 tests across three suites, all green in CI:

| Suite | Tests | Coverage |
|---|---|---|
| `Wall-E.Application.Tests` | 227 | Full pipeline characterization + lexer/parser isolated |
| `Wall-E.Domain.Tests` | 68 | Domain unit tests (ColorTable, HSL, Result monad, intersections) |
| `Wall-E.UI.Tests` | 6 | MainViewModel tests via Avalonia.Headless |

Line coverage is measured with Coverlet and gated at **>40%** in CI
(measured 2026-09: Domain 50.7%, Application 76.7%, combined 60.9%).

```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet test src/Wall-E.sln
```

CI (`.github/workflows/ci.yml`, ubuntu, Release) runs build + all tests on every
push/PR, plus a coverage job that fails the build under 40% line coverage.

## Example programs

The desktop UI ships 8 progressive demo programs (auto-loaded from the **Ejemplos**
dropdown):

| File | Demonstrates |
|---|---|
| `programs/01-basics.geo` | Points, lines, circles, segments, labels |
| `programs/02-colors.geo` | Full color system: CSS names, HSL, hex, gradients |
| `programs/03-sequences.geo` | Finite/infinite sequences, ranges, `{…} + {…}` |
| `programs/04-loops.geo` | `repeat`, `for`, conditionals, functions |
| `programs/05-figures.geo` | Polygons, ellipses, arcs |
| `programs/06-layers.geo` | `layer`, `hide`/`show`, `snap` |
| `programs/07-math.geo` | Trigonometry, constants, `samples` |
| `programs/08-animate.geo` | Parametric `animate` with Play/Pause |

![Samples showcase](docs/screenshots/samples.png)

## Sample files

| File | Description |
|---|---|
| [`GeoLibrary/basic.geo`](GeoLibrary/basic.geo) | Basic shapes: points, lines, circles, segments |
| [`GeoLibrary/colors.geo`](GeoLibrary/colors.geo) | Full color system: CSS names, HSL, gradients |
| [`GeoLibrary/grid.geo`](GeoLibrary/grid.geo) | Grid with layers, snap, hide/show |
| [`GeoLibrary/samples.geo`](GeoLibrary/samples.geo) | Polygons, ellipses, concentric circles |

## Documentation

| Document | Contents |
|---|---|
| [`AGENTS.md`](AGENTS.md) | Architecture, conventions, DSL semantics |
| [`ROADMAP.md`](ROADMAP.md) | Implementation plan and audit (M6–M12) |
| [`MIGRATION_LOG.md`](MIGRATION_LOG.md) | Evaluator migration record (monolith → Visitor) |
| [`DEBT_SPRINT.md`](DEBT_SPRINT.md) | Post-migration debt sprint |
