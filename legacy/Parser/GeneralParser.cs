public class GeneralParser
{
    List<List<Token>> lines{get;set;}
    List<List<Error>> all_the_errors{get;set;}
    string File{get;set;}

    public GeneralParser(List<List<Token>> lexers,string file)
    {
        lines=lexers;
        all_the_errors=new List<List<Error>>();
        File=file;
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
        if (CheckImportStatementsOrder(Trees)>0)
        {
          all_the_errors.Add(new List<Error>(){new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Invalid,"statement order,import orders must predate all other statements",new Location(File,CheckImportStatementsOrder(Trees).ToString(),"-1"))});
        }

        return Trees;
    }
    public long CheckImportStatementsOrder(List<Node> lines)
    {
       bool is_import=true;
       long count=0;
       foreach (var item in lines)
       {
         if (item.Type==Node.NodeType.Import)
         {
           if (is_import)
           {
             continue;
           }
           else
           {
             return count;
           }
         }
         else
         {
           is_import=false;
         }
         count++;
       }
       return -1;
    }
    public List<List<Error>> ParserErrors()
    {
        return all_the_errors;
    }
}