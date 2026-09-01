# WALL-E — Sprint de Deudas Post-Migración

Plan detallado para saldar las deudas conocidas de la Fase 1 **antes** de iniciar la Fase 2
(Avalonia UI). Justificación de prioridad en la conversación de planificación y resumida aquí:
las deudas cuestan días, la Fase 2 semanas; la red de tests (43/43) está fresca y es la red
de seguridad más barata que habrá nunca para tocar gramática y jerarquía de secuencias; y el
renderizador de la Fase 2 consumirá secuencias directamente — el shadowing sin corregir es una
mina terrestre para ese consumo.

Estado al cierre de este archivo: **COMPLETO ✅ — sprint ejecutado íntegramente**.
Suite final: 59/59 tests en verde.

Orden de ejecución (cada tarea = un commit, tests en verde antes de avanzar):

```
T1 Shadowing (raíz)  →  T2 let-in  →  T3 import  →  T4 Tests GlobalSeq   [T5 diferida]
```

### Resultados de la ejecución

| Tarea | Commit | Tests |
|---|---|---|
| T1 shadowing | `d841fc3` | 44 |
| T2 let-in | `c5b5f68` (+ `ce195b6` corrige aserción) | 47 |
| T3 import | `8c89892` | 52 |
| T4 GlobalSeq | `64c2324` | 59 |

Hallazgos que refinaron el plan durante la ejecución:

- **T1**: el diseño final declara `count` como propiedad **abstracta en `AbsSequence`**
  (una sola implementación en `GenericSequence<T>`) en vez de moverla sin más: así toda
  lectura por referencia base es polimórficamente correcta y `WrapResult` no necesita
  casts. Se confirmó el bug latente con probe (`{seq}+undefined` reportaba `Count=0`,
  ahora `Count=2`, cubierto por test de regresión). También se eliminó `IsExhausted`
  (cero consumidores) y los enumeradores privados duplicados de las subclases.
- **T2**: fueron **tres defectos coordinados**, no uno: (a) `Parser.GlobalVar` consumía
  incondicionalmente el token tras el valor y se comía el `in`; (b) `GeneralLexer`
  dejaba `amount_of_open_let` colgando cuando un chunk cerraba sus propios lets y se
  tragaba silenciosamente todos los statements siguientes (por eso `x;` desaparecía);
  (c) `VisitVar` consultaba `GlobalConstant` antes que las variables de scope, haciendo
  imposible el shadowing. Nota clave: como let-in **nunca parseó en el legacy**, no había
  comportamiento que preservar — la semántica actual es decisión de diseño documentada
  (cuerpo con instrucciones separadas por `;`, cierre `in <expr>`, scope local sombreada
  a globales). El troceado de statements por `;` vive en `GeneralLexer`, no en el parser.
- **T3**: el handler solo se cablea cuando existe fuente (si no, `VisitImport` conserva
  su error semántico); los fallos de importación van a DOS canales (`pipeline.Errors` y
  el `ErrorResult` del statement); los resultados internos de la biblioteca NO llegan a
  `Context.Results` (decisión deliberada, test lo congela).
- **T4**: el drenaje de `{1...}` con dos targets deja `rest.count == MaxElements - 1`
  (9999) y finito — invariante de seguridad verificado. Ojo para futuros tests:
  `Infinite_Sequence` produce **longs** mientras los literales finitos guardan doubles
  del lexer; `Assert.Equal(1.0, boxedLong)` falla aunque valgan lo mismo.

---

## T1 — Eliminar el property shadowing en la jerarquía de secuencias

**Deuda**: MIGRATION_LOG.md bug #5. El fix del Lote 4 parcheó los puntos de consumo;
esta tarea elimina la raíz.

### Diagnóstico exacto (verificado en código)

Triple sombreado de `Sequence` con almacenamiento distinto por nivel:

| Nivel | Miembro | Problema |
|---|---|---|
| `AbsSequence` (`GeneralSequence.cs`) | `long count`, `IEnumerable<object>? Sequence`, `IsInfinite => count < 0`, `IsExhausted` | `count`/`Sequence` **nunca asignados** (siempre 0/null); `IsInfinite` siempre falso |
| `GenericSequence<T>` (`Sequence.cs`) | `new long count`, `new IEnumerable<T>? Sequence` | Los reales usados por subclases |
| Subclases | `Finite_Sequence<T>.new Sequence`, `Infinite_Sequence.new Sequence` + `private enumerator` propio, `InfinitePointSequence.*`, `InfiniteDoubleSequence.*`, `Enclosed_Infinite_Sequence` (revisar archivo) | Segunda capa de sombreado + enumeradores duplicados |

