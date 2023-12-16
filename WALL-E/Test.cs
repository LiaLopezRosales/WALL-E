//string test="point p1; circle c1; draw{p1,c1}; color yellow; let a=4 ; b=5 ;in a+b; if let a=4; b=5;in a+b then 1 else 2;";
string test="point p1; point p2; color blue; draw line(p1,p2); restore; draw p1; draw p2;";
GeneralLexer l=new GeneralLexer(test,"MainFile");
List<string> r=l.lines;
// foreach (var item in r)
// {
//     Console.WriteLine(item);
// }
List<List<Token>> p=l.Process(r);
Console.WriteLine(p.Count);
int index=0;
foreach (var item in p)
{
    Console.WriteLine("{0} tokens in line {1}",item.Count,index);
    foreach (var line in item)
    {
        Console.WriteLine(line);
    }
    index++;
}
GeneralParser e=new GeneralParser(p,"MainFile");
List<Node> f=e.ParseArchive();
List<List<Error>> t=e.ParserErrors();
if (t.Count>0)
{
    foreach (var item in t)
    {
        foreach (var error in item)
        {
            Console.WriteLine(error);
        }
    }
}
else
{
    int count=0;
    foreach (var item in f)
    {
        Console.WriteLine(item.Type);
        Console.WriteLine(count);
        Console.WriteLine("level 1");
        foreach (var item2 in item.Branches)
        {
            Console.WriteLine(item2.Type);
            Console.WriteLine("level 2");
            foreach (var item3 in item2.Branches)
        {
            Console.WriteLine(item3.Type);
            Console.WriteLine("level 3");
            

        }

        }
        count++;
    }
}
GeneralEvaluation w=new GeneralEvaluation(f,"MainFile");
Context c=w.EvaluateArchive(new Context());
List<List<Error>> v=w.Semantic_Errors();
foreach (var item in v)
{
    foreach (var a in item)
    {
        Console.WriteLine(a);
    }
}
// foreach (var item in c.ToDraw)
// {
//     Console.WriteLine(item);
// }
// while (c.UtilizedColors.Count>0)
// {
//     Console.WriteLine(c.UtilizedColors.Pop());
// }