using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GeoWall_E
{
    public partial class Form1 : Form
    {
        private Graphics Papel { get; set; }
        private Pen Lapiz { get; set; }
        private SolidBrush Brush { get; set; }
        private HashSet<Point> PuntosGenerados { get; set; }
        public Context context { get; set; }
        public Form1()
        {
            InitializeComponent();
            Papel = pictureBox1.CreateGraphics();
            Lapiz = new Pen(Color.Black);
            Brush = new SolidBrush(Color.Black);
            PuntosGenerados = new HashSet<Point>();
            context = new Context();
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


            string errors = "";




            MessageBox.Show(errors, "Lexical Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            MessageBox.Show(errors, "Sintactic Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            MessageBox.Show(errors, "Semantic Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            


            
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

        public void Draw_Point()
        {
            Point p = Generar_Punto();

            // Dibujar el punto como un circulo
            Papel.FillEllipse(Brush, p.X, p.Y, 5, 5);
        }
        public void Draw_Point(string name)
        {
            Point p = Generar_Punto();

            // Dibujar el punto como un circulo
            Papel.FillEllipse(Brush, p.X, p.Y, 5, 5);

            //Mostrar el nombre del punto
            Papel.DrawString(name, this.Font, Brushes.Black, p.X + 5, p.Y - 5);
        }
        public void Draw_Point(Point p)
        {
            // Dibujar el punto como un circulo
            Papel.FillEllipse(Brush, p.X, p.Y, 5, 5);
        }
        public void Draw_Point(Point p, string name)
        {
            Draw_Point(p);
            //Mostrar el nombre del punto
            Papel.DrawString(name, this.Font, Brushes.Black, p.X + 5, p.Y - 5);
        }

        public void Draw_Segment()
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
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }
        public void Draw_Segment(Point p1, Point p2)
        {
            Papel.DrawLine(Lapiz, p1, p2);
        }
        public void Draw_Segment(Point p1, Point p2, string name)
        {
            Papel.DrawLine(Lapiz, p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }

        /*public void Draw_Line(Point p1, Point p2)
        {
            //Calcular la ecuacion de la recta que pasa por los dos puntos dados
            double m = (double)((p2.Y - p1.Y) / (p2.X - p1.X)); //Pendiente
            double b = p1.Y - m * p1.X; //Termino independiente

            //Encontrar los puntos de interseccion con los bordes del plano
            int xEnYMin = (int)((0 - b) / m); //Punto de intersecion con Y minimo (borde inferior)
            int xEnYMax = (int)((pictureBox1.Height - b) / m); //Punto de intersecion con Y maximo (borde superior)
            int yEnXMin = (int)(m * 0 + b); //Punto de intersecion con X minimo (borde izquierdo)
            int yENXMax = (int)(m * (pictureBox1.Width + b)); //Punto de intersecion con X maximo (borde derecho)

            //Dibujar la linea que pasa por los dos puntos y se extiende hasta los bordes del plano
            Papel.DrawLine(Lapiz, xEnYMin, 0, xEnYMax, pictureBox1.Height); //Linea vertical
            Papel.DrawLine(Lapiz, 0, yEnXMin, pictureBox1.Width, yENXMax); //Linea horizontal
        }
        public void Draw_Line(Point p1, Point p2, string name)
        {
            Draw_Line(p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }*/

        public void Draw_Line(Point p1, Point p2)
        {
            //Calcular la pendiente de la recta que pasa por p1 y p2
            double m = (double)(p2.Y - p1.Y) / (double)(p2.X - p1.X);

            //Calcular los puntos de interseccion con los bordes del pictureBox
            int xEnYMin = 0; //Punto de intersccion con Y minimo (borde inferior)
            int xEnYMax = pictureBox1.Width; //Punto de intersccion con Y maximo (borde superior)
            int yEnXMin = (int)(p1.Y - m * p1.X); //Punto de interseccion con X minimo (bode izquierdo)
            int yEnXMax = (int)(p1.Y + m * (pictureBox1.Width - p1.X)); //Punto de intersccion con X maximo (borde derecho)

            //Dibujar la linea que pasa por p1 y p2 y se extienede hasta el borde del plano
            Papel.DrawLine(Lapiz, p1.X, p1.Y, xEnYMin, yEnXMin);
            Papel.DrawLine(Lapiz, p1.X, p1.Y, xEnYMax, yEnXMax);
        }
        public void Draw_Line(Point p1, Point p2, string name)
        {
            Draw_Line(p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }

        public void Draw_Ray(Point p1, Point p2)
        {
            //Calcular la interseccion con el borde del pictureBox
            int xIntersect, yIntersect;
            if (p2.X - p1.X != 0) //Identificar si la pendinete es finita
            {
                double m = (double)(p2.Y - p1.Y) / (p2.X - p1.X); //Calcular la pendiente
                double b = p1.Y - m * p1.X; //Calcular el termino independiente
                if (m != 0)
                {
                    xIntersect = p2.X > p1.X ? pictureBox1.Width : 0; //Interseccion con el borde vertical
                    yIntersect = (int)(m * xIntersect + b); //Coordenada Y correspondiente
                }
                else //m = 0 => la pendendiente es horizontal
                {
                    xIntersect = p2.X > p1.X ? pictureBox1.Width : 0; //Insterseccion en el borde horizontal
                    yIntersect = p1.Y; //La coordena Y es la misma que la del punto p1
                }
            }
            else //La pendiente es infinita, la semirrecta es vertical
            {
                xIntersect = p1.X;
                yIntersect = p2.Y > p1.Y ? pictureBox1.Height : 0; //Interseccion en el bode vertical
            }

            Point graph = new Point(xIntersect, yIntersect);

            Papel.DrawLine(Lapiz, p1, graph);
        }
        public void Draw_Ray(Point p1, Point p2, string name)
        {
            Draw_Ray(p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }

        public void Draw_Circule(Point p1, Point p2)
        {
            //El punto p1 es el centro y p2 se encuentra en el borde
            double radio = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

            Papel.DrawEllipse(Lapiz, (float)(p1.X - radio), (float)(p1.Y - radio), (float)(2 * radio), (float)(2 * radio));
        }
        public void Draw_Circule(Point p1, Point p2, string name)
        {
            Draw_Circule(p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }
        public void Draw_Circule()
        {
            Point p1 = Generar_Punto();
            Random r = new Random();
            int size = r.Next(30, 60);

            Papel.DrawEllipse(Lapiz, p1.X, p1.Y, size, size);
        }
        public void Draw_Circule(Point c, int radio)
        {
            //Calcular las coordenadas del rect'angulo que contiene el circulo
            int x = c.X - radio;
            int y = c.Y - radio;
            int m = 2 * radio;

            Papel.DrawEllipse(Lapiz, x, y, m, m);
        }
        public void Draw_Circule(Point c, int radio, string name)
        {
            Draw_Circule(c, radio);
            Papel.DrawString(name, this.Font, Brushes.Black, c.X + 5, c.Y - 5);
        }


        public void Draw_Curve(Point center, Point p1, Point p2)
        {
            //Calcular el rectangulo que contiene el arco
            int radio = (int)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            int x = center.X - radio;
            int y = center.Y - radio;
            int d = 2 * radio;

            //Calcular el angulo inicial y el angulo que abarca el arco
            float anguloInicial = (float)(180 / Math.PI * Math.Atan2(p1.Y - center.Y, p1.X - center.X));
            float anguloFinal = (float)(180 / Math.PI * Math.Atan2(p2.Y - center.Y, p2.X - center.X));

            Papel.DrawArc(Lapiz, x, y, d, d, anguloInicial, anguloFinal - anguloInicial);
        }
        public void Draw_Curve(Point c, Point p1, Point p2, string name)
        {
            Draw_Curve(c, p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, p1.X + 5, p1.Y - 5);
        }


        public void Draw_Arc(Point c, int radio, Point p1, Point p2)
        {
            //Calcular el angulo entre cada semirrecta y el eje
            double a1 = Math.Atan2(p1.Y - c.Y, p1.X - c.X) * (180 / Math.PI);
            double a2 = Math.Atan2(p2.Y - c.Y, p2.X - c.X) * (180 / Math.PI);

            //Calcular el angulo inicialy el final del arco en sentido de las manecillas del relor
            double aInit = Math.Min(a1, a2);
            double aEnd = Math.Max(a1, a2);

            int d = 2 * radio;

            Papel.DrawArc(Lapiz, c.X - radio, c.Y - radio, d, d, (float)aInit, (float)(aEnd - aInit));
        }
        public void Draw_Arc(Point c, int radio, Point p1, Point p2, string name)
        {
            Draw_Arc(c, radio, p1, p2);
            Papel.DrawString(name, this.Font, Brushes.Black, c.X + 5, c.Y - 15);
        }

       
    }
}
