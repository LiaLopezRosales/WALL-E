using System.Collections;
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

    public int Position()
    {
        return position;
    }

    public bool End()
    {
        if (position==tokens.Count-1)
        {
            return true;
        }
        else return false;
    }
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

    public void MoveForward(int i)
    {
        if ( position+i <=tokens.Count-1)
        {
            position+=i;
        }
        
    }

    public void MoveBackward(int i)
    {
        if ( position-i >= tokens.Count-1)
        {
            position-=i;
        }
    }
    public void MoveTo(int i)
    {
        if (i>=0&&i<=tokens.Count-1)
        {
            position=i;
        }
        
    }

    public bool Next()
    {
        if (position<tokens.Count-1)
        {
            position++;
        }
        return position<tokens.Count-1;
    }
    
     public bool Next(Token.TokenType type)
    {
        if (position<tokens.Count-1&& LookAhead(1).Type==type)
        {
            position++;
            return true;
        }
        return false;
    }
    public Token LookAhead(int k=0)
    {
        return tokens[position+k];
    }
    public bool CanLookAhead(int k=0)
    {
        return tokens.Count-position>k;
    }

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