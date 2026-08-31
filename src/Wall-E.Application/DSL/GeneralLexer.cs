using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Wall_E.Domain;
namespace Wall_E.Application.DSL;
/// <summary>Splits multi-line DSL source code into logical statement chunks, handling let-in and block concatenation.</summary>
public class GeneralLexer
{    //Divide un código en expresiones y declaraciones
    public string code{get;set;}
    List<List<Error>> errors{get;set;}
    public List<string> lines{get;private set;}
    public string File{get;set;}

    public GeneralLexer(string code,string file)
    {
        this.code=StripLineComments(code);
        errors=new List<List<Error>>();
        File=file;

        // Split the source into complete, self-contained statements. Statements
        // are delimited by ';' but a repeat/for block (or a let-in) may span
        // several ';' pieces. We accumulate pieces and emit a finished statement
        // whenever its braces are balanced and no 'let' awaits its 'in'.
        //
        // Historical bug this rewrites: a piece that closes one block and then
        // continues with more statements on the same ';' unit (e.g. "} repeat(2) {"
        // or "} draw ...") was merged into a single chunk because only the net
        // brace depth was examined. That silently dropped every block/statement
        // after the first, so a program with several consecutive loops (or a loop
        // followed by more code) only rendered the first one. Now, whenever a piece
        // brings the running depth back to 0, that can only be the close of a
        // statement-level block (sequence literals like {1,2} stay balanced within
        // their enclosing block, so their '}' never makes the GLOBAL depth hit 0),
        // and the text after that point starts a fresh statement: split there.
        string[] pieces=this.code.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
        List<string> statements=new List<string>();
        StringBuilder acc=new StringBuilder();
        int depth=0;          // unmatched '{' across the accumulated statement
        int pendingLet=0;     // unmatched 'let' still awaiting its 'in'

        foreach (string piece in pieces)
        {
            if (acc.Length==0)
            {
                acc.Append(piece);
                depth=CountBraces(piece);
                pendingLet=Math.Max(0, Amount_of_Lets(piece)-CountIns(piece));
                FlushIfComplete(statements, acc, ref depth, ref pendingLet);
                continue;
            }

            // A statement is already open. If an unmatched block is pending and
            // this piece closes it and then opens a fresh unclosed block, the
            // two halves belong to different statements: split at that boundary.
            int boundary=EarliestCloseIndex(piece, depth, out int remainderNet);
            if (depth>0 && boundary>=0 && pendingLet==0)
            {
                string prefix=piece.Substring(0, boundary);
                string rest=piece.Substring(boundary);
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    acc.Append(';').Append(prefix);
                }
                // The prefix's trailing '}' balances the block that was open in
                // acc, so the accumulation left behind is now a complete statement.
                depth=0;
                FlushIfComplete(statements, acc, ref depth, ref pendingLet); // emits the closed statement
                acc.Clear();
                acc.Append(rest);
                depth=CountBraces(rest);
                pendingLet=Math.Max(0, Amount_of_Lets(rest)-CountIns(rest));
                FlushIfComplete(statements, acc, ref depth, ref pendingLet); // may already be complete
                continue;
            }

            // Otherwise continue the open statement, re-joining pieces with ';'.
            acc.Append(';').Append(piece);
            depth+=CountBraces(piece);
            pendingLet=Math.Max(0, pendingLet+Amount_of_Lets(piece)-CountIns(piece));
            FlushIfComplete(statements, acc, ref depth, ref pendingLet);
        }

        // Trailing unclosed text still counts as a statement.
        if (acc.Length>0 && !string.IsNullOrWhiteSpace(acc.ToString()))
        {
            statements.Add(acc.ToString().Trim());
        }

        this.lines=statements;
    }

    /// <summary>Emits the accumulated statement when it is complete (balanced
    /// braces and no pending let), then resets the accumulator.</summary>
    private void FlushIfComplete(List<string> statements, StringBuilder acc,
        ref int depth, ref int pendingLet)
    {
        if (depth==0 && pendingLet==0 && acc.Length>0 && !string.IsNullOrWhiteSpace(acc.ToString()))
        {
            statements.Add(acc.ToString().Trim());
            acc.Clear();
        }
    }

    /// <summary>Counts the net brace depth of a chunk ({ = +1, } = -1).</summary>
    private int CountBraces(string s)
    {
        int d=0;
        foreach (char c in s)
        {
            if (c=='{') d++;
            else if (c=='}') d--;
        }
        return d;
    }

    /// <summary>Given a piece appended to a statement already at 'startDepth',
    /// returns the index in the piece where the running depth first falls back
    /// to 0 (its leading block closes); or -1 if it never does.</summary>
    private int EarliestCloseIndex(string piece, int startDepth, out int remainderNet)
    {
        int running=startDepth;
        for (int i=0; i<piece.Length; i++)
        {
            if (piece[i]=='{') running++;
            else if (piece[i]=='}') running--;
            if (running==0 && piece[i]=='}')
            {
                string remainder=piece.Substring(i+1);
                remainderNet=CountBraces(remainder);
                return i+1;
            }
        }
        remainderNet=0;
        return -1;
    }

    /// <summary>Lexes each statement chunk into a list of tokens, accumulating lexical errors.</summary>
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

    /// <summary>Returns all lexical errors collected during processing.</summary>
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
    private int CountIns(string s)
    {
        MatchCollection matches=Regex.Matches(s,"\\bin\\b",RegexOptions.IgnoreCase);
        return matches.Count;
    }

    /// <summary>Strips single-line comments (//...) from each line,
    /// respecting quoted strings so "http://..." is preserved.</summary>
    private static string StripLineComments(string code)
    {
        var result = new System.Text.StringBuilder();
        var lines = code.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            bool inString = false;
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == '"') inString = !inString;
                if (!inString && j + 1 < line.Length && line[j] == '/' && line[j + 1] == '/')
                {
                    line = line.Substring(0, j);
                    break;
                }
            }
            line = line.TrimEnd('\r', ' ');
            if (i > 0) result.Append('\n');
            result.Append(line);
        }
        return result.ToString();
    }
}