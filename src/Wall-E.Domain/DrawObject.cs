namespace Wall_E.Domain;

/// <summary>Represents a drawable object wrapping a figure or sequence with rendering metadata.</summary>
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
    public int Layer{get;set;}

    public bool IsFilled => FillType == FillType.Solid;

    /// <summary>Creates a DrawObject with default line style and fill.</summary>
    public DrawObject(object value,string tag,string color)
    {
        Figures=value;
        Tag=tag;
        UsedColor=color;
        LineStyle=LineStyle.Solid;
        StrokeWidth=1.0;
        FillType=FillType.None;
    }

    /// <summary>Creates a DrawObject with full rendering options including line style, fill, and layer.</summary>
    public DrawObject(object value,string tag,string color,LineStyle style,double width,FillType fill,
        string grad1="",string grad2="",int layer=0)
    {
        Figures=value;
        Tag=tag;
        UsedColor=color;
        LineStyle=style;
        StrokeWidth=width;
        FillType=fill;
        GradientColor1=grad1;
        GradientColor2=grad2;
        Layer=layer;
    }
    //Método que revisa si el objeto es válido para gráficar(solo figuras y secuencias de figuras)
    /// <summary>Returns true if the wrapped figure value is a valid drawable type.</summary>
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
    /// <summary>Returns true if the given object is a valid drawable type (figure or sequence of figures).</summary>
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