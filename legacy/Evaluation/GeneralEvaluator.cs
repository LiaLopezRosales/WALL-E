using System.Threading;

public class GeneralEvaluation
{
    public List<Node> Lines{get;set;}
    public List<List<Error>> SemanticErrors{get;private set;}
    public string file{get;set;}
    public GeneralEvaluation(List<Node> lines,string file)
    {
        Lines=lines;
        SemanticErrors=new List<List<Error>>();
        this.file=file;
    }
    
    public Context EvaluateArchive(Context basecontext, CancellationToken token = default)
    {
       Evaluator evaluate=new Evaluator(basecontext,file);
       evaluate.CancellationToken = token;
       int actualcounterror=0;
        int count=0;
       foreach (var item in Lines)
       {
          token.ThrowIfCancellationRequested();
          evaluate.line=count.ToString();
          object value=evaluate.GeneralEvaluation(item);
          if (evaluate.AllTheSemantic_Errors().Count>actualcounterror)
          {
            SemanticErrors.Add(evaluate.AllTheSemantic_Errors());
            actualcounterror=SemanticErrors.Count;
          }
          else if(SemanticErrors.Count==0)
          {
            basecontext.Results.Add(value);
          }
          count++;
       }
       return evaluate.ResultingContext();
    }
    public List<List<Error>> Semantic_Errors()
    {
        return SemanticErrors;
    }

}