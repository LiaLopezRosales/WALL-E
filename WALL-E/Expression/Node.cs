public class Node
{
    public NodeType Type{get;set;}
    public object? NodeExpression{get;set;}
    public List<Node>Branches;
    public Node()
    {
      Type=NodeType.Indefined;
      Branches=new();
    }

    public enum NodeType{Assignations,VarName,Assigment,Let_exp,Draw,Conditional,IF,Else,FucName,Declared_FucName,Declared_Fuc,ParName,Negation,Var,parameters,Fuction,Concat,And,Or,Minor,Major,Equal_Minor,Equal_Major,Equal,Diferent,Sum,Sub,Mul,Div,Pow,No,Number,Circle,Point,Line,Ray,Segment,Arc,Point_Seq,Line_Seq,Color,Restore,Import,Line_Fuc,Segment_Fuc,Ray_Fuc,Circle_Fuc,Measure,Intersect,Count,Text,Cos,Sin,Log,Sqrt,Points,Randoms,Samples,PI,E,Indefined};

}