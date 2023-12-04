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

    public enum NodeType{Instructions,GlobalVar,GlobalSeq,VarName,Assigment,Low_Hyphen,Let_exp,Draw,Conditional,IF,Else,FucName,Declared_FucName,Declared_Fuc,ParName,Negation,Var,parameters,Fuction,Concat,And,Or,Minor,Major,Equal_Minor,Equal_Major,Equal,Diferent,Sum,Sub,Mul,Div,Module,Pow,No,Number,Circle,Point,Line,Ray,Segment,Arc,Point_Seq,Line_Seq,Color,Restore,Import,Line_Fuc,Segment_Fuc,Ray_Fuc,Circle_Fuc,Measure,Measure_Fuc,Intersect,Count,Text,Cos,Sin,Log,Sqrt,Points,Randoms,Samples,Empty_Seq,Enclosed_Infinite_Seq,Infinite_Seq,Finite_Seq,PI,E,Indefined,Undefined};

}