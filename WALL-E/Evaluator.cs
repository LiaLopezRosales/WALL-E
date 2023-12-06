public class Evaluator
{
     private Node AST{get;set;}
    private Scope scope{get;set;}
    private Scope CurrentScope{get;set;}
    private Context context{get;set;}
    // private List<Scope>? currentcontext{get;set;}
    public List<Error> Semantic_Errors{get;set;}
    public bool PreviusLineWasImport{get;}

     public Evaluator(Context context,bool previuslineisimport)
     {
        this.context=context;
        scope = new Scope();
        Semantic_Errors=new List<Error>();
        AST=new Node();
        PreviusLineWasImport=previuslineisimport;
        CurrentScope=scope;
     }

     public void Tree_Reader(Node root)
     {
       AST=root;
       // currentcontext=new List<Scope>(){new Scope()};
     }

      public object StartEvaluation(Node node)
     {
         object result=GeneralEvaluation(node);
         return result;
     }
      public List<Error> AllTheSemantic_Errors()
    {
        return Semantic_Errors;
    }

    public object GeneralEvaluation(Node node)
    {
      if (node.Type==Node.NodeType.Circle)
      {
        Point center=new Point(0,0);
        Circle c=new Circle(center,1);
        c.RandomCircle(context.ExistingCircles,context.ExistingPoints);
        context.ExistingCircles.Add(c);
        if (CurrentScope.Parent==null)
        {
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,c);
        }
        else
        {
            CurrentScope.Add
        }
      }
    }

     

}