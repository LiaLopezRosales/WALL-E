//string test="point p1; circle c1; draw{p1,c1}; color yellow; let a=4 ; b=5 ;in a+b; if let a=4; b=5;in a+b then 1 else 2;";
//string test="Fib(n) = if n<=1 then 1 else Fib(n - 1) + Fib(n - 2);Fib(5);";
//string test="Sum(x,y,z)=x+y - z;Sum(2,3,2)*Sum(2,4,2);";
//string test="let a=let b=4; in b+2; in a+5;";
//string test="a,b,_=randoms(); c,d,_=randoms();";
//string test="a,rest={1,2,3}+undefined;";
string test="import \"try.geo\";";
//  Finite_Sequence<object> seq=new Finite_Sequence<object>(new List<object>(){1,2,3});
//  object firstvalue=seq.ReturnValue();
//  object secondvalue=seq.ReturnValue();
//  Console.WriteLine(firstvalue);
//  Console.WriteLine(secondvalue);
//  foreach (var item in seq.Sequence)
//  {
//     Console.WriteLine(item);
//  }
GeneralLexer l=new GeneralLexer(test,"MainFile");
List<string> r=l.lines;
foreach (var item in r)
{
    Console.WriteLine(item);
}
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
            foreach (var item4 in item3.Branches)
        {
            Console.WriteLine(item4.Type);
            Console.WriteLine("level 4");
            

        }

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
foreach (var item in c.GlobalConstant)
{
    Console.WriteLine(item.Key);
    if (item.Value is Circle)
    {
        Console.WriteLine(item.Value);
    }
    else if (item.Value is Finite_Sequence<object>)
    {
        foreach (var item1 in ((Finite_Sequence<object>)item.Value).Sequence)
        {
            Console.WriteLine(item1);
        }
    }
    else Console.WriteLine(item.Value);
}
// foreach (var item in c.GlobalConstant)
// {
//     if (item.Value is List<Point>)
//     {
//         Console.WriteLine(item.Key);
//         foreach (var item1 in ((List<Point>)item.Value))
//         {
//             Console.WriteLine(item1);
//         }
//     }
// }
// foreach (var item in c.ToDraw)
// {
//     Console.WriteLine(item);
// }
// while (c.UtilizedColors.Count>0)
// {
//     Console.WriteLine(c.UtilizedColors.Pop());
// }