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
    
    public Context EvaluateArchive(Context basecontext)
    {
        Evaluator evaluate=new Evaluator(basecontext,false);
        
       foreach (var item in Lines)
       {
          object value=evaluate.GeneralEvaluation(item);
          if (evaluate.AllTheSemantic_Errors().Count>0)
          {
            SemanticErrors.Add(evaluate.AllTheSemantic_Errors());
          }
          else if(SemanticErrors.Count==0)
          {
            Console.WriteLine(value);
          }
       }
       return evaluate.ResultingContext();
    }
    public List<List<Error>> Semantic_Errors()
    {
        return SemanticErrors;
    }

}