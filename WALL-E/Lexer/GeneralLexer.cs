using System.IO;
public class GeneralLexer
{
    public string code{get;set;}
    List<List<Error>> errors{get;set;}
    string[] lines{get;set;}
    public string File{get;set;}

    public GeneralLexer(string code,string file)
    {
        this.code=code;
        errors=new List<List<Error>>();
        lines=code.Split(new[] {";"},StringSplitOptions.RemoveEmptyEntries);
        File=file;
    }

    public List<List<Token>> Process(string[] group_of_lines)
    {
        List<List<Token>> tokens=new List<List<Token>>();
        for (int i = 0; i < group_of_lines.Length; i++)
        {
            Lexer lexer=new Lexer(File,i.ToString());
            tokens.Add(lexer.Tokens(group_of_lines[i]));
            if (lexer.Lexic_Errors().Count>0)
            {
                errors.Add(lexer.Lexic_Errors());
            }
        }
        return tokens;
    }

    public List<List<Error>> LexicalErrors()
    {
        return errors;
    }
    
}