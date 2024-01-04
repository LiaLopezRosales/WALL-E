using System.IO;
using System.Text.RegularExpressions;
public class GeneralLexer
{    //Divide un código en expresiones y declaraciones
    public string code{get;set;}
    List<List<Error>> errors{get;set;}
    public List<string> lines{get;private set;}
    public string File{get;set;}

    public GeneralLexer(string code,string file)
    {
        this.code=code;
        errors=new List<List<Error>>();
        //Se dividen las expresiones por ;
        string[] lines=code.Split(new[] {";"},StringSplitOptions.RemoveEmptyEntries);
        int index=-1;
        int amount_of_open_let=0;
        //Para tratar el caso especial del let-in se recorren las expresiones separadas existentes
        for (int i = 0; i < lines.Length; i++)
        {  
            //Si el índice es mayor que 0 hay un let abierto  se concatenan las dos líneas
            if (index>=0 && !(ContainIn(lines[i])))
            {
                if (lines[i].Contains("let ")|| lines[i].Contains("let"))
                {
                    int amount=Amount_of_Lets(lines[i]);
                    amount_of_open_let=amount_of_open_let+amount;
                }
                lines[index]=lines[index]+";"+lines[i];
                lines[i]="";
            }
             //Si hay lets abiertos y la línea contiene a 'in' se concatena la línea y se cierra un let
             if (index>=0 && (ContainIn(lines[i])))
            {
                
                lines[index]=lines[index]+";"+lines[i];
                
                amount_of_open_let--;
                amount_of_open_let=amount_of_open_let+Amount_of_Lets(lines[i]);
                //Si no hay más lets abiertos se coloca el índice en -1
                if (amount_of_open_let==0)
                {
                    index=-1;
                }
               
                lines[i]="";
            }
            //Si una linea contiene expresión let,se busca cuantos hay y se modifica el índice y la cantidad de lets abiertos
            if (lines[i].Contains("let ") || lines[i].Contains("let"))
            {
                int amount=Amount_of_Lets(lines[i]);
                if (amount==0)
                {
                    amount=1;
                }
                index=i;
                amount_of_open_let=amount_of_open_let+amount;
                // continue;
            }  
        }
        
        this.lines=new List<string>();
        //Se eliminan las lineas que quedaron vacías luego de concatenar
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
        //Se divide en tokens cada línea
            foreach (var line in group_of_lines)
            {
            Lexer lexer=new Lexer(File,count.ToString());
            
            List<Token> linetokens= lexer.Tokens(line);
            if (linetokens.Count==1 && linetokens[0].Type==Token.TokenType.EOL)
            {
                continue;
            }
            
            tokens.Add(lexer.Tokens(line));
            //Se van acumulando los errore léxicos
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
    private bool ContainIn(string s)
    {
        MatchCollection matches=Regex.Matches(s,"\\bin\\b",RegexOptions.IgnoreCase);
        return matches.Count>0;
    }
    
}