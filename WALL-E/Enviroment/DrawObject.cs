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
        throw new NotImplementedException();
    }
}