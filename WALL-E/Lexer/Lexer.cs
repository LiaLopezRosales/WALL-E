using System.Text.RegularExpressions;
using System.Globalization;
public class Lexer
{
     public List<Error> lexererrors;

    public Lexer()
    {
        lexererrors = new List<Error>();
    }
    public List<Token> Tokens(string code)
    {
        string patronNumeroNegativo = @"-?\d+(\.\d+)?";
        string low=@"_";
        string patronTexto = "\".*?\"";
        string quotes ="\"";
        string patronPalabras = @"\+|\-|\*|\%|(\...)|(\<\=)|(\>\=)|(\=\=)|(\!\=)|(\=\>)|\{|\}|(\|)|(\&)|\/|\^|(\!)|\@|\,|\(|\)|\{|\}|\<|\>|\=|\;|\:";
        string patronIdentificador = @"\b\w*[a-zA-Z]\w*\b";
        string patron = $"{patronTexto}|{low}|{quotes}|{patronIdentificador}|{patronNumeroNegativo}|{patronPalabras} ";
        MatchCollection matches = Regex.Matches(code, patron);
        List<Token> possibletokens = new List<Token>();
        foreach (Match match in matches)
        {
            Token temporal = IdentifyType(match.Value,lexererrors);
            possibletokens.Add(temporal);
        } 
        //string previus_token="";
        // foreach (Token token in possibletokens)
        // {
        //     if (token.Value != "EOL")
        //     {
        //         previus_token=token.Value;
        //     }
        //     else
        //     {
        //         if (previus_token != ";" && previus_token!="let")
        //         {
        //             lexererrors.Add(new Error(Error.TypeError.Lexical_Error,Error.ErrorCode.Expected,";"));
        //         }
        //     }
        // }
        return possibletokens;
    }

    public List<Error> Lexic_Errors()
    {
        return lexererrors;
    }

