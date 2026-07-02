# WALL-E — Catalogo de Mejoras Potenciales

Mejoras extendidas más allá del plan principal (`IMPROVEMENT_PLAN.md`). Clasificadas por área, esfuerzo e impacto para portafolio.

---

## 1. Sistema de Color

### 1.1 Estado actual

9 colores fijos hardcodeados en el lexer (`Lexer.cs:240`) y mapeados uno a uno en el renderer (`Form1.cs:128-171`). El estado de color es global mediante un `Stack<string>` en `Context.cs`.

```csharp
// Lexer.cs — 9 colores únicamente
else if (possibletoken == "black" || possibletoken == "white"|| ...)
    token = new Token(Token.TokenType.color_value, ...);

// Form1.cs — 9 cases en switch
switch (color.ToLower()) {
    case "black": Lapiz.Color = Color.Black; Brush.Color = Color.Black; break;
    ...
    case "magenta": ... break;
    default: break;  // ← cualquier color no-listado se ignora silenciosamente
}
```

### 1.2 Hex RGB (`#rrggbb`)

Amplía de 9 colores a **16.7 millones** con una sintaxis estándar.

**DSL**: `color #FF5733;`

**Cambios**:

- **Lexer**: Nuevo token `hex_color` con regex `#[0-9a-fA-F]{6}`. Insertar antes del `double.TryParse` para evitar que `#FF5733` se interprete como no-válido.
- **Parser**: Aceptar `hex_color` como alternativa a `color_value`. Crear nueva producción o modificar `Color()`.
- **Evaluator**: Mapear string hex → `Color.FromArgb(...)`.
- **Form1 / Avalonia renderer**: Usar `ColorTranslator.FromHtml("#FF5733")` en WinForms, o `SKColor.Parse("#FF5733")` en SkiaSharp.

**Esfuerzo**: ~2 horas. **Impacto**: ⭐⭐⭐⭐⭐

### 1.3 RGB funcional (`rgb(r, g, b)`)

Sintaxis más explícita y enseñable.

**DSL**: `color rgb(255, 87, 51);`

**Cambios**:

- **Lexer**: Nuevo token `rgb` (keyword).
- **Parser**: Nueva producción `ColorRgb`:
  ```
  rgb → "rgb" "(" expr "," expr "," expr ")"
  ```
- **Evaluator**: Evaluar 3 expresiones, validar rango 0–255, construir `Color.FromArgb(r, g, b)`.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐⭐

### 1.4 Alpha / transparencia (`rgba(r, g, b, a)` o hex 8-digito)

Permite figuras semitransparentes superpuestas — efecto visual sofisticado y diferencial.

**DSL**: `color rgba(255, 87, 51, 0.5);` o `color #FF573380;`

**Cambios**:

- Extensión directa de 1.3: aceptar 4º argumento opcional en `rgb()` o token `hex8_color` con regex `#[0-9a-fA-F]{8}`.
- Renderer: usar `Color.FromArgb(alpha, r, g, b)` / `SKColor.Parse("#FF573380")`.

**Esfuerzo**: +1h sobre 1.2/1.3. **Impacto**: ⭐⭐⭐⭐

### 1.5 HSL (`hsl(h, s%, l%)`)

Los diseñadores piensan en HSL, no RGB. Demuestra conocimiento de teoría del color.

**DSL**: `color hsl(200, 80%, 50%);`

**Cambios**:

- Lexer: nuevo token `hsl`.
- Parser: producción análoga a `rgb`.
- Evaluator: convertir HSL → RGB mediante fórmula estándar.
- Renderer: usar `Color` resultante.

**Esfuerzo**: ~2 horas. **Impacto**: ⭐⭐⭐

### 1.6 Colores CSS completos (~140 nombres)

Backward compatible con los 9 actuales, pero añade `coral`, `indianred`, `seagreen`, etc.

**DSL**: `color "coral";` o `color coral;`

**Cambios**:

