public class GeneralParser
{
    List<List<Token>> lines{get;set;}
    List<List<Error>> all_the_errors{get;set;}

    public GeneralParser(List<List<Token>> lexers)
    {
        lines=lexers;
        all_the_errors=new List<List<Error>>();
    }

    public List<Node> ParseArchive()
    {
        List<Node> Trees=new List<Node>();
        foreach (List<Token> tokens in lines)
        {
            Parser temp_parser=new Parser(tokens);
            Node line=temp_parser.Parse();
            Trees.Add(line);
            if (temp_parser.Syntactic_Errors().Count>0)
            {
                all_the_errors.Add(temp_parser.Syntactic_Errors());
            }
            
        }
        return Trees;
    }
    public List<List<Error>> ParserErrors()
    {
        return all_the_errors;
    }
}