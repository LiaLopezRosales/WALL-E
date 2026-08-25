using System.Collections;
using Wall_E.Domain;
namespace Wall_E.Application.DSL;
/// <summary>Provides sequential and lookahead access over a list of tokens for the parser.</summary>
public class TokenStream:IEnumerable<Token>
{
    public List<Token> tokens{get;}
    int position;
    int line;
    public TokenStream(List<Token>tokens)
    {
       this.tokens=tokens;
       position=0;
       line=1;
    }

    /// <summary>Returns the current index within the token list.</summary>
    public int Position()
    {
        return position;
    }

    /// <summary>Returns true if the stream has reached the last token.</summary>
    public bool End()
    {
        if (position==tokens.Count-1)
        {
            return true;
        }
        else return false;
    }
    /// <summary>Returns true if any token in the stream has the specified value.</summary>
    public bool Contains(string s)
    {
        bool contain=false;
        foreach (var token in tokens)
        {
            if (token.Value==s)
            {
                contain=true;
            }
        }
        return contain;
    }

    /// <summary>Advances the current position by i tokens, clamping at the end.</summary>
    public void MoveForward(int i)
    {
        if ( position+i <=tokens.Count-1)
        {
            position+=i;
        }
        
    }

    /// <summary>Moves the current position backward by i tokens.</summary>
    public void MoveBackward(int i)
    {
        if ( position-i >= tokens.Count-1)
        {
            position-=i;
        }
    }
    /// <summary>Sets the current position to the specified index if within bounds.</summary>
    public void MoveTo(int i)
    {
        if (i>=0&&i<=tokens.Count-1)
        {
            position=i;
        }
        
    }

    /// <summary>Advances the position by one and returns true if not at the end.</summary>
    public bool Next()
    {
        if (position<tokens.Count-1)
        {
            position++;
        }
        return position<tokens.Count-1;
    }
    
    /// <summary>Advances if the next token matches the given type, returning whether it matched.</summary>
     public bool Next(Token.TokenType type)
    {
        if (position<tokens.Count-1&& LookAhead(1).Type==type)
        {
            position++;
            return true;
        }
        return false;
    }
    /// <summary>Returns the token at position+k without advancing the stream.</summary>
    public Token LookAhead(int k=0)
    {
        return tokens[position+k];
    }
    /// <summary>Returns true if k tokens ahead are still within bounds.</summary>
    public bool CanLookAhead(int k=0)
    {
        return tokens.Count-position>k;
    }

    /// <summary>Advances if the next token matches the given value, returning whether it matched.</summary>
    public bool Next(string value)
    {
        if (position<tokens.Count-1 && LookAhead(1).Value==value)
        {
            position++;
            return true;
        }
        return false;
    }
    public IEnumerator<Token> GetEnumerator()
    {
        for (int i = position; i < tokens.Count; i++)
        {
            yield return tokens[i];
        }
    }
     IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}