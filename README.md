# GeoWall-E

**GeoWall-E** is a geometric drawing interpreter for a small DSL: write short programs (points, lines, circles, sequences, functions, conditionals) and render them on a canvas. Pipeline: **Lexer → Parser → Evaluator → Canvas**.

```
point p1 = point(100, 100);
circle c = circle(point(200, 200), 50);
draw p1, "A"; draw c;
count({1,2,3});                    // 3
f(x) = x * 2; f(21);               // 42
let y = (let z = 2 in z) in y * 3; // 6
```

## Project status

Two codebases coexist in this repository:

| | Legacy | New architecture |
|---|---|---|
| Project | root `Wall-E.csproj` (`net6.0-windows`, WinForms) | `src/Wall-E.{Domain,Application,Infrastructure}` (`net8.0`) |
| Status | fully working, Windows only | Clean Architecture, complete visitor-based evaluator, 59 characterization tests |
| UI | WinForms | Avalonia UI + MVVM (in progress, see ROADMAP Fase 2) |

The new architecture follows a strict dependency direction (**Infrastructure → Application → Domain**, zero external dependencies in Domain), replaces the legacy god-object evaluator with the Visitor pattern and sealed result records, and wires `import` through an injectable `IGeoLibrarySource`.

### Build & test (new architecture)

```bash
dotnet build src/Wall-E.sln
dotnet test tests/Wall-E.Application.Tests/Wall-E.Application.Tests.csproj
```

### Run the legacy app (Windows)

```bash
./Wall-E.sh          # or: dotnet build Wall-E.csproj && dotnet run
```

Requires the `net6.0-windows` SDK; `.geo` importable files live under `GeoLibrary/`.

## Documentation

- [`AGENTS.md`](./AGENTS.md) — architecture map, conventions, DSL semantics quirks
- [`ROADMAP.md`](./ROADMAP.md) — unified implementation plan (Fases 0–6)
- [`MIGRATION_LOG.md`](./MIGRATION_LOG.md) — complete record of the evaluator migration
- [`DEBT_SPRINT.md`](./DEBT_SPRINT.md) — post-migration debt sprint record

---

## Español

**GeoWall-E** es un intérprete de dibujo geométrico para un lenguaje pequeño: se escriben programas cortos (puntos, segmentos, circunferencias, secuencias, funciones) y se grafican en una pizarra. El proyecto original se desarrolló con interfaz gráfica en WindowsForms, que presenta un espacio de gráficos o pizarra, un recuadro para introducir los comandos y tres botones cuyas funciones se especifican más adelante.

### Funcionamiento básico (aplicación legacy)

1. Abrir la aplicación de WindowsForm: ejecutando el programa desde un editor de código o desde la consola mediante el script `Wall-E.sh`.
2. Escribir el programa en la caja de texto: no debe dejarse vacía.
   - **Importar archivos**: el archivo a importar debe estar en `\WALL-E\GeoLibrary` con extensión `.geo`; puede estar en una subcarpeta pero su nombre debe ser único.
3. Procesar con el botón "Process Commands".
4. Cerrar o escribir un nuevo programa: terminado el procesamiento se puede introducir código nuevo sin reiniciar.

### Características de la aplicación de WindowsForm

- El botón "Clean" limpia la pizarra para seguir dibujando figuras encima.
- El botón "Jump seq" detiene la impresión de una secuencia infinita sin cerrar el programa.
- Los cuadros de mensajes detienen el ciclo de ejecuciones: hay que cerrar la última ventana emergente para concluir y limpiar el TextBox.
- Si hay figuras que dibujar, esa es la última acción; si no, se muestran en ventanas emergentes los elementos procesados y los errores encontrados hasta ese punto.

### El lenguaje DSL

- **Puntos con coordenadas**: deben estar en un rango de 50 a 310 para ambos elementos, y la diferencia entre un punto y otro debe ser de al menos 15 unidades para distinguirlos. El radio directo de una circunferencia conviene mayor a 25 unidades para que sea visible.
- **Operaciones matemáticas**: dejar espacio entre el símbolo `-` y el número siguiente.
- El símbolo `;` determina el fin de una instrucción; su mala colocación no se detecta por sí sola pero puede ocasionar el procesamiento de instrucciones inválidas.
- `{secuencia} + undefined` devuelve la primera secuencia sin cambios (comportamiento de concatenación, por diseño).
- Los errores informan tipo, explicación y localización (archivo, línea y columna; los semánticos solo archivo y línea).
- El procesamiento es por etapas (tokenizado línea a línea → parseo → evaluación) y cada instrucción produce un mensaje:
  - `color blue;` → `Color changed to blue`
  - `point p1;` → `Point created`
