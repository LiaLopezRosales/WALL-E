using System.IO;
using System.Text.RegularExpressions;
public class GeneralLexer
{
    public string code{get;set;}
    List<List<Error>> errors{get;set;}
    public List<string> lines{get;private set;}
    public string File{get;set;}

    public GeneralLexer(string code,string file)
    {
        this.code=code;
        errors=new List<List<Error>>();
        string[] lines=code.Split(new[] {";"},StringSplitOptions.None);
        int index=-1;
        int amount_of_open_let=0;
        //Check this
        for (int i = 0; i < lines.Length; i++)
        {  
            if (index>=0 && !(lines[i].Contains("in")))
            {
                lines[index]=lines[index]+";"+lines[i];
                lines[i]="";
            }
            else if (index>=0 && (lines[i].Contains("in")))
            {
                lines[index]=lines[index]+";"+lines[i];
                
                amount_of_open_let--;
                if (amount_of_open_let==0)
                {
                    index=-1;
                }
                
                lines[i]="";
            }
            if (lines[i].Contains("let"))
            {
                int amount=Amount_of_Lets(lines[i]);
                
                index=i;
                amount_of_open_let=amount_of_open_let+amount;
                continue;
            }  
        }
        this.lines=new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i]==" " || lines[i]=="" || lines[i]=="  ")
            {
                continue;
            }
            else
            {
                this.lines.Add(lines[i]);
            }
        }

        File=file;
    }

    public List<List<Token>> Process(List<string> group_of_lines)
    {
        List<List<Token>> tokens=new List<List<Token>>();
        long count=0;
    
            foreach (var line in group_of_lines)
            {
            Lexer lexer=new Lexer(File,count.ToString());
            tokens.Add(lexer.Tokens(line));
            if (lexer.Lexic_Errors().Count>0)
            {
                errors.Add(lexer.Lexic_Errors());
            }
            count++;
            }
        return tokens;
    }

    public List<List<Error>> LexicalErrors()
    {
        return errors;
    }
    private int Amount_of_Lets(string s)
    {
        MatchCollection matches=Regex.Matches(s,"\\blet\\b",RegexOptions.IgnoreCase);
        return matches.Count;
    }
    
}