- Evaluator: en vez de switch de 9, usar `Color.FromName(name)` (WinForms) o lookup table a `SKColor` (Avalonia).
- Parser/lexer: aceptar cualquier identifier como color (o solo strings).

**Esfuerzo**: ~1 hora. **Impacto**: ⭐⭐

### 1.7 Gradientes en fill

No solo stroke, sino relleno con gradiente lineal o radial.

**DSL**:
```
draw circle(point(0,0), 5) fill linear(red, blue);
draw circle(point(0,0), 5) fill radial(red, blue);
```

**Cambios**:

- Requiere migración a SkiaSharp (plan sección 5).
- Usar `SKShader.CreateLinearGradient` / `CreateRadialGradient`.
- Evaluator: nuevo nodo `FillGradient` con tipo (linear/radial) y colores.

**Esfuerzo**: ~4 horas. **Impacto**: ⭐⭐⭐⭐

### 1.8 Operaciones cromáticas

Operaciones del DSL sobre colores existentes.

**DSL**:
```
color lighten(red, 20%);
color darken(blue, 30%);
color mix(red, blue, 50%);
color complement(green);
```

**Cambios**:

- Lexer: nuevos tokens `lighten`, `darken`, `mix`, `complement`.
- Parser/Eval: funciones que transforman colores en tiempo de evaluación.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐

### 1.9 Color por-figura (arquitectura)

Rompe con el estado global (`Color` stack). El color viaja como parte del comando `draw`.

**DSL actual** (estado global):
```
color red;
draw circle(point(0,0), 5);
color blue;
draw line(point(1,1), point(2,2));
```

**DSL propuesto** (color adjunto al draw):
```
draw circle(point(0,0), 5) color red;
draw line(point(1,1), point(2,2)) color blue;
```

**Cambios**:

- AST: `Draw` node gana un `ColorExpression` opcional.
- Evaluator: no necesita stack global para color.
- Elimina bugs de `restore` cuando el stack está desbalanceado.

**Esfuerzo**: ~5 horas (refactor toucha evaluator, parser, AST). **Impacto**: ⭐⭐⭐⭐⭐

### 1.10 Paletas temáticas

Conjuntos de colores pre-definidos.

**DSL**: `theme "ocean"; theme "sunset"; theme "forest";`

**Cambios**:

- Context: `Dictionary<string, Theme>` con colores predefinidos.
- Evaluator: al encontrar `theme`, establecer colores base.
- Renderer: usar los colores del tema activo.

**Esfuerzo**: ~2 horas. **Impacto**: ⭐⭐⭐

### 1.11 Color picker UI (Avalonia)

Selector visual de color en la interfaz.

**UI**: Botón que abre un `ColorPicker` (Avalonia built-in).

**Esfuerzo**: ~2 horas (en Avalonia). **Impacto**: ⭐⭐⭐

---

## 2. Mejoras al Lenguaje DSL

### 2.1 Comentarios

Cero comentarios en el DSL actual. Hace que los ejemplos parezcan código ofuscado.

**DSL**: `# esto es un comentario` o `// esto también`

**Cambios**:

- Solo lexer: ignorar líneas o segmentos que empiecen con `#` o `//`.
- Regex: `(#.*$|//.*$)` con opción `Multiline`.

**Esfuerzo**: 30 minutos. **Impacto**: ⭐⭐⭐⭐⭐

### 2.2 Semilla aleatoria (`seed`)

Sin esto, `randoms` y `samples` dan resultados distintos cada ejecución, imposibilitando tests deterministas y demos reproducibles.

**DSL**: `seed(42);`

**Cambios**:

- Lexer: nuevo token `seed`.
- Parser: nueva producción `SeedStmt`.
- Evaluator: `RandomProvider.Seed = value;` (reemplazar `new Random()` por singleton con seed configurable).

**Esfuerzo**: 1 hora. **Impacto**: ⭐⭐⭐⭐⭐

### 2.3 `print` / `debug`