    public static Token IdentifyType(string possibletoken,List<Error> errors)
    {
        Token token=new Token(Token.TokenType.not_id,possibletoken);
        if (possibletoken == "draw" )
        {
            token = new Token(Token.TokenType.draw,possibletoken);
        }
        else if (possibletoken == "sqrt" )
        {
            token = new Token(Token.TokenType.sqrt,possibletoken);
        }
        else if (possibletoken == "exp" )
        {
            token = new Token(Token.TokenType.exp,possibletoken);
        }
        else if (possibletoken == "randoms" )
        {
            token = new Token(Token.TokenType.randoms,possibletoken);
        }
        else if (possibletoken == "points" )
        {
            token = new Token(Token.TokenType.points,possibletoken);
        }
        else if (possibletoken == "samples" )
        {
            token = new Token(Token.TokenType.samples,possibletoken);
        }
        else if (possibletoken == "count" )
        {
            token = new Token(Token.TokenType.count,possibletoken);
        }
        else if (possibletoken == "intersect" )
        {
            token = new Token(Token.TokenType.intersect,possibletoken);
        }
        else if (possibletoken == "measure" )
        {
            token = new Token(Token.TokenType.measure,possibletoken);
        }
        else if (possibletoken == "arc" )
        {
            token = new Token(Token.TokenType.arc,possibletoken);
        }
        else if (possibletoken == "EOL" )
        {
            token = new Token(Token.TokenType.EOL,possibletoken);
        }
        else if (possibletoken == "EOF" )
        {
            token = new Token(Token.TokenType.EOF,possibletoken);
        }
        //Revisar si estos dos funcionan de la forma actual(vía alternativa hacerlo por separado y comprobar en el parser si son sucesivos y actuar acorde)
        else if (possibletoken == "point sequence" )
        {
            token = new Token(Token.TokenType.point_sequence,possibletoken);
        }
        else if (possibletoken == "line sequence" )
        {
            token = new Token(Token.TokenType.line_sequence,possibletoken);
        }
        else if (possibletoken == "circle" )
        {
            token = new Token(Token.TokenType.circle,possibletoken);
        }
        else if (possibletoken == "ray" )
        {
            token = new Token(Token.TokenType.ray,possibletoken);
        }
        else if (possibletoken == "segment" )
        {
            token = new Token(Token.TokenType.segment,possibletoken);
        }
        else if (possibletoken == "line" )
        {
            token = new Token(Token.TokenType.line,possibletoken);
        }
        else if (possibletoken == "point" )
        {
            token = new Token(Token.TokenType.point,possibletoken);
        }
        //Este existía?
        else if (possibletoken == "sequence" )
        {
            token = new Token(Token.TokenType.sequence,possibletoken);
        }
        else if (possibletoken == "undefined" )
        {
            token = new Token(Token.TokenType.undefined,possibletoken);
        }
        else if (possibletoken == "sin" )
        {
            token = new Token(Token.TokenType.sin,possibletoken);
        }
        else if (possibletoken == "cos" )
        {
            token = new Token(Token.TokenType.cos,possibletoken);
        }
        else if (possibletoken == "log" )
        {
            token = new Token(Token.TokenType.log,possibletoken);
        }
        else if (possibletoken == "PI" )
        {
            token = new Token(Token.TokenType.PI,possibletoken);
        }
        else if (possibletoken == "E" )
        {
            token = new Token(Token.TokenType.E,possibletoken);
        }
        else if (possibletoken=="+")
        {
            token = new Token(Token.TokenType.sum,possibletoken);
        }
        else if (possibletoken=="-")
        {
            token = new Token(Token.TokenType.substraction,possibletoken);
        }
        else if (possibletoken=="*")
        {
            token = new Token(Token.TokenType.multiplication,possibletoken);
        }
        else if (possibletoken=="/")
        {
            token = new Token(Token.TokenType.division,possibletoken);
        }
        else if (possibletoken=="^")
        {
            token = new Token(Token.TokenType.power,possibletoken);
        }     
         else if (possibletoken=="%")
        {
            token = new Token(Token.TokenType.module,possibletoken);
        }  
        else if (possibletoken == "let" || possibletoken == "in" || possibletoken == "import"|| possibletoken == "color"|| possibletoken == "restore")
        {
            token = new Token(Token.TokenType.keyword, possibletoken);
        }
        else if (possibletoken == "if" || possibletoken == "else"|| possibletoken == "then")
        {
            token = new Token(Token.TokenType.conditional, possibletoken);
        }
        else if (possibletoken == "!=" )
        {
            token = new Token(Token.TokenType.diferent, possibletoken);
        }
        else if (possibletoken == "," ||  possibletoken == ";" || possibletoken == ":" || possibletoken == "=>" || possibletoken == "="||  possibletoken == "..." )
        {
            token = new Token(Token.TokenType.symbol, possibletoken);
        }
        else if (possibletoken=="(")
        {
            token = new Token(Token.TokenType.left_bracket,possibletoken);
        }
        else if (possibletoken==")")
        {
            token = new Token(Token.TokenType.right_bracket,possibletoken);
        }
        else if (possibletoken=="{")
        {
            token = new Token(Token.TokenType.left_key,possibletoken);
        }
        else if (possibletoken=="}")
        {
            token = new Token(Token.TokenType.right_key,possibletoken);
        }
        else if (possibletoken=="!")
        {
            token = new Token(Token.TokenType.not,possibletoken);
        }
        else if (possibletoken == "@")
        {
            token = new Token(Token.TokenType.concatenate, possibletoken);
        }
        else if (possibletoken=="_")
        {
            token = new Token(Token.TokenType.low_hyphen,possibletoken);
        }
        else if (possibletoken == "==" )
        {
            token = new Token(Token.TokenType.equal, possibletoken);
        }
        else if (possibletoken == "<" )
        {
            token = new Token(Token.TokenType.minor, possibletoken);
        }
        else if (possibletoken == ">" )
        {
            token = new Token(Token.TokenType.major, possibletoken);
        }
        else if (possibletoken == "<=" )
        {
            token = new Token(Token.TokenType.equal_minor, possibletoken);
        }
        else if (possibletoken == ">=" )
        {
            token = new Token(Token.TokenType.equal_major, possibletoken);
        }
        else if (possibletoken == "|" )
        {
            token = new Token(Token.TokenType.Or, possibletoken);
        }
        else if (possibletoken == "&" )
        {
            token = new Token(Token.TokenType.And, possibletoken);
        } 
        else if (possibletoken == "black" || possibletoken == "white"|| possibletoken == "blue"|| possibletoken == "red"|| possibletoken == "yellow"|| possibletoken == "green"|| possibletoken == "cyan"|| possibletoken == "magenta"|| possibletoken == "grey")
        {
            token = new Token(Token.TokenType.color_value, possibletoken);
        }
        else if (double.TryParse(possibletoken, out double result))
        {
            token = new Token(Token.TokenType.number,possibletoken);
        }
        else if (possibletoken.StartsWith("\"") && possibletoken.EndsWith("\"")&&possibletoken!="\"")
        {
            token = new Token(Token.TokenType.text, possibletoken);
        }
        else if(possibletoken=="\"")
        {
          errors.Add(new Error(Error.TypeError.Lexical_Error,Error.ErrorCode.Expected,"\""));
        }
        else
        {
            if (char.IsDigit(possibletoken[0]))
            {
                errors.Add(new Error(Error.TypeError.Lexical_Error,Error.ErrorCode.Invalid,"name, must start with letters"));
                token = new Token(Token.TokenType.not_id,possibletoken);
            }
            else
            {
            token = new Token(Token.TokenType.identifier, possibletoken);
            }
        }
        return token;

    }

}