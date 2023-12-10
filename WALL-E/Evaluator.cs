using System.Globalization;
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
        return ("Used color has been restore to {0}",context.UtilizedColors.Peek());
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
         string par_name="";
        foreach (var item in node.Branches[1].Branches)
        {
            par_name=(string)item.NodeExpression!;
            arg.Add(par_name,"");
        }
        Fuction func=new Fuction(node.Branches[0].NodeExpression!.ToString()!,node.Branches[2],arg);
         bool exist=false;
         foreach (var function in context.Available_Functions)
         {
            if (function.Name==par_name)
            {
               exist=true;
            }
         }
         if (exist)
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"function name, already exist a preexistent function with the same name",new Location("file","line","column")));
         }
         else context.Available_Functions.Add(func);
         return ("{0} Function created and saved",name);
      }
      else if (node.Type==Node.NodeType.GlobalSeq)
      {
        object value=GeneralEvaluation(node.Branches[1]);
        Type type=value.GetType();
        long amount_of_elements=node.Branches[0].Branches.Count;
        long index=0;
        List<object>AlternativeSeq=new List<object>();
        if (value is Finite_Sequence<object>)
        {
        foreach (var subnode in node.Branches[0].Branches)
        {

          if (subnode.Type==Node.NodeType.Low_Hyphen)
          {
            continue;
          }
          if (index==amount_of_elements-1)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            object valueofarg=((Finite_Sequence<object>)value).ReturnValue();
            if (valueofarg==default(object))
            {
              valueofarg="{}";
              if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }
            }
            else
            {
             AlternativeSeq.Add(valueofarg);
            while (valueofarg!=default(object))
            {
              valueofarg=((Finite_Sequence<object>)value).ReturnValue();
              if (valueofarg!=default(object))
              {
                AlternativeSeq.Add(valueofarg);
              }
            }
            Finite_Sequence<object> rest =new Finite_Sequence<object>(AlternativeSeq);
            valueofarg=rest;
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }

            }
          }
          else if (subnode.Type==Node.NodeType.VarName && index!=amount_of_elements-1)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            object valueofarg=((Finite_Sequence<object>)value).ReturnValue();
            if (valueofarg==default(object))
            {
              valueofarg="undefined";
            }
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }
          }
        }
      }
      else if(value.ToString()=="undefined")
      {
        foreach (var subnode in node.Branches[0].Branches)
        {
           if (subnode.Type==Node.NodeType.Low_Hyphen)
          {
            continue;
          }
          else if (subnode.Type==Node.NodeType.VarName)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,"undefined");
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,"undefined");
        }
          }
        }
      }
      else if (value is Enclosed_Infinite_Sequence)
      {
        foreach (var subnode in node.Branches[0].Branches)
        {

          if (subnode.Type==Node.NodeType.Low_Hyphen)
          {
            continue;
          }
          if (index==amount_of_elements-1)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            object valueofarg=((Enclosed_Infinite_Sequence)value).ReturnValue();
            if (valueofarg.Equals(long.MinValue))
            {
              valueofarg="{}";
              if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }
            }
            else
            {
              long newstart=Convert.ToInt64(valueofarg);
              long end=((Enclosed_Infinite_Sequence)value).EndsAd;
              Enclosed_Infinite_Sequence rest =new Enclosed_Infinite_Sequence(newstart,end);
              valueofarg=rest;
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }

            }
          }
          else if (subnode.Type==Node.NodeType.VarName && index!=amount_of_elements-1)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            object valueofarg=((Enclosed_Infinite_Sequence)value).ReturnValue();
            if (valueofarg.Equals(long.MinValue))
            {
              valueofarg="undefined";
            }
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }
          }
        }
      }
      else if (value is Infinite_Sequence)
      {
        foreach (var subnode in node.Branches[0].Branches)
        {

          if (subnode.Type==Node.NodeType.Low_Hyphen)
          {
            continue;
          }
          if (index==amount_of_elements-1)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            object valueofarg=((Infinite_Sequence)value).ReturnValue();
            
              long newstart=Convert.ToInt64(valueofarg);
              Infinite_Sequence rest =new Infinite_Sequence(newstart);
              valueofarg=rest;
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }

            
          }
          else if (subnode.Type==Node.NodeType.VarName && index!=amount_of_elements-1)
          {
            string name=GeneralEvaluation(subnode).ToString()!;
            object valueofarg=((Infinite_Sequence)value).ReturnValue();
            if (CurrentScope.Parent==null)
        {   
             if (context.GlobalConstant.Keys.Contains(name))
            {
              Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
            }
            else 
            context.GlobalConstant.Add(name,valueofarg);
        }
        else
        {   
            if (CurrentScope.Variables.Keys.Contains(name))
           {
             Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"operation,constants can't be modified",new Location("file","line","column")));
           }
            else
            CurrentScope.Variables.Add(name,valueofarg);
        }
          }
        }
      }
      return "Requested values seted";
      }
      else if (node.Type==Node.NodeType.FucName)
      {
         return node.NodeExpression!;
      }
      else if (node.Type==Node.NodeType.ParName)
      {
         return node.NodeExpression!;
      }
      else if (node.Type==Node.NodeType.VarName)
      {
         return node.NodeExpression!;
      }
      if (node.Type==Node.NodeType.Text)
      {
         return node.NodeExpression!;
      }
      else if (node.Type==Node.NodeType.Number)
      {
         return node.NodeExpression!;
      }
      else if (node.Type==Node.NodeType.Sum)
      {
         Sum sum =new Sum();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is string) && !(left is Measure)&& !(left is GenericSequence<object>)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"valid values to operate",new Location("file","line","column")));
         }
         sum.Evaluate(left,right);
         return sum.Value!;
      }
      else if (node.Type==Node.NodeType.Sub)
      {
         Substraction sub =new Substraction();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is Measure)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"valid values to operate",new Location("file","line","column")));
         }
         sub.Evaluate(left,right);
         return sub.Value!;
      }
      else if (node.Type==Node.NodeType.Mul)
      {
         Multiplication mul =new Multiplication();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if (!(left is double && right is Measure) && !(left is Measure && right is double) && !(left is double && right is double))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"valid values to operate",new Location("file","line","column")));
         }
         mul.Evaluate(left,right);
         return mul.Value!;
      }
      else if (node.Type==Node.NodeType.Div)
      {
         Division div =new Division();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is Measure)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"valid values to operate",new Location("file","line","column")));
         }
         div.Evaluate(left,right);
         return div.Value!;
      }
      else if (node.Type==Node.NodeType.Pow)
      {
         Power pow =new Power();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if (!(left is double)||!(right is double))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"numerical values",new Location("file","line","column")));
         }
         pow.Evaluate(left,right);
         return pow.Value!;
      }
      else if (node.Type==Node.NodeType.Module)
      {
        Module mod=new Module();
        object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if (!(left is double)||!(right is double))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"numerical values",new Location("file","line","column")));
         }
         mod.Evaluate(left,right);
         return mod.Value!;
      }
      else if (node.Type==Node.NodeType.Var)
      {
         if (!CurrentScope.Variables.ContainsKey(node.NodeExpression!.ToString()!))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"variable",new Location("file","line","column")));
         }
         else return CurrentScope.Variables[node.NodeExpression!.ToString()!];
      }
      else if (node.Type==Node.NodeType.Declared_FucName)
      {
         return node.NodeExpression!;
      }
      else if (node.Type==Node.NodeType.Sin)
      {
         object arg = GeneralEvaluation(node.Branches[0]);
         return context.Trig_functions["sin"](Convert.ToDouble(arg,CultureInfo.InvariantCulture));
      }
      else if (node.Type==Node.NodeType.Cos)
      {
         object arg = GeneralEvaluation(node.Branches[0]);
         return context.Trig_functions["cos"]((double)arg);
      }
      else if (node.Type==Node.NodeType.Sqrt)
      {
         object arg = GeneralEvaluation(node.Branches[0]);
         return context.Trig_functions["sqrt"](Convert.ToDouble(arg,CultureInfo.InvariantCulture));
      }
      else if (node.Type==Node.NodeType.Log)
      {
         object base_of = GeneralEvaluation(node.Branches[0]);
         object arg = GeneralEvaluation(node.Branches[1]);
         return context.Log["log"](Convert.ToDouble(base_of,CultureInfo.InvariantCulture),Convert.ToDouble(arg,CultureInfo.InvariantCulture));
      }
      else if (node.Type==Node.NodeType.PI)
      {
         return context.Math_value["PI"]();
      }
      else if (node.Type==Node.NodeType.E)
      {
         return context.Math_value["E"]();
      }
      else if (node.Type==Node.NodeType.Randoms)
      {
         IEnumerable<double> rand =context.Randoms["randoms"]();
          InfiniteDoubleSequence randoms=new InfiniteDoubleSequence(rand);
          return randoms;
      }
      else if (node.Type==Node.NodeType.Samples)
      {
         IEnumerable<Point> sam =context.Samples["samples"]();
          InfinitePointSequence samples=new InfinitePointSequence(sam);
          return samples;
      }
      else if (node.Type==Node.NodeType.Points)
      {
          object arg=GeneralEvaluation(node.Branches[0]);
          if (arg is Circle)
          {
            IEnumerable<Point> point =context.Points["points"]((Circle)arg);
            InfinitePointSequence samples=new InfinitePointSequence(point);
             return samples;
          }
          else
          {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"argument",new Location("line","file","column")));
          }
          
      }
      else if (node.Type==Node.NodeType.Count)
      {
        object arg=GeneralEvaluation(node.Branches[0]);
        if (arg is GenericSequence<object>)
        {
          long c= ((GenericSequence<object>)arg).count;
          if (c<0)
          {
            return "undefined";
          }
          return c;
        }
        
      }
      else if (node.Type==Node.NodeType.Undefined)
        {
          return "undefined";
        }
        else if(node.Type==Node.NodeType.Empty_Seq)
        {
          List<object> list=new List<object>();
          Finite_Sequence<object> seq=new Finite_Sequence<object>(list);
          return seq;
        }
        else if (node.Type==Node.NodeType.Infinite_Seq)
        {
          object value=GeneralEvaluation(node.Branches[0]);
          if (value is long)
          {
            Infinite_Sequence seq =new Infinite_Sequence((long)value);
            return seq;
          }
          else
          {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"argument",new Location("line","file","column")));
          }
        }
        else if (node.Type==Node.NodeType.Enclosed_Infinite_Seq)
        {
          object firstvalue=GeneralEvaluation(node.Branches[0]);
          object finalvalue=GeneralEvaluation(node.Branches[1]);
          if (firstvalue is long && finalvalue is long)
          {
            Enclosed_Infinite_Sequence seq =new Enclosed_Infinite_Sequence((long)firstvalue,(long)finalvalue);
            return seq;
          }
          else
          {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"boundries",new Location("line","file","column")));
          }
        }
        else if (node.Type==Node.NodeType.Finite_Seq)
        {
          object firstvalue=GeneralEvaluation(node.Branches[0]);
          Type t=firstvalue.GetType();
          long index=0;
          List<object> valuesofseq=new List<object>();
          valuesofseq.Add(firstvalue);
          foreach (var item in node.Branches)
          {
             if (index==0)
             {
              continue;
             }
             object value=GeneralEvaluation(item);
             if (firstvalue.GetType()!=value.GetType())
             {
               Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"sequence, all values must belong to the same type",new Location("file","line","column")));
               return "Invalid sequence";
             }
             valuesofseq.Add(value);
          }
          Finite_Sequence<object> seq=new Finite_Sequence<object>(valuesofseq);
          return seq;
        }
        else if (node.Type==Node.NodeType.Negation)
        {
          object value=GeneralEvaluation(node.Branches[0]);
          if (CheckTrueORFalse.Check(value))
          {
            return 0;
          }
          else return 1;
        }
        else if (node.Type==Node.NodeType.Minor)
      {
         Minor min =new Minor();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is Measure)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"numeric or measure values",new Location("file","line","column")));
         }
         min.Evaluate(left,right);
         return min.Value!;
      }
      else if (node.Type==Node.NodeType.Major)
      {
         Major maj =new Major();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is Measure)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"numeric or measure values",new Location("file","line","column")));
         }
         maj.Evaluate(left,right);
         return maj.Value!;
      }
      else if (node.Type==Node.NodeType.Equal_Major)
      {
         Equal_Major emaj =new Equal_Major();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is Measure)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"numeric or measure values",new Location("file","line","column")));
         }
         emaj.Evaluate(left,right);
         return emaj.Value!;
      }
      else if (node.Type==Node.NodeType.Equal_Minor)
      {
         Equal_Minor emin =new Equal_Minor();
         object left = GeneralEvaluation(node.Branches[0]);
         object right=GeneralEvaluation(node.Branches[1]);
         if ((left.GetType()!=right.GetType())||(!(left is double) && !(left is Measure)))
         {
            Semantic_Errors.Add(new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Expected,"numeric or measure values",new Location("file","line","column")));
         }
         emin.Evaluate(left,right);
         return emin.Value!;
      }

    }

     

}