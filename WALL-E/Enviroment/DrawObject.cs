public class DrawObject
{
    public object Figures{get;set;}
    public string Tag{get;set;}
    public string UsedColor{get;set;}

    public DrawObject(object value,string tag,string color)
    {
        Figures=value;
        Tag=tag;
        UsedColor=color;
    }
    public bool CheckValidType()
    {
        if (Figures is Figure || Figures is Finite_Sequence<object> || Figures is InfinitePointSequence)
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