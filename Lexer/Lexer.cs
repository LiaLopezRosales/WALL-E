using System.Text.RegularExpressions;
using System.Globalization;
public class Lexer
{
     public List<Error> lexererrors;
     public string File{get;set;}
     public string Line{get;set;}

    public Lexer(string file,string line)
    {
        lexererrors = new List<Error>();
        File=file;
        Line=line;
    }
    public List<Token> Tokens(string code)
    {
        string patronNumeroNegativo = @"-?\d+(\.\d+)?";
        string low=@"_";
        string patronTexto = "\".*?\"";
        string quotes ="\"";
        string patronPalabras = @"\+|\-|\*|\%|(\...)|(\<\=)|(\>\=)|(\=\=)|(\!\=)|(\=\>)|\{|\}|\/|\^|(\!)|\,|\(|\)|\{|\}|\<|\>|\=|\;|\:";
        string patronIdentificador = @"\b\w*[a-zA-Z]\w*\b";
        string patron = $"{patronTexto}|{low}|{quotes}|{patronIdentificador}|{patronNumeroNegativo}|{patronPalabras} ";
        MatchCollection matches = Regex.Matches(code, patron);
        List<Token> possibletokens = new List<Token>();
        foreach (Match match in matches)
        {
            Token temporal = IdentifyType(match.Value,lexererrors,possibletokens.Count);
            if (temporal.Type==Token.TokenType.not_id)
            {
                lexererrors.Add(new Error(Error.TypeError.Lexical_Error,Error.ErrorCode.Invalid,"token",temporal.TokenLocation));
            }
            possibletokens.Add(temporal);
        }
        possibletokens.Add(new Token(Token.TokenType.EOL,";",File,Line,possibletokens.Count.ToString())); 
        return possibletokens;
    }

    public List<Error> Lexic_Errors()
    {
        return lexererrors;
    }

    public Token IdentifyType(string possibletoken,List<Error> errors,int index)
    {
        Token token=new Token(Token.TokenType.not_id,possibletoken,File,Line,index.ToString());
        if (possibletoken == "draw" )
        {
            token = new Token(Token.TokenType.draw,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "sqrt" )
        {
            token = new Token(Token.TokenType.sqrt,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "randoms" )
        {
            token = new Token(Token.TokenType.randoms,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "points" )
        {
            token = new Token(Token.TokenType.points,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "samples" )
        {
            token = new Token(Token.TokenType.samples,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "count" )
        {
            token = new Token(Token.TokenType.count,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "intersect" )
        {
            token = new Token(Token.TokenType.intersect,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "measure" )
        {
            token = new Token(Token.TokenType.measure,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "arc" )
        {
            token = new Token(Token.TokenType.arc,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "EOL" )
        {
            token = new Token(Token.TokenType.EOL,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "EOF" )
        {
            token = new Token(Token.TokenType.EOF,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "circle" )
        {
            token = new Token(Token.TokenType.circle,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "ray" )
        {
            token = new Token(Token.TokenType.ray,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "segment" )
        {
            token = new Token(Token.TokenType.segment,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "line" )
        {
            token = new Token(Token.TokenType.line,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "point" )
        {
            token = new Token(Token.TokenType.point,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "sequence" )
        {
            token = new Token(Token.TokenType.sequence,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "undefined" )
        {
            token = new Token(Token.TokenType.undefined,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "sin" )
        {
            token = new Token(Token.TokenType.sin,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "cos" )
        {
            token = new Token(Token.TokenType.cos,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "log" )
        {
            token = new Token(Token.TokenType.log,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "PI" )
        {
            token = new Token(Token.TokenType.PI,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "E" )
        {
            token = new Token(Token.TokenType.E,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="+")
        {
            token = new Token(Token.TokenType.sum,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="-")
        {
            token = new Token(Token.TokenType.substraction,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="*")
        {
            token = new Token(Token.TokenType.multiplication,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="/")
        {
            token = new Token(Token.TokenType.division,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="^")
        {
            token = new Token(Token.TokenType.power,possibletoken,File,Line,index.ToString());
        }     
         else if (possibletoken=="%")
        {
            token = new Token(Token.TokenType.module,possibletoken,File,Line,index.ToString());
        }  
        else if (possibletoken == "let" || possibletoken == "in" || possibletoken == "import"|| possibletoken == "color"|| possibletoken == "restore")
        {
            token = new Token(Token.TokenType.keyword, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "if" || possibletoken == "else"|| possibletoken == "then")
        {
            token = new Token(Token.TokenType.conditional, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "!=" )
        {
            token = new Token(Token.TokenType.diferent, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "," ||  possibletoken == ";" || possibletoken == ":" || possibletoken == "=>" || possibletoken == "="||  possibletoken == "..." )
        {
            token = new Token(Token.TokenType.symbol, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="(")
        {
            token = new Token(Token.TokenType.left_bracket,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken==")")
        {
            token = new Token(Token.TokenType.right_bracket,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="{")
        {
            token = new Token(Token.TokenType.left_key,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="}")
        {
            token = new Token(Token.TokenType.right_key,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="not")
        {
            token = new Token(Token.TokenType.not,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "@")
        {
            token = new Token(Token.TokenType.concatenate, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken=="_")
        {
            token = new Token(Token.TokenType.low_hyphen,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "==" )
        {
            token = new Token(Token.TokenType.equal, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "<" )
        {
            token = new Token(Token.TokenType.minor, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == ">" )
        {
            token = new Token(Token.TokenType.major, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "<=" )
        {
            token = new Token(Token.TokenType.equal_minor, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == ">=" )
        {
            token = new Token(Token.TokenType.equal_major, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "or" )
        {
            token = new Token(Token.TokenType.Or, possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken == "and" )
        {
            token = new Token(Token.TokenType.And, possibletoken,File,Line,index.ToString());
        } 
        else if (possibletoken == "black" || possibletoken == "white"|| possibletoken == "blue"|| possibletoken == "red"|| possibletoken == "yellow"|| possibletoken == "green"|| possibletoken == "cyan"|| possibletoken == "magenta"|| possibletoken == "grey")
        {
            token = new Token(Token.TokenType.color_value, possibletoken,File,Line,index.ToString());
        }
        else if (double.TryParse(possibletoken, out double result))
        {
            token = new Token(Token.TokenType.number,possibletoken,File,Line,index.ToString());
        }
        else if (possibletoken.StartsWith("\"") && possibletoken.EndsWith("\"")&&possibletoken!="\"")
        {
            token = new Token(Token.TokenType.text, possibletoken,File,Line,index.ToString());
        }
        else if(possibletoken=="\"")
        {
          errors.Add(new Error(Error.TypeError.Lexical_Error,Error.ErrorCode.Expected,"\"",new Location(File,Line,index.ToString())));
        }
        else
        {
            if (char.IsDigit(possibletoken[0]))
            {
                errors.Add(new Error(Error.TypeError.Lexical_Error,Error.ErrorCode.Invalid,"name, must start with letters",new Location(File,Line,index.ToString())));
                token = new Token(Token.TokenType.not_id,possibletoken,File,Line,index.ToString());
            }
            else
            {
            token = new Token(Token.TokenType.identifier, possibletoken,File,Line,index.ToString());
            }
        }
        return token;

    }

}