Consumidores vía referencia base detectados:
- `EvaluatorVisitor.WrapResult` hace `seq.count` con `seq: AbsSequence` → `SequenceResult.Count`
  **siempre vale 0** hoy (latente: nadie consume `.Count` aún, pero el dato está mal).
- `Sum.cs` y `Sequence Concatenation.cs`: ya parcheados con casts (T1 los simplifica).

### Diseño

1. `AbsSequence` conserva SOLO: `DefaultMaxElements`, `MaxElements`. Se eliminan
   `count`, `Sequence`, `IsInfinite`, `IsExhausted` (sin consumidores reales).
2. `GenericSequence<T>` pasa a DECLARAR (no sombrear): `long count {get; protected set;}`,
   `IEnumerable<T>? Sequence`, `IsInfinite => count < 0`, `protected IEnumerator<T> enumerator`.
3. Subclases: eliminar TODOS los `new ... Sequence`, `new long count` y enumeradores
   privados re-declarados. Las asignaciones (`count=-1`, `Sequence=...`) resuelven a los
   miembros heredados sin cambios de comportamiento.
4. `WrapResult`: `seq.count` ahora lee el valor correcto polimórficamente.
5. Simplificar los casts defensivos de T1-precedentes en `Sum.cs`/`Concat` si el compilador
   ya resuelve bien (opcional, solo si no añade riesgo).

### Archivos afectados

```
src/Wall-E.Domain/AST/Sequence/GeneralSequence.cs      (AbsSequence)
src/Wall-E.Domain/AST/Sequence/Sequence.cs             (GenericSequence<T>)
src/Wall-E.Domain/AST/Sequence/Finite Sequence.cs
src/Wall-E.Domain/AST/Sequence/Infinite Sequence.cs    (3 clases: Infinite_Sequence,
                                                        InfinitePointSequence, InfiniteDoubleSequence)
src/Wall-E.Domain/AST/Sequence/Enclosed Infinite Sequence.cs
src/Wall-E.Domain/AST/Sequence/TakenSequence.cs        (solo verificar)
src/Wall-E.Domain/Evaluation/EvaluatorVisitor.cs       (WrapResult)
```

### Verificación

- Build limpio + 43 tests en verde.
- Probe: concat finitas (`count({1,2}+{3,4})==4`), infinitas (`count({1...100})==100`),
  `{seq}+undefined`, GlobalSeq rest, `point sequence ps;`.
- **Test nuevo**: asertar `SequenceResult.Count == 3` para `{1,2,3}` (hoy devolvería 0) —
  regresión permanente del dato antes latente.

**Riesgo**: consumidores ocultos contra miembros sombreados → el build los revela (ruido
positivo). `ReturnValue()` de `Infinite_*` usaba enumerador privado: tras unificar, verificar
que devuelve `long.MinValue`/`default(Point)` al agotarse igual que antes.

---

## T2 — Reparar la gramática let-in

**Deuda**: bug #4. `VisitLetExp` ya está migrado (visitor, líneas ~174); el defecto es del parser.

### Síntomas reproducidos (probe)

| Entrada | Resultado actual |
|---|---|
| `let x = 5 in x + 1;` | Error sintáctico "Invalid let-in expression" |
| `let x = 5; in x + 1;` | Sin errores visibles — verificar si produce resultado (sospechoso: silencio total) |

### Causa raíz (análisis estático)

`Parser.Let_In()` (Parser.cs:700–729): bucle `do { ParseStatement(); } while (token != "in")`.

1. Variante SIN `;`: `ParseStatement` → `GlobalVar` exige `;` → consume/error desincroniza el
   stream → el guard `Position() >= tokens.Count - 1` dispara el error espurio.
2. Variante CON `;`: hay que trazar qué queda tras consumir `;` y si el `in` se parsea como
   statement huérfano (el lexer lo tokeniza como keyword, Parser.cs línea 162; `ParseStatement`
   no tiene rama para él).

### Plan de corrección

1. Trazar con probes las dos variantes + anidada (`let x = (let y = 2 in y) in x;`) + let
   dentro de función. Documentar hallazgos aquí durante ejecución.
2. Decidir la gramática canónica (la del legacy es la referencia; el README no documenta
   let-in): propuesta — instrucciones internas separadas por `;`, cierre obligatorio con
   `in <expresión>`, `;` final opcional tras la expresión si termina el programa.
3. Corregir `Let_In()` (manejo de `;` opcional dentro del cuerpo, detección robusta de `in`,
   guard de fin-de-stream sin error falso).
4. Reemplazar el test `KNOWN_BUG` de `ControlFlowCharacterizationTests` por aserciones tipadas
   del valor real (`let x = 5 in x+1` → `NumberResult(6)`), más casos de scope (variable interna
   no filtra al exterior).

