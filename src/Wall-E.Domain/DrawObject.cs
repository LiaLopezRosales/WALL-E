namespace Wall_E.Domain;
public class DrawObject
{//Clase que define un objeto a gráficar
    //Contiene la figura(cada figura contiene los datos necesarios para su caracterización)
    public object Figures{get;set;}
    //Etiqueta opcional que debe aparecer alrededor de la figura(en caso de no tener ninguna será un string vacío)
    public string Tag{get;set;}
    //Color que tenía la "brocha" en el momento que se indica su graficación 
    public string UsedColor{get;set;}
    public LineStyle LineStyle{get;set;}
    public double StrokeWidth{get;set;}
    public FillType FillType{get;set;}
    public string GradientColor1{get;set;} = "";
    public string GradientColor2{get;set;} = "";

    public bool IsFilled => FillType == FillType.Solid;

    public DrawObject(object value,string tag,string color)
    {
        Figures=value;
        Tag=tag;
        UsedColor=color;
        LineStyle=LineStyle.Solid;
        StrokeWidth=1.0;
        FillType=FillType.None;
    }

    public DrawObject(object value,string tag,string color,LineStyle style,double width,FillType fill,
        string grad1="",string grad2="")
    {
        Figures=value;
        Tag=tag;
        UsedColor=color;
        LineStyle=style;
        StrokeWidth=width;
        FillType=fill;
        GradientColor1=grad1;
        GradientColor2=grad2;
    }
    //Método que revisa si el objeto es válido para gráficar(solo figuras y secuencias de figuras)
    public bool CheckValidType()
    {
        if (Figures is Figure || Figures is Finite_Sequence<object> ||Figures is Finite_Sequence<Point>|| Figures is InfinitePointSequence)
        {
            if (Figures is Finite_Sequence<object>)
            {
                foreach (var item in ((Finite_Sequence<object>)Figures).Sequence)
                {
                    if (!(DrawObject.CheckValidDrawType(item)))
                    {
                        return false;
                    }
                }
                return true;
            }
            else return true;
        }
        else return false;
    }
    //Auxiliar del método anterior para comprobar que los elementos de una secuencia sean válidos
    public static bool CheckValidDrawType(object x)
    {
        if (x is Figure || x is Finite_Sequence<object> || x is InfinitePointSequence)
        {
            if (x is Finite_Sequence<object>)
            {

                foreach (var item in ((Finite_Sequence<object>)x).Sequence)
                {
                    if (!(DrawObject.CheckValidDrawType(item)))
                    {
                        return false;
                    }
                }
                return true;
            }
            else return true;
        }
        else return false;
    }
     public override string ToString() => string.Format("{0} {1} in {2}",Figures,Tag,UsedColor);
}