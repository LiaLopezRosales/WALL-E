# GeoWall-E

Es una biblioteca para graficar elementos geometricos como puntos, segmentos y circunferencias. En nuestro proyecto la interfaz gráfica se desarrolla en WindowsForm la que presenta un espacio para graficos o pizarra, un recuadro para introducir los comandos y tres botones en el espacio principal cuyas funciones se especifican más adelante. Esperamos que les resulte útil.


Funcionamiento básico de GeoWall-E:
1.	Abrir la aplicación de WindowsForm: Esto se puede hacer ejecutando el programa desde un editor de código o ejecutando desde la consola de comandos el script Wall-E.sh
2.	Escribir el programa que se desea correr en la caja de texto: no debe dejarse la caja de texto vacía, en cuyo caso se le notificará de este incidente, tampoco debe de dejarse una línea final en blanco ya que se esperará un nuevo comando y al no obtenerlo se notificará como un error.
2.1	Importar archivos: para que se ejecute correctamente, el archivo a importar debe encontrarse en \WALL-E\GeoLibrary y tener como identificador la extensión “.geo”, puede encontrarse en una subcarpeta pero el nombre de este archivo debe ser único para poder importarse.
3.	Comenzar su procesamiento mediante el botón de comando: una vez finalizada la escritura de los comandos debe presionar el botón “Prosess Commands” para que como indica se ejecute el intérprete de Wall-E.
4.	Cerrar el programa o escribir una nueva lista de comandos: Una vez finalizado el procesamiento de los comandos introducidos (incluyendo la visualización de las figuras geométricas), se puede analizar nuevo código no relacionado con el anterir sin necesidad de volver a iniciar el proyecto, o cerrar el programa mediante el botón “X” en la esquina superior derecha de la ventana.

Características de la aplicación de WindowsForm:
- El botón “Clean” es el encargado de limpiar la pizarra de dibujo, por lo que se pueden seguir dibujando figuras una encima de las otras mientras se quiera.
- El botón “Jump seq” tiene como funcionalidad detener la impresión de una secuencia infinita sin tener que cerrar completamente el programa.
- Los cuadros de mensajes detienen el ciclo de ejecuciones, por lo que para dar por concluido el procesamiento de un conjunto de comandos hay que cerrar la última de estas ventanas emergentes para que se limpie el TextBox.
- En caso de que sea necesario imprimir en la pizarra algún objeto esta es la última acción que se realizará, de no ser así se imprimirán en una ventana emergente los elementos que fueron procesados y de haber errores se mostrarán los que fueron procesados correctamente antes de encontrar el error (esto incluye si se analizaron figuras a dibujar aunque esta acción no haya sido ejecutada).

Especificaciones sobre el intérprete de GeoWall-E:
- En el caso de los puntos a los que el usuario le pone coordenadas, estas deben encontrarse en un rango de 50 a 310 para ambos elementos y la diferencia de un punto a otro debe de ser de al menos 15 unidades para que se vea su diferencia, así mismo en caso de dar directamente una medida al radio de una circunferencia es preferible que esta sea mayor a 25 unidades para que sea completamente visible la figura trazada.
- A la hora de introducir operaciones matemáticas se debe dejar el espacio entre el símbolo ' - ' y el siguiente número
- La incorrecta colocación del símbolo “;” no sé detecta por si sola como un error, pero es la que determina el final de una instrucción por lo tanto su mala colocación ocasionará que se intenten procesar instrucciones invalidas y se detectará dicho error.
- La concatenación de una secuencia y “undefined” en el siguiente orden: {secuencia cualquiera} + undefined devolverá la primera secuencia ya que la concatenación hace una nueva secuencia que devuelva primero los valores de la primera secuencia y luego los de la segunda, como el valor automático luego que se acaba una secuencia es undefined no es necesario cambiar o sustituir una secuencia que ya cumple lo pedido por la concatenación (la operación se ejecuta pero la secuencia devuelta se comporta exactamente igual a la primera).
- Cuando se devuelven los errores encontrados se informa de su tipo, cuál fue el error (pequeña explicación) y su localización. La localización consta de archivo, línea y columna(# del token donde se encontró el problema) aunque los errores semánticos detectados solo informarán de archivo y línea pues el orden de tokens ya no se tiene como referencia para poder determinar en que columna tuvo el error.
- El procesamiento se hace por etapas y en cada etapa de hace línea por línea, es decir se tokeniza línea por línea y si no hallan errores en ninguna de estas se prosigue a parsear una lista de listas de tokens y así sucesivamente.
- Al evaluar cada línea se informa algo: si la instrucción tiene valor de retorno (es expresión) se informa del valor obtenido y si no tiene se informa de lo ejecutado por esa acción, por ejemplo:
       - color blue; Color changed to blue
       - point p1; Point created