Único output hoy es `MessageBox` con errores. Poder imprimir valores permite debuggear y demuestra uso del DSL.

**DSL**: `print measure(p1, p2);` o `print "Distancia calculada";`

**Cambios**:

- Context: `List<string> Output` para resultados de print.
- UI: consola o status bar que muestre los prints en tiempo real.

**Esfuerzo**: 2 horas. **Impacto**: ⭐⭐⭐⭐

### 2.4 Coordenadas relativas

Hoy solo coordenadas absolutas. Mover figuras relativamente es esencial.

**DSL**: `p1 + point(5, 0)` o `@p1 + (5, 0)`

**Cambios**:

- Si `Sum` ya funciona entre Points (revisar), solo documentar. Si no, implementar en evaluator.

**Esfuerzo**: 1-3 horas (depende de qué tan roto esté Sum). **Impacto**: ⭐⭐⭐

### 2.5 Constantes matemáticas adicionales

Hoy solo `PI` y `E`. Añadir estándares.

**DSL**: `phi` (golden ratio), `sqrt2`

**Cambios**:

- Lexer: nuevos tokens. Context: nuevas constantes.

**Esfuerzo**: 30 minutos. **Impacto**: ⭐⭐

### 2.6 Funciones matemáticas adicionales

Hoy: `sin`, `cos`, `sqrt`, `log`. Faltan.

**DSL**: `tan(x)`, `atan(x)`, `abs(x)`, `floor(x)`, `ceil(x)`, `round(x)`, `min(x,y)`, `max(x,y)`, `clamp(x,lo,hi)`

**Cambios**:

- Lexer: nuevos tokens. Context: añadir lambdas a `Trig_functions`.

**Esfuerzo**: 1 hora. **Impacto**: ⭐⭐⭐

### 2.7 Bucles y control de flujo

Hoy solo `if-then-else` como expresión. Sin loops.

**DSL**:
```
while(condition) { draw point(x, y); x := x + 1; }
for i in range(1, 10) { draw point(i, i^2); }
repeat(5) { draw point(random(), random()); }
```

**Cambios**:

- AST: nuevos nodos `While`, `For`, `Repeat`.
- Parser: nuevas producciones.
- Evaluator: implementar ejecución repetida con límite de seguridad (>1000 iteraciones = error).

**Esfuerzo**: ~6 horas. **Impacto**: ⭐⭐⭐⭐

### 2.8 Tipos de datos compuestos

**DSL**:
```
let t = (1, 2.5, "texto");               # tupla
let m = { "key1": 10, "key2": 20 };      # mapa/diccionario
```

**Cambios**:

- AST: nueva producción para tuplas y mapas.
- Context: soporte en el sistema de tipos.

**Esfuerzo**: ~8 horas (cambio profundo). **Impacto**: ⭐⭐⭐

---

## 3. Figuras Geométricas

### 3.1 Polígono regular

**DSL**: `polygon(center, radius, 6)` — hexágono; `polygon(center, radius, 3)` — triángulo.

**Cambios**:

- Nueva clase `Polygon : Figure`.
- Implementar `FigurePoints()` devolviendo N vértices.
- Intersección: delegar a segmentos del perímetro.
- Renderer: dibujar polígono (SkiaSharp `SKPath.AddPoly`).
- Parser: nueva producción.

**Esfuerzo**: ~4 horas. **Impacto**: ⭐⭐⭐⭐

### 3.2 Elipse

**DSL**: `ellipse(center, rx, ry)`

**Cambios**:

- Nueva clase `Ellipse : Figure`.
- Renderer: `canvas.DrawOval(rect, paint)`.
- Intersección: requiere resolver ecuación de elipse.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐

### 3.3 Texto / etiquetas en canvas

**DSL**: `label(point(1,2), "A")` — etiqueta simple; `label(point(1,2), "A", size=14, color=red)`

**Cambios**:

- Nueva clase `Label : Figure` (o tratarlo como draw especial).
- Renderer: `canvas.DrawText(text, x, y, paint)`.
- Parser: nueva producción.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐⭐

### 3.4 Estilos de línea

**DSL**: `style(line(p1,p2), dashed)` o `draw line(p1,p2) style dashed width 2`

**Cambios**:

- Nuevo tipo `LineStyle`: solid, dashed, dotted, dash-dot.
- Renderer: `SKPaint.PathEffect = SKPathEffect.CreateDash(...)` en SkiaSharp.
- Evaluator: almacenar estilo en `DrawObject`.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐

### 3.5 Relleno de figuras

**DSL**: `draw circle(...) fill solid(red);` o `draw circle(...) fill none;`

**Cambios**:

- `DrawObject` gana propiedad `FillStyle`.
- Renderer: dibujar relleno antes del stroke. En SkiaSharp, `paint.Style = SKPaintStyle.Fill`.
- Parser: `fill` como opción del `draw`.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐

---

## 4. Sistema de Dibujo y Canvas

### 4.1 Capas (z-order)

**DSL**: `layer 1; draw circle(...); layer 2; draw line(...);`

**Cambios**:

- Context: `List<List<DrawObject>> Layers`.
- Renderer: dibujar capas en orden ascendente.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐

### 4.2 Ocultar/mostrar figuras

**DSL**: `hide l;` / `show l;`

**Cambios**:

- Context: `List<string> HiddenFigures`.
- Evaluator/Renderer: al dibujar, skipear figuras ocultas.

**Esfuerzo**: 1 hora. **Impacto**: ⭐⭐⭐

### 4.3 Sistema de coordenadas (origen + escala)

**DSL**: `origin point(200, 200);` y `scale 2;`

**Cambios**:

- Context: `Point Origin` y `double Scale`.
- Renderer: transformar coordenadas del mundo a píxeles con origen y escala.

**Esfuerzo**: 2 horas. **Impacto**: ⭐⭐⭐⭐

### 4.4 Snap a grid

**DSL**: `snap 0.5;` — redondear coordenadas a múltiplos de 0.5.

**Cambios**:

- Evaluator: tras evaluar cada expresión numérica, redondear según snap activo.
- Context: `double SnapPrecision` (0 = desactivado).

**Esfuerzo**: 1 hora. **Impacto**: ⭐⭐

### 4.5 Animación paramétrica

**DSL**:
```
animate(t from 0 to 2*PI step 0.1) {
    draw point(t, sin(t));
    draw point(t, cos(t));
}
```

**Cambios**:

- AST: nuevo nodo `Animate`.
- Evaluator: ejecutar el bloque para cada valor de t, acumulando frames.
- Renderer: mostrar frames secuencialmente con timer/async.

**Esfuerzo**: ~6 horas. **Impacto**: ⭐⭐⭐⭐⭐

---

## 5. Performance y UX

### 5.1 Cancelación de secuencias infinitas

Hoy un `draw samples;` en una secuencia infinita congela la UI para siempre.

**Cambios**:

- `CancellationTokenSource` en el evaluator.
- Las secuencias infinitas (`InfinitePointSequence`, `InfiniteDoubleSequence`) aceptan `CancellationToken`.
- Botón "Stop" en la UI que llama `cts.Cancel()`.

**Esfuerzo**: ~3 horas. **Impacto**: ⭐⭐⭐⭐⭐

### 5.2 Evaluación incremental

Solo re-procesar líneas cambiadas del editor, no todo el archivo.

**Cambios**:

- Cachear AST por línea.
- `Diff` entre editor actual y último AST procesado.
- Solo re-evaluar líneas nuevas/modificadas.

**Esfuerzo**: ~8 horas (complejo). **Impacto**: ⭐⭐⭐

### 5.3 Barra de progreso

Para imports grandes o múltiples `draw` con secuencias largas.

**Cambios**:

- Evaluator emite eventos de progreso.
- UI: `ProgressBar` bindeada.

