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
        lines=code.Split(new[] {";"},StringSplitOptions.None);
        int index=-1;
        //Check this
        for (int i = 0; i < lines.Length; i++)
        {  
            if (index>=0 && !(lines[i].Contains("in")))
            {
                lines[index]=lines[index]+" "+lines[i];
            }
            else if (index>=0 && (lines[i].Contains("in")))
            {
                lines[index]=lines[index]+" "+lines[i];
                index=-1;
            }
            else if (lines[i].Contains("let"))
            {
                index=i;
                continue;
            }

            
        }
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