# WALL-E — Registro de Migración del EvaluatorVisitor

Documentación en vivo del plan **"cerrar Fase 1: migración completa del `EvaluatorVisitor` + red de tests"**.
Cada sección registra qué se hizo, qué se encontró y cómo se verificó.

Estado del plan al final de este archivo: **COMPLETO — Fase 1 cerrada**.

---

## Fase A — Entorno y línea base ✅

### A.1 — Instalación de .NET SDK

- Ubuntu 24.04 no tenía `dotnet`; `sudo` requiere contraseña → instalado en modo usuario
  con el script oficial `dot.net/v1/dotnet-install.sh` en `~/.dotnet`.
- Versión instalada: **8.0.424** (canal 8.0 LTS).
- PATH y `DOTNET_ROOT` persistidos en `~/.bashrc`.

### A.2/A.3 — Primer build + errores latentes

El código de `src/` **nunca se había compilado** (no había SDK). El primer
`dotnet build src/Wall-E.sln` reveló 6 familias de errores latentes:

| Error | Archivo | Causa raíz | Corrección |
|---|---|---|---|
| CS0535 | `EvaluatorVisitor.cs` | `INodeVisitor` declara `VisitIf` pero el visitor no lo implementa (el parser nunca produce nodos `IF`: crea `Conditional` con 3 ramas) | `VisitIf` implementado como alias de `VisitConditional` |
| CS0236 ×5 | `EvaluationContext.cs` | Inicializadores de campo referenciando métodos de instancia (`Sin`, `Cos`, generadores) — ilegal en C# | Inicialización movida al constructor |
| CS1503 ×7 | `Infinite Sequence.cs` | `.Take(MaxElements)` con `MaxElements` como `long` (firma de LINQ es `int`) | `MaxElements`/`DefaultMaxElements` cambiados a `int` (valor 10000; los usos como `long limit` siguen compilando por ensanchamiento) |
| CS1503 ×6 | `EvaluatorVisitor.cs` (figuras) | Argumentos intercambiados: firmas reales son `RandomLine(List<Line>, List<Point>)` etc., el código pasaba `(points, lines)` | Argumentos reordenados según firma |
| CS8120 | `EvaluatorVisitor.VisitCount` | `case Finite_Sequence<object>` inalcanzable: subsumido por `case GenericSequence<object>` anterior | Casos específicos reordenados antes del genérico |
| CS0246 ×5 | `ILexer/IParser/IPipeline.cs` | Falta `using Wall_E.Domain;` para tipos `Error`/`Node` | Usings añadidos |

**Resultado**: build verde (0 errores, 9 warnings preexistentes CS0108).
Commit: `221e651 fix: first successful build of src/Wall-E.sln`
(incluye además los 4 métodos migrados sin commitear: `Instructions`, `LetExp`, `DeclaredFuc`, `Fuction`).

---

## Fase B — Red de seguridad: tests de caracterización ✅

Objetivo: capturar el comportamiento ACTUAL (visitor con fallback al legacy) como regresión
permanente antes de tocar los lotes riesgosos de migración.

- [x] B.1 `tests/Wall-E.Application.Tests` creado (xUnit, net8.0), agregado a `src/Wall-E.sln`
- [x] B.2 `PipelineOrchestrator` expone ahora `Context`, `Figures`, `Scene` (antes solo `Errors`)
- [x] B.3–B.7 Suite completa: **27 tests en verde** (`3b3fb56`)
  - `ArithmeticCharacterizationTests` (9): aritmética, precedencia, trig, PI
  - `FigureCharacterizationTests` (5): declaraciones, draw, color por defecto
  - `ControlFlowCharacterizationTests` (5): asignación+lectura, if-then-else, let-in, funciones
  - `SequenceCharacterizationTests` (5): caracterización de secuencias (ver bugs conocidos)
  - `ErrorCharacterizationTests` (4): división por cero, reasignación, variable desconocida

### Bugs descubiertos por la caracterización

| # | Bug | Estado |
|---|---|---|
| 1 | `VisitGlobalVar` almacenaba variables bajo el `ToString()` del record (`"StringResult { Value = x }"`), rompiendo toda lectura posterior. Ídem tags de `draw`. | **CORREGIDO** (`1087a50`, helper `RawString`) |
| 2 | `Finite_Sequence<T>.ToString()` usa formato malformado `"Type {}"` → `FormatException`. Cualquier sentencia cuyo valor sea secuencia finita/vacía revienta el pipeline. | **CORREGIDO** (Lote 4, `88a39b2`) |
| 3 | El puente fallback envuelve cualquier secuencia legacy como `StringResult(ToString())` → `count({1...100})` responde "can't count this type". | **CORREGIDO** (Lote 4: `WrapResult` mapea `AbsSequence → SequenceResult`; puente eliminado en Fase D) |
| 4 | `let-in` produce error sintáctico (`let x = 5 in ...`) o NRE (con `;`). | Documentado; pendiente de decisión (ver "Deudas conocidas") |
| 5 | **Property shadowing**: `GenericSequence<T>` declara `public new long count` / `new IEnumerable<T> Sequence`, ocultando los miembros de `AbsSequence` con almacenamiento distinto. Todo acceso vía referencia `AbsSequence` leía 0 (count nunca asignado) e `IsInfinite` era siempre falso → concatenaciones vacías (`count({1,2} + {3,4}) == 0`). Latente también en `Sum.cs`. | **CORREGIDO** (Lote 4, `88a39b2`: los consumidores castean a `GenericSequence<T>` antes de leer; nota: refactor mayor pendiente para eliminar el shadowing) |

