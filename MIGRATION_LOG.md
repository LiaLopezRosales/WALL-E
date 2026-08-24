# WALL-E — Registro de Migración del EvaluatorVisitor

Documentación en vivo del plan **"cerrar Fase 1: migración completa del `EvaluatorVisitor` + red de tests"**.
Cada sección registra qué se hizo, qué se encontró y cómo se verificó.

Estado del plan al final de este archivo: **Fase A completa, Fase B en curso**.

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

## Fase B — Red de seguridad: tests de caracterización ⏳

Objetivo: capturar el comportamiento ACTUAL (visitor con fallback al legacy) como regresión
permanente antes de tocar los lotes riesgosos de migración.

- [ ] B.1 Crear `tests/Wall-E.Application.Tests` (xUnit, net8.0) + añadir a la solución
- [ ] B.2 Exponer `EvaluationContext` desde `PipelineOrchestrator` (hoy solo expone `Errors`)
- [ ] B.3 Tests B1 aritmética/lógica/trig
- [ ] B.4 Tests B2 figuras y escena
- [ ] B.5 Tests B3 control flow y funciones
- [ ] B.6 Tests B4 secuencias (crítico: finitas, infinitas acotadas por MaxElements, concat, quirk `{seq} + undefined`)
- [ ] B.7 Tests B5 errores semánticos

## Fase C — Migración de los 25 métodos restantes ⬜

| Lote | Métodos | Estado |
|---|---|---|
| 1 | Conditional, Else, Assigment, Parameters | ⬜ |
| 2 | PointFuc, CircleFuc, LineFuc, SegmentFuc, RayFuc, Arc, Measure, MeasureFuc | ⬜ |
| 3 | Intersect, Import (+ cablear GeoLibraryLoader) | ⬜ |
| 4 | EmptySeq, Randoms, Samples, Points, FiniteSeq, InfiniteSeq, EnclosedInfiniteSeq, PointSeq, LineSeq, Concat | ⬜ |
| 5 | GlobalSeq (~600 líneas legacy, extraer helpers compartidos) | ⬜ |

Verificación por lote: `dotnet build` + `dotnet test`.

## Fase D — Eliminar la muleta ⬜

- [ ] Borrar `EvaluateFallback`, `WrapResult` y `Evaluator.cs` legacy (1933 líneas)
- [ ] Scan final: cero referencias a WinForms en `src/`

## Fase E — Documentación ⬜

- [ ] AGENTS.md: quitar nota de fallback, actualizar comandos/tests, marcar Fase 1 cerrada
- [ ] ROADMAP.md: reflejar estado real

---

### Convenciones de esta migración

- Implementación directa en el visitor con `EvaluationResult` sellado; prohibido copiar el patrón duplicado del legacy.
- Tests = caracterización pura: se aserta lo que hace HOY. Si un test falla tras un lote,
  se analiza si el nuevo código está mal o si la caracterización capturó un bug del fallback;
  la decisión se documenta aquí. Nunca se ajusta un test sin justificación escrita.
- Un commit por lote/milestone, conventional commits.
