string test="x=3+4; EOL circle _ c1 p2 point sequence; EOL draw(p1); EOL";
Lexer lexer=new Lexer();
List<Token> tokens=lexer.Tokens(test);
foreach (var item in tokens)
{
    Console.WriteLine(item);
}