## Fase C — Migración de los 25 métodos restantes ✅

| Lote | Métodos | Commit |
|---|---|---|
| 1 | Conditional (impl. directa: evalúa condición, `CheckTrueORFalse.Check`, solo rama tomada); If = alias; Else/Parameters = no-ops fantasma (el parser nunca crea esos nodos); Assigment = wrapper transparente | `f3c436e` |
| 2 | PointFuc, CircleFuc, LineFuc, SegmentFuc, RayFuc, MeasureFuc, Arc con `FigureResult` tipado y propagación de errores; `measure()` devuelve `NumberResult(Value)` (no existe variante Measure en la jerarquía sellada); Measure plano = fantasma; helper `IsDistance` | `5f06abc` |
| 3 | Intersect → `SequenceResult(Finite_Sequence<Point>, count)` (`null → "undefined"`); Import reporta error semántico "import requires UI/Infrastructure layer" (cablear GeoLibraryLoader exige diseño a nivel pipeline, ver Deudas) | `8205543` |
| 4 | EmptySeq, InfiniteSeq (fix deliberado: acepta doubles integrales — el lexer solo produce doubles; legacy exigía long y `{1...}` siempre fallaba), EnclosedInfiniteSeq, FiniteSeq (+`ClassifySequenceType`), Randoms, Samples, Points, PointSeq (`point sequence ps;`), LineSeq; Concat = fantasma (la concatenación fluye por Sum). **Bugs #2 y #5 corregidos aquí** | `88a39b2` |
| 5 | GlobalSeq unificado: las ~600 líneas legacy (una copia por cada tipo de secuencia) se reducen a una sola implementación con dispatch polimórfico de `ReturnValue()`: `_` descarta, última posición recibe el resto como secuencia finita (`"{}"` si agotada), posiciones previas un elemento cada una (`"undefined"` si agotada); RHS no-secuencia ahora da error semántico en vez de ignorarse silenciosamente | `44b7243` |

Suite final tras Lotes 1–5: **43 tests en verde**.

## Fase D — Eliminar la muleta ✅

- [x] Borrar `EvaluateFallback` y `Evaluator.cs` legacy (1933 líneas) (`4e64be1`)
- [x] `WrapResult` se conserva únicamente para envolver resultados de los nodos
  expresión de Domain (`Sum`, `Pow`, …) que aún se autoevalúan internamente.
- [x] Scan: cero referencias a WinForms/Evaluator en `src/`.

## Fase E — Documentación ✅

- [x] MIGRATION_LOG.md actualizado a estado final
- [x] AGENTS.md: Fase 1 marcada cerrada, comandos de test añadidos, deudas referenciadas
- [x] ROADMAP.md: nota de estado en Fase 1 con desviaciones reales (net8.0, visitor completo, 43 tests)

---

### Deudas conocidas (post-Fase 1)

1. **let-in roto** (bug #4): requiere decisión de diseño sobre la gramática (`LetExp` ya está migrado pero el parser no lo alcanza bien).
2. **Property shadowing** en `GenericSequence<T>` (`new count`/`new Sequence`): los fixes actuales castean en los puntos de consumo; el refactor limpio es eliminar los miembros sombreados de `AbsSequence`.
3. **Import sin cablear**: `VisitImport` devuelve error; conectar `GeoLibraryLoader` requiere que el pipeline pase el loader al contexto (Infrastructure → Application ya tiene la dirección correcta).
4. **Nodos expresión autoevaluados**: `Sum`, `Pow`, etc. aún ejecutan su propio `Evaluate`; `WrapResult` sigue en pie por ellos. Migrarlos al patrón visitor es opcional (no bloquea Fase 2).
5. **Tests de GlobalSeq**: cubierto solo por probe manual; añadir casos de caracterización sería barato.

---

### Convenciones de esta migración

- Implementación directa en el visitor con `EvaluationResult` sellado; prohibido copiar el patrón duplicado del legacy.
- Tests = caracterización pura: se aserta lo que hace HOY. Si un test falla tras un lote,
  se analiza si el nuevo código está mal o si la caracterización capturó un bug del fallback;
  la decisión se documenta aquí. Nunca se ajusta un test sin justificación escrita.
- Un commit por lote/milestone, conventional commits.
