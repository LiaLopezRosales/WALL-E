using System;
using System.Drawing;

namespace Wall_E
{
    public partial class Form1 : Form
    {
        private Graphics Papel { get; set; }
        private Pen Lapiz { get; set; }
        private SolidBrush Brush { get; set; }
        private HashSet<Point> PuntosGenerados { get; set; }
        private Context Context { get; set; }
        public Form1()
        {
            InitializeComponent();
            Papel = pictureBox1.CreateGraphics();
            Lapiz = new Pen(Color.Black);
            Brush = new SolidBrush(Color.Black);
            PuntosGenerados = new HashSet<Point>();
            Context = new Context();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Papel.Clear(Color.Silver);
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            string command = Commands.Text;

            if (command == "")
            {
                MessageBox.Show("Please introduce al least one line of code in the TextBox", "Suggestion", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            List<List<Error>> errors;
            string errorsToPrint = "";




            //Ver como paso el File q es el segundo parametro de GeneralLexer
            GeneralLexer startlexing = new GeneralLexer(command, null);
            List<List<Token>> tokenizedcode = startlexing.Process(startlexing.lines);
            errors = startlexing.LexicalErrors();
            //Se encuentra errores léxicos los imprime e interrumpe el proceso
            if (errors.Count > 0)
            {
                foreach (var item in errors)
                {
                    foreach (var error in item)
                    {
                        errorsToPrint += String.Format("{0}, {1}, {2} \n", error.Code, error.Argument, error.location);
                    }
                }
                MessageBox.Show(errorsToPrint, "Lexical Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //De lo contrario procede a realizar el análisis sintáctico
            else
            {   //Se realizan procesos similares al léxico para el parser y la evaluación 
                GeneralParser startparsing = new GeneralParser(tokenizedcode, null);
                List<Node> parsedcode = startparsing.ParseArchive();
                errors = startparsing.ParserErrors();
                if (errors.Count > 0)
                {
                    foreach (var item in errors)
                    {
                        foreach (var error in item)
                        {
                            errorsToPrint += String.Format("{0}, {1}, {2} \n", error.Code, error.Argument, error.location);
                        }
                    }
                    MessageBox.Show(errorsToPrint, "Syntactic Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    GeneralEvaluation startevaluation = new GeneralEvaluation(parsedcode, null);
                    Context result = startevaluation.EvaluateArchive(Context);
                    errors = startevaluation.Semantic_Errors();
                    if (errors.Count > 0)
                    {
                        foreach (var item in errors)
                        {
                            foreach (var error in item)
                            {
                                errorsToPrint += String.Format("{0}, {1}, {2} \n", error.Code, error.Argument, error.location);
                            }
                        }
                        MessageBox.Show(errorsToPrint, "Semantic Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        List<DrawObject> drawObjects = Context.ToDraw;
                        if (drawObjects.Count != 0)
                            DrawFigures(drawObjects);
                    }
                }
            }
            Context.ToDraw.Clear();
        }

        public void DrawFigures(List<DrawObject> draw)
        {
            foreach (var item in draw)
            {
                string color = item.UsedColor;
                string tag = item.Tag;
                object figure = item.Figures;

                Change_Color(color);

                if (figure is Point p)
                {
                    Draw_Point(p, tag);
                }
                else if (figure is Segment s)
                {
                    Draw_Segment(s.StartIn, s.EndsIn, tag);
                }
                else if (figure is Line l)
                {
                    Draw_Line(l.generalpoint1, l.generalpoint2, tag);
                }
                else if (figure is Ray r)
                {
                    Draw_Ray(r.StartIn, r.PassFor, tag);
                }
                else if (figure is Circle c)
                {
                    Draw_Circule(c.center, c.radio, tag);
                }
                else if (figure is Arc a)
                {
                    Draw_Arc(a.center, a.measure, a.point_of_semirect1, a.point_of_semirect2, tag);
                }
                else if (figure is Finite_Sequence<object> f)
                {
                    //...
                }
            }

        }


        public void Change_Color(string color)
        {
            switch (color.ToLower())
            {
                case "black":
                    Lapiz.Color = Color.Black;
                    Brush.Color = Color.Black;
                    break;
                case "white":
                    Lapiz.Color = Color.White;
                    Brush.Color = Color.White;
                    break;
                case "gray":
                    Lapiz.Color = Color.Gray;
                    Brush.Color = Color.Gray;
                    break;
                case "red":
                    Lapiz.Color = Color.Red;
                    Brush.Color = Color.Red;
                    break;
                case "blue":
                    Lapiz.Color = Color.Blue;
                    Brush.Color = Color.Blue;
                    break;
                case "yellow":
                    Lapiz.Color = Color.Yellow;
                    Brush.Color = Color.Yellow;
                    break;
                case "green":
                    Lapiz.Color = Color.Green;
                    Brush.Color = Color.Green;
                    break;
                case "cyan":
                    Lapiz.Color = Color.Cyan;
                    Brush.Color = Color.Cyan;
                    break;
                case "magenta":
                    Lapiz.Color = Color.Magenta;
                    Brush.Color = Color.Magenta;
                    break;
                default:
                    break;
            }
        }

        private Point Generar_Punto()
        {
            Random random = new Random();
            Point nuevoPunto;

            do
            {
                int x = random.Next(50, pictureBox1.Width - 50);
                int y = random.Next(50, pictureBox1.Height - 50);
                nuevoPunto = new Point(x, y);
            } while (PuntosGenerados.Contains(nuevoPunto));

            PuntosGenerados.Add(nuevoPunto);

            return nuevoPunto;
        }

        //Este codigo se encuentra comentado porque para las funcionalidades del proyecto en general no es necesario, pero funciona bien para futuras mejoras
        /*public void Draw_Point()
        {
            Point p = Generar_Punto();

            // Dibujar el punto como un circulo
            Papel.FillEllipse(Brush, (float)p.x, (float)p.y, 5, 5);
        }
        public void Draw_Point(string name)
        {
            Point p = Generar_Punto();

            // Dibujar el punto como un circulo
            Papel.FillEllipse(Brush, (float)p.x, (float)p.y, 5, 5);

            //Mostrar el nombre del punto
            Papel.DrawString(name, this.Font, Brushes.Black, (float)p.x + 5, (float)p.y - 5);
        }*/

        private void Draw_Point(Point p)
        {
            // Dibujar el punto como un circulo
            Papel.FillEllipse(Brush, (float)p.x, (float)p.y, 5, 5);
        }
        public void Draw_Point(Point p, string name)
        {
            Draw_Point(p);
            if (name != "")
                Papel.DrawString(name, this.Font, Brushes.Black, (float)p.x + 5, (float)p.y - 5);
        }

        //Creacion de segmentos sin dar puntos de referencia
        /* public void Draw_Segment()
        {
            Point p1 = Generar_Punto();
            Point p2 = Generar_Punto();
            Papel.DrawLine(Lapiz, p1, p2);
        }
        public void Draw_Segment(string name)
        {
            Point p1 = Generar_Punto();
            Point p2 = Generar_Punto();
            Papel.DrawLine(Lapiz, p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, (float)p1.x + 5, (float)p1.y - 5);
        } */

        private void Draw_Segment(Point p1, Point p2)
        {
            Papel.DrawLine(Lapiz, (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y);
        }
        public void Draw_Segment(Point p1, Point p2, string name)
        {
            Papel.DrawLine(Lapiz, (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y);
            
            if (name != "")
                Papel.DrawString(name, this.Font, Brushes.Black, (float)p1.x + 5, (float)p1.y - 5);
        }

        public void Draw_Line(Point p1, Point p2)
        {
            //Calcular la pendiente de la recta que pasa por p1 y p2
            double m = (double)(p2.y - p1.y) / (double)(p2.x - p1.x);

            //Calcular los puntos de interseccion con los bordes del pictureBox
            int xEnYMin = 0; //Punto de intersccion con Y minimo (borde inferior)
            int xEnYMax = pictureBox1.Width; //Punto de intersccion con Y maximo (borde superior)
            int yEnXMin = (int)(p1.y - m * p1.x); //Punto de interseccion con X minimo (bode izquierdo)
            int yEnXMax = (int)(p1.y + m * (pictureBox1.Width - p1.x)); //Punto de intersccion con X maximo (borde derecho)

            //Dibujar la linea que pasa por p1 y p2 y se extienede hasta el borde del plano
            Papel.DrawLine(Lapiz, (float)p1.x, (float)p1.y, xEnYMin, yEnXMin);
            Papel.DrawLine(Lapiz, (float)p1.x, (float)p1.y, xEnYMax, yEnXMax);
        }
        public void Draw_Line(Point p1, Point p2, string name)
        {
            Draw_Line(p1, p2);
            
            if (name != "")
                Papel.DrawString(name, this.Font, Brushes.Black, (float)p1.x + 5, (float)p1.y - 5);
        }

        public void Draw_Ray(Point p1, Point p2)
        {
            //Calcular la interseccion con el borde del pictureBox
            double xIntersect, yIntersect;
            if (p2.x - p1.x != 0) //Identificar si la pendinete es finita
            {
                double m = (double)(p2.y - p1.y) / (p2.x - p1.x); //Calcular la pendiente
                double b = p1.y - m * p1.x; //Calcular el termino independiente
                if (m != 0)
                {
                    xIntersect = p2.x > p1.x ? pictureBox1.Width : 0; //Interseccion con el borde vertical
                    yIntersect = (int)(m * xIntersect + b); //Coordenada Y correspondiente
                }
                else //m = 0 => la pendendiente es horizontal
                {
                    xIntersect = p2.x > p1.x ? pictureBox1.Width : 0; //Insterseccion en el borde horizontal
                    yIntersect = p1.y; //La coordena Y es la misma que la del punto p1
                }
            }
            else //La pendiente es infinita, la semirrecta es vertical
            {
                xIntersect = p1.x;
                yIntersect = p2.y > p1.y ? pictureBox1.Height : 0; //Interseccion en el bode vertical
            }

            Point graph = new(xIntersect, yIntersect);

            Papel.DrawLine(Lapiz, (float)p1.x, (float)p1.y, (float)graph.x, (float)graph.y);
        }
        public void Draw_Ray(Point p1, Point p2, string name)
        {
            Draw_Ray(p1, p2);
            
            if (name != "")
                Papel.DrawString(name, this.Font, Brushes.Black, (float)p1.x + 5, (float)p1.y - 5);
        }

        // Formas de dibujar el circulo a partir de distintos datos
        /* public void Draw_Circule(Point p1, Point p2)
        {
            //El punto p1 es el centro y p2 se encuentra en el borde
            double radio = Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));

            Papel.DrawEllipse(Lapiz, (float)(p1.x - radio), (float)(p1.y - radio), (float)(2 * radio), (float)(2 * radio));
        }
        public void Draw_Circule(Point p1, Point p2, string name)
        {
            Draw_Circule(p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, (float)p1.x + 5, (float)p1.y - 5);
        }
        public void Draw_Circule()
        {
            Point p1 = Generar_Punto();
            Random r = new Random();
            int size = r.Next(30, 60);

            Papel.DrawEllipse(Lapiz, (float)p1.x, (float)p1.y, size, size);
        }*/

        public void Draw_Circule(Point c, double radio)
        {
            //Calcular las coordenadas del rect'angulo que contiene el circulo
            float x = (float)c.x - (float)radio;
            float y = (float)c.y - (float)radio;
            float m = 2 * (float)radio;

            Papel.DrawEllipse(Lapiz, x, y, m, m);
        }
        public void Draw_Circule(Point c, double radio, string name)
        {
            Draw_Circule(c, radio);

            if (name != "")
                Papel.DrawString(name, this.Font, Brushes.Black, (float)c.x + 5, (float)c.y - 5);
        }

        public void Draw_Arc(Point c, double radio, Point p1, Point p2)
        {
            //Calcular el angulo entre cada semirrecta y el eje
            double a1 = Math.Atan2(p1.y - c.y, p1.x - c.x) * (180 / Math.PI);
            double a2 = Math.Atan2(p2.y - c.y, p2.x - c.x) * (180 / Math.PI);

            //Calcular el angulo inicialy el final del arco en sentido de las manecillas del relor
            double aInit = Math.Min(a1, a2);
            double aEnd = Math.Max(a1, a2);

            double d = 2 * radio;

            Color color = Lapiz.Color;

            Change_Color("black");
            Draw_Point(c);
            Draw_Point(p1);
            Draw_Point(p2);

            Change_Color("cyan");
            Draw_Segment(c, p1);
            Draw_Segment(c, p2);

            Change_Color("gray");
            Draw_Circule(c, 35);

            Lapiz.Color = color;

            Papel.DrawArc(Lapiz, (float)c.x - (float)radio, (float)c.y - (float)radio, (float)d, (float)d, (float)aInit, (float)(aEnd - aInit));
        }
        public void Draw_Arc(Point c, double radio, Point p1, Point p2, string name)
        {
            Draw_Arc(c, radio, p1, p2);

            if (name != "")
                Papel.DrawString(name, this.Font, Brushes.Black, (float)c.x + 5, (float)c.y - 15);
        }
    }
}