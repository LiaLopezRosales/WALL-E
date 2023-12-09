public class Evaluator
{   //Modificar para poder acceder a las localizaciones de los errores semánticos
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
            if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else context.GlobalConstant.Add(node.NodeExpression!.ToString()!,c);
        }
        else
        {   if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,c);
        }
        return "circle created";
      }
      else if (node.Type==Node.NodeType.Point)
      {
        Point p=new Point(0,0);
        p.RandomPoint(context.ExistingPoints);
        context.ExistingPoints.Add(p);
        if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,p);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,p);
        }
        return "point created";
      }
      else if (node.Type==Node.NodeType.Line)
      {
        Point p1=new Point(0,0);
        Point p2=new Point(0,0);
        Line l=new Line(p1,p2);
        l.RandomLine(context.ExistingLines,context.ExistingPoints);
        context.ExistingLines.Add(l);
        if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,l);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,l);
        }
        return "line created";
      }
      else if (node.Type==Node.NodeType.Segment)
      {
        Point p1=new Point(0,0);
        Point p2=new Point(0,0);
        Segment s=new Segment(p1,p2);
        s.RandomSegment(context.ExistingSegments,context.ExistingPoints);
        context.ExistingSegments.Add(s);
        if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,s);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,s);
        }
        return "segment created";
      }
      else if (node.Type==Node.NodeType.Ray)
      {
        Point p1=new Point(0,0);
        Point p2=new Point(0,0);
        Ray r=new Ray(p1,p2);
        r.RandomRay(context.ExistingRays,context.ExistingPoints);
        context.ExistingRays.Add(r);
        if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,r);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,r);
        }
        return "ray created";
      }
      else if (node.Type==Node.NodeType.Point_Seq)
      {
         List<Point> elements=new List<Point>();
         Random r=new Random();
         int amount=r.Next(1,30);
         for (int i = 0; i < amount ; i++)
         {
            Point temp=new Point(0,0);
            temp.RandomPoint(context.ExistingPoints);
            elements.Add(temp);
            context.ExistingPoints.Add(temp);
         }
         Finite_Sequence<Point> pts=new Finite_Sequence<Point>(elements);
         if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,pts);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,pts);
        }
        return "sequence of points created";
      }
      else if (node.Type==Node.NodeType.Line_Seq)
      {
         List<Line> elements=new List<Line>();
         Random r=new Random();
         int amount=r.Next(1,30);
         for (int i = 0; i < amount ; i++)
         {
            Point temp1=new Point(0,0);
            Point temp2=new Point(0,0);
            Line l=new Line(temp1,temp2);
            l.RandomLine(context.ExistingLines,context.ExistingPoints);
            elements.Add(l);
            context.ExistingLines.Add(l);
         }
         Finite_Sequence<Line> pts=new Finite_Sequence<Line>(elements);
         if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(node.NodeExpression!.ToString()!))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(node.NodeExpression!.ToString()!,pts);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(node.NodeExpression!.ToString()!))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(node.NodeExpression!.ToString()!,pts);
        }
        return "sequence of lines created";
      }
      else if (node.Type==Node.NodeType.GlobalVar)
      {
        string name=GeneralEvaluation(node.Branches[0]).ToString()!;
        object value=GeneralEvaluation(node.Branches[1]);
        if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,value);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,value);
        }
        return "global constant has been added";
        
      }
      else if (node.Type==Node.NodeType.Color)
      {
        if (context.UtilizedColors.Peek()!=node.NodeExpression!.ToString()!)
        {
          context.UtilizedColors.Push(node.NodeExpression!.ToString()!);
        }
        return ("color changed to {0}",node.NodeExpression!.ToString()!);
      }
      else if (node.Type==Node.NodeType.Restore)
      {
        if (context.UtilizedColors.Count>1)
        {
          context.UtilizedColors.Pop();
        }
        return context.UtilizedColors.Peek();
      }
      else if (node.Type==Node.NodeType.Draw)
      {
        object value=GeneralEvaluation(node.Branches[0]);
        string tag=GeneralEvaluation(node.Branches[1]).ToString()!;
        DrawObject d=new DrawObject(value,tag,context.UtilizedColors.Peek());
        if (!d.CheckValidType())
        {
          Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"type,this type of object can't be draw",new Location("file","line","column")));
        }
        else context.ToDraw.Add(d);
        return "Function to draw added";
      }
      else if (node.Type==Node.NodeType.Fuction)
      {
        string name=node.Branches[0]!.ToString()!;
        Dictionary<string,object> arg=new Dictionary<string, object>();
        foreach (var item in node.Branches[1].Branches)
        {
          
        }
      }
    }

     

}