### Archivos afectados

```
src/Wall-E.Application/DSL/Parser.cs            (Let_In)
tests/Wall-E.Application.Tests/ControlFlowCharacterizationTests.cs
```

**Riesgo**: bajo — la suite de caracterización congela el resto del lenguaje mientras se toca
el parser. La gramática elegida debe quedar documentada en este archivo al cerrar la tarea.

---

## T3 — Cablear import a GeoLibraryLoader

**Deuda**: `VisitImport` devuelve error semántico desde el Lote 3. Restricción estructural:
Domain no puede referenciar Infrastructure (dirección de dependencias), así que la carga
de bibliotecas debe inyectarse.

### Diseño propuesto (inyección por delegado)

1. Nueva interfaz en Application/Interfaces:
   ```csharp
   public interface IGeoLibrarySource { string? Resolve(string libraryName); }
   ```
2. `GeoLibraryLoader` (Infrastructure) implementa `IGeoLibrarySource` — ya tiene
   `ListGeoFiles`/`ReadGeoFile`; `Resolve` busca `<base>/GeoLibrary/**/*.geo` cuyo nombre de
   archivo coincida (regla legacy: nombre único entre subdirectorios).
3. `PipelineOrchestrator`: ctor con parámetro opcional `IGeoLibrarySource? librarySource = null`,
   expuesto como propiedad.
4. `EvaluatorVisitor`: propiedad pública `Func<string, EvaluationResult?>? ImportHandler`.
5. El orquestador cablea el handler: resolver → lexear+parsear+evaluar el contenido con el
   MISMO visitor/contexto (las definiciones quedan disponibles globalmente), con guarda de
   ciclos (`HashSet<string>` de nombres en carga por `Execute`) y profundidad máxima.
6. `VisitImport`: si `ImportHandler == null` → error semántico actual (comportamiento
   preservado); si existe → delegar y propagar `ErrorResult` del sub-parse.

### Archivos afectados

```
src/Wall-E.Application/Interfaces/              (+IGeoLibrarySource)
src/Wall-E.Infrastructure/FileSystem/GeoLibraryLoader.cs
src/Wall-E.Application/Pipeline/PipelineOrchestrator.cs
src/Wall-E.Domain/Evaluation/EvaluatorVisitor.cs (VisitImport)
tests/... (nuevo ImportCharacterizationTests o dentro de ErrorTests)
```

### Verificación

- Test unitario con fake inline de `IGeoLibrarySource` (los tests no dependen de Infrastructure):
  import exitoso define función/constante usable después; ciclo `a↔b` corta con error; nombre
  inexistente da error semántico; handler ausente mantiene el error actual.
- Test de integración opcional con directorio temporal + loader real.

**Riesgo**: medio-bajo. El sub-parse comparte estado mutable del visitor — probar explícitamente
que una importación a mitad de programa no rompe las declaraciones previas.

---

## T4 — Caracterización de GlobalSeq

**Deuda**: cobertura solo manual (probe). Barato y cierra la última zona ciega.

Casos mínimos (nuevo `GlobalSeqCharacterizationTests.cs`):

1. `a,b = {1,2};` → a=1, b=2
2. `a,_ = {1,2,3};` → a=1, `_` descarta
3. `x,y = {1,2,3};` → x=1, y=Finite_Sequence{2,3} (rest)
4. `p,q = {};` → p="undefined", q="{}"
5. RHS no-secuencia → error semántico (divergencia documentada del Lote 5)
6. RHS infinito con rest: `x = {1...};` → x.count == MaxElements (10000) — valida el
   invariante de seguridad post-T1
7. Mezcla con `_` y rest: `a,_,b = {1,2,3,4};` → b recibe {3,4}

---

## T5 — (DIFERIDA) Migración de nodos expresión autoevaluados

`Sum`, `Pow`, etc. aún ejecutan su propio `Evaluate`; `WrapResult` sobrevive por ellos.
**Decisión: no hacerla ahora.** Racional: cero impacto funcional, refactor interno puro,
y `WrapResult` ya es correcto tras T1. Se reevalúa si la Fase 2 exige extender operadores.

---

## Definición de "done" del sprint

- [x] T1–T4 commiteadas por separado, conventional commits
- [x] Suite completa en verde (59 tests tras el sprint; eran 43)
- [x] Este documento actualizado con hallazgos de ejecución y decisiones tomadas
- [x] AGENTS.md ("Deferred gaps") y MIGRATION_LOG.md ("Deudas conocidas") actualizados:
      shadowing/let-in/import RESUELTAS; T5 queda como única deuda, explícitamente diferida
- [x] Dominio estable → arranque de Fase 2 (Avalonia UI) con base limpia
