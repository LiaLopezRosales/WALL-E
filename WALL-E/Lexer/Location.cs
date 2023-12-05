public class Location
{
    public string File{get;set;}
    public string Line{get;set;}
    public string Column{get;set;}

    public Location(string file,string line,string column)
    {
        File=file;
        Line=line;
        Column=column;
    }

     public override string ToString() => string.Format("at {0}, {1}, {2}",File,Line,Column);
}