**Esfuerzo**: ~2 horas. **Impacto**: ⭐⭐

---

## 6. I/O y Ecosistema

### 6.1 Exportar dibujo a SVG

Vector, no raster. Escalable infinitamente, editable en Illustrator/Inkscape.

**Cambios**:

- Renderer alternativo que genera XML SVG en vez de dibujar en canvas.
- Figura por figura: `<circle>`, `<line>`, `<polygon>`, etc.

**Esfuerzo**: ~5 horas (nuevo renderer). **Impacto**: ⭐⭐⭐⭐⭐

### 6.2 Exportar a PDF

Documento vectorial imprimible.

**Cambios**:

- Usar `SkiaSharp.SKDocument.CreatePdf`.
- Mismo pipeline que SVG pero escribiendo a PDF stream.

**Esfuerzo**: ~3 horas sobre 6.1. **Impacto**: ⭐⭐⭐⭐

### 6.3 Importar/exportar formato estándar (DXF, GeoJSON)

Interoperabilidad con otras herramientas.

**DSL**: `import "drawing.dxf";` además de `import "file.geo";`

**Esfuerzo**: ~8+ horas por formato. **Impacto**: ⭐⭐⭐

### 6.4 Línea de comandos

Procesar .geo sin GUI.

**CLI**: `dotnet run --project Wall-E.Cli -- input.geo -o output.png`

**Cambios**:

- Nuevo proyecto `Wall-E.Cli` que referencia `Wall-E.Core`.
- Pipeline headless: parse → evaluate → render → save.

**Esfuerzo**: ~4 horas. **Impacto**: ⭐⭐⭐⭐

---

## 7. Resumen de Priorización

### Alto impacto, bajo esfuerzo (hacer primero)

| # | Mejora | Esfuerzo | Impacto |
|---|---|---|---|
| 1 | Comentarios en DSL `#` / `//` | 30 min | ⭐⭐⭐⭐⭐ |
| 2 | `seed(n)` para random determinista | 1 h | ⭐⭐⭐⭐⭐ |
| 3 | `color #hex;` (16M colores) | 2 h | ⭐⭐⭐⭐⭐ |
| 4 | Constantes extra (`phi`, `sqrt2`) | 30 min | ⭐⭐ |
| 5 | Funciones math extra (`tan`, `abs`, etc.) | 1 h | ⭐⭐⭐ |
| 6 | Cancelación de secuencias infinitas | 3 h | ⭐⭐⭐⭐⭐ |

### Alto impacto, esfuerzo medio

| # | Mejora | Esfuerzo | Impacto |
|---|---|---|---|
| 7 | `rgb(r,g,b)` / hex 8-digito alpha | 3 h | ⭐⭐⭐⭐ |
| 8 | `print` / `debug` | 2 h | ⭐⭐⭐⭐ |
| 9 | Color por-figura (arquitectura) | 5 h | ⭐⭐⭐⭐⭐ |
| 10 | Línea de comandos (CLI headless) | 4 h | ⭐⭐⭐⭐ |
| 11 | Sistema coordenadas origen+escala | 2 h | ⭐⭐⭐⭐ |
| 12 | Polygon (`polygon(center, r, n)`) | 4 h | ⭐⭐⭐⭐ |

### Alto impacto, esfuerzo alto

| # | Mejora | Esfuerzo | Impacto |
|---|---|---|---|
| 13 | Animación paramétrica | 6 h | ⭐⭐⭐⭐⭐ |
| 14 | Exportar SVG | 5 h | ⭐⭐⭐⭐⭐ |
| 15 | Bucles (`for`, `while`, `repeat`) | 6 h | ⭐⭐⭐⭐ |
| 16 | Gradientes en fill | 4 h | ⭐⭐⭐⭐ |
| 17 | Exportar PDF | 3 h (si ya hay SVG) | ⭐⭐⭐⭐ |

---

*Documento generado el 2026-07-01. Esfuerzos estimados para una persona con conocimiento del códigobase.*
