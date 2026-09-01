namespace Wall_E.Domain;

/// <summary>Abstract syntax tree node with a typed discriminator, optional payload, and child branches.</summary>
public class Node
{
    /// <summary>The node kind (statement, expression, figure, sequence, ...).</summary>
    public NodeType Type { get; set; }
    /// <summary>Optional literal value or payload carried by the node.</summary>
    public object? NodeExpression { get; set; }
    /// <summary>Child nodes (operands, block bodies, arguments).</summary>
    public List<Node> Branches;

    /// <summary>Creates a node with an undefined type and no children.</summary>
    public Node()
    {
        Type = NodeType.Indefined;
        Branches = new List<Node>();
    }

    /// <summary>Enumerates every node kind recognized by the WALL-E grammar.</summary>
    public enum NodeType { Instructions, GlobalVar, GlobalSeq, VarName, Assigment, Low_Hyphen, Let_exp, Draw, Conditional, IF, Else, FucName, Declared_FucName, Declared_Fuc, ParName, Negation, Var, parameters, Fuction, Concat, And, Or, Minor, Major, Equal_Minor, Equal_Major, Equal, Diferent, Sum, Sub, Mul, Div, Module, Pow, No, Number, Circle, Point, Line, Ray, Segment, Arc, Point_Seq, Line_Seq, Color, Restore, Import, Point_Fuc, Line_Fuc, Segment_Fuc, Ray_Fuc, Circle_Fuc, Polygon_Fuc, Ellipse_Fuc, Measure, Measure_Fuc, Intersect, Count, Label, Text, Cos, Sin, Log, Sqrt, Tan, Atan, Abs, Floor, Ceil, Phi, Sqrt2, Seed, Print, ColorRgb, ColorRgba, ColorHsl, Lighten, Darken, MixColors, Complement, Repeat, For, Animate, LineStyleStmt, GrosorStmt, FillStmt, Points, Randoms, Samples, Empty_Seq, Enclosed_Infinite_Seq, Infinite_Seq, Finite_Seq, PI, E, Indefined, Undefined, LayerStmt, HideStmt, ShowStmt, SnapStmt };

    /// <summary>Enumerates the possible runtime value kinds a node can yield.</summary>
    public enum ReturnType { number, text, figure, sequence, no_return, fuction_call }
}