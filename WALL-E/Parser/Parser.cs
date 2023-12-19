using System.Globalization;
public class Parser
{
    List<Token> tokens;
    TokenStream tokenstream;
    Scope scope;

    public List<Error> errors;
    public Parser(List<Token> tokens_expression)
    {
        tokenstream = new TokenStream(tokens_expression);
        tokens = tokens_expression;
        scope = new Scope();
        errors = new List<Error>();
    }

    public Node Parse()
    {
        Node AST = ParseStatement();
        return AST;
    }
    public List<Error> Syntactic_Errors()
    {
        return errors;
    }

    public Node ParseStatement()
    {
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Value == "import")
        {
            return Import();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.circle && tokens[tokenstream.Position() + 1].Type != Token.TokenType.left_bracket)
        {
            return SCircle();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.point && tokens[tokenstream.Position() + 1].Type == Token.TokenType.sequence)
        {
            return PointSequence();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.line && tokens[tokenstream.Position() + 1].Type == Token.TokenType.sequence)
        {
            return LineSequence();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.point)
        {
            return SPoint();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.line && tokens[tokenstream.Position() + 1].Type != Token.TokenType.left_bracket)
        {
            return SLine();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.segment && tokens[tokenstream.Position() + 1].Type != Token.TokenType.left_bracket)
        {
            return SSegment();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.ray && tokens[tokenstream.Position() + 1].Type != Token.TokenType.left_bracket)
        {
            return SRay();
        }
        
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.identifier && tokens[tokenstream.Position() + 1].Value == "=")
        {
            return GlobalVar();
        }
        if ((tokenstream.Position() < tokens.Count) && ((tokens[tokenstream.Position()].Type ==Token.TokenType.low_hyphen) || (tokens[tokenstream.Position()].Type == Token.TokenType.identifier && tokens[tokenstream.Position() + 1].Value == ",")))
        {
            return GlobalSeq();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Value == "color")
        {
            return Color();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Value == "restore")
        {
            return Restore();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.draw)
        {
            return Draw();
        }
         if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.identifier && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket && (tokenstream.Contains("=")))
        {
            return Function();
        }
        return ParseExpression();

    }
    public Node ParseExpression()
    {
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Value == "let")
        {
            return Let_In();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Value == "if")
        {
            return IF_ElSE();
        }
        // if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.identifier && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket)
        // {
        //     return Exp_Fuc();
        // }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.circle && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket)
        {
            return PCircle();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.ray && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket)
        {
            return PRay();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.segment && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket)
        {
            return PSegment();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.line && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket)
        {
            return PLine();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.arc)
        {
            return PArc();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.measure)
        {
            return PMeasure();
        }
        if ((tokenstream.Position() < tokens.Count) && tokens[tokenstream.Position()].Type == Token.TokenType.intersect)
        {
            return PIntersect();
        }
        return ParseOP();

    }
    public Node Import()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.text)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of file",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Import;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        if (!(tokenstream.tokens[tokenstream.Position()].Value.EndsWith(".geo\"")))
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Invalid,"file named,must end with .geo extension",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        tokenstream.MoveForward(1);
        
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,import order must contain only the name of the file",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node Exp_Fuc()
    {
        string name = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        Node Arguments = new Node();
        Node function = new Node();

        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            do
            {
                tokenstream.MoveForward(1);
                if (tokenstream.Position() >= tokens.Count - 1)
                {
                    errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",new Location(tokenstream.tokens[0].TokenLocation.File,tokenstream.tokens[0].TokenLocation.Line,((tokens.Count)-1).ToString())));
                    break;
                }
                
                Node arg=ParseExpression();
                // arg.Type=Node.NodeType.ParName;
                // Node val = ParseExpression();
                // arg.NodeExpression=val;
                Arguments.Branches.Add(arg);
            } while (tokenstream.tokens[tokenstream.Position()].Value == ",");
            if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);

        }
        else tokenstream.MoveForward(1);
        function.Type = Node.NodeType.Declared_Fuc;
        Node func_name = new Node();
        func_name.Type = Node.NodeType.Declared_FucName;
        func_name.NodeExpression = name;
        function.Branches = new List<Node> { func_name, Arguments };
        return function;
    }
    public Node SCircle()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Circle;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node SPoint()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Point;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node SLine()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Line;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node SSegment()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Segment;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node SRay()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Ray;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the figure",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node PointSequence()
    {
        tokenstream.MoveForward(2);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the sequence",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Point_Seq;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the sequence",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node LineSequence()
    {
        tokenstream.MoveForward(2);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "name of the sequence",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Line_Seq;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only an identifier for the sequence",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node Color()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.color_value)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "valid color",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Color;
        value.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement,must declare only a color",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;

    }
    public Node Restore()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement, restore order must be alone",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        Node value = new Node();
        value.Type = Node.NodeType.Restore;
        if (tokenstream.Position()==tokens.Count-1)
        {
            return value;
        }
        tokenstream.MoveForward(1);
        return value;
    }
    public Node GlobalVar()
    {
        Node globalvar = new Node();
        globalvar.Type = Node.NodeType.GlobalVar;
        Node var_name = new Node();
        var_name.Type = Node.NodeType.VarName;
        var_name.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
        tokenstream.MoveForward(2);
        Node value = ParseExpression();
        if (value.NodeExpression is Error)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        globalvar.Branches = new List<Node> { var_name, value };
        //Revisar si hay que comprobar si la expresion continua
        if (tokenstream.Position()==tokens.Count-1)
        {
            return globalvar;
        }
        tokenstream.MoveForward(1);
        return globalvar;
    }
    public Node GlobalSeq()
    {
        Node globalseq = new Node();
        globalseq.Type = Node.NodeType.GlobalSeq;
        Node Arguments = new Node();
        int count=0;
        
        do
        {   
            if (count>0)
            {
                tokenstream.MoveForward(1);
            }
            //Console.WriteLine(tokenstream.tokens[tokenstream.Position()].Type);
            if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.identifier && tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.low_hyphen)
            {
                
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "constant name or (_) symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            Node arg = new Node();
           
            if (tokenstream.tokens[tokenstream.Position()].Type == Token.TokenType.low_hyphen)
            {
                arg.Type = Node.NodeType.Low_Hyphen;
                arg.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
            }
            else
            {
                arg.Type = Node.NodeType.VarName;
                arg.NodeExpression = tokenstream.tokens[tokenstream.Position()].Value;
            }
            Console.WriteLine(tokenstream.tokens[tokenstream.Position()].Value);
            tokenstream.MoveForward(1);
            Arguments.Branches.Add(arg);
            count++;
        } while (tokenstream.tokens[tokenstream.Position()].Value == ",");
        if (tokenstream.tokens[tokenstream.Position()].Value != "=")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, " = symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        
        Node value = ParseExpression();
        globalseq.Branches = new List<Node> { Arguments, value };
        
        tokenstream.MoveForward(1);
        return globalseq;
    }

    public Node Draw()
    {
        tokenstream.MoveForward(1);
        Node draw = new Node();
        draw.Type = Node.NodeType.Draw;
        Node mainvalue=ParseExpression();
        Node text=new Node();
        //Revisar posicion en este punto
        // Console.WriteLine(tokenstream.tokens[tokenstream.Position()].Value);
        if (tokenstream.tokens[tokenstream.Position()].Value != ";")
        {
            text=ParseExpression();
           if(text.Type!=Node.NodeType.Text)
          {
             errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "text",tokenstream.tokens[tokenstream.Position()].TokenLocation));
          }
        }
        
        draw.Branches=new List<Node>{mainvalue,text};
        if (tokenstream.Position()==tokens.Count-1)
        {
            return draw;
        }
        tokenstream.MoveForward(1);
        return draw;
    }

    public Node IF_ElSE()
    {
        tokenstream.MoveForward(1);
        Node if_part = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != "then")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, " 'then' keyword",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node then_part = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != "else")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "'else' keyword",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node else_part = ParseExpression();
        Node conditional = new Node();
        conditional.Type = Node.NodeType.Conditional;
        conditional.Branches = new List<Node> { if_part, then_part, else_part };
        return conditional;
    }

    public Node PCircle()
    {
        tokenstream.MoveForward(2);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Circle_Fuc;
        Node center = ParseExpression();
        // center.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node radio = ParseExpression();
        // radio.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { center, radio };
        return arguments;
    }

    public Node PLine()
    {
        tokenstream.MoveForward(2);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Line_Fuc;
        Node p1 = ParseExpression();
        // p1.NodeExpression = ;
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node p2 = ParseExpression();
        // p2.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { p1, p2 };
        return arguments;
    }
    public Node PSegment()
    {
        tokenstream.MoveForward(2);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Segment_Fuc;
        Node p1 =ParseExpression();
        // p1.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node p2 = ParseExpression();
        // p2.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { p1, p2 };
        return arguments;
    }
    public Node PRay()
    {
        tokenstream.MoveForward(2);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Ray_Fuc;
        Node p1 = ParseExpression();
        // p1.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node p2 = ParseExpression();
        // p2.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { p1, p2 };
        return arguments;
    }
    public Node PMeasure()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.left_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Measure_Fuc;
        Node p1 = ParseExpression();
        // p1.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node p2 = ParseExpression();
        // p2.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { p1, p2 };
        return arguments;
    }
    public Node PIntersect()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.left_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Intersect;
        Node f1 = ParseExpression();
        //f1.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node f2 = ParseExpression();
        // f2.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { f1, f2 };
        return arguments;
    }
    public Node PArc()
    {
        tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.left_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node arguments = new Node();
        arguments.Type = Node.NodeType.Arc;
        Node p1 = ParseExpression();
        //p1.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node p2 = ParseExpression();
        //p2.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node p3 = ParseExpression();
        //p3.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Value != ",")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        Node m = ParseExpression();
        //m.NodeExpression = ParseExpression();
        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
        arguments.Branches = new List<Node> { p1, p2, p3, m };
        return arguments;
    }

    //Revisar este 
    public Node Let_In()
    {
        tokenstream.MoveForward(1);
        Node let_exp = new Node();
        
        let_exp.Type = Node.NodeType.Let_exp;
        Node instructions = new Node();
        instructions.Type = Node.NodeType.Instructions;
        do
        {   //Revisar que termine de parsear instruccion hasta ; y luego avance una posicion para que el ciclo acabe
            if (tokenstream.Position() >= tokens.Count - 1)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "let-in expression",new Location(tokenstream.tokens[0].TokenLocation.File,tokenstream.tokens[0].TokenLocation.Line,((tokens.Count)-1).ToString())));
                break;
            }
           
            Node instruction = ParseStatement();
            //instruction.NodeExpression = ParseStatement();
            
            instructions.Branches.Add(instruction);
            
        } while (tokenstream.tokens[tokenstream.Position()].Value != "in");
        //tokenstream.MoveForward(1);
        Node assigment_exp = new Node();
        assigment_exp.Type = Node.NodeType.Assigment;
        tokenstream.MoveForward(1);
       
        Node assig=ParseExpression();
        assigment_exp.Branches = new List<Node>{assig};
        let_exp.Branches = new List<Node> { instructions, assigment_exp };
        return let_exp;
    }

    public Node Function()
    {
        string name = tokenstream.tokens[tokenstream.Position()].Value;
        int original_index=tokenstream.Position();
        tokenstream.MoveForward(1);
        Node Arguments = new Node();
        Node function = new Node();

        if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
        {
            do
            {
                tokenstream.MoveForward(1);
                if (tokenstream.Position() >= tokens.Count - 1)
                {
                    errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",new Location(tokenstream.tokens[0].TokenLocation.File,tokenstream.tokens[0].TokenLocation.Line,((tokens.Count)-1).ToString())));
                    break;
                }
                Node arg=ParseExpression();
                //Cambiar este type
                // arg.Type=Node.NodeType.ParName;
                // Node val = ParseExpression();
                // arg.NodeExpression=val;
                Arguments.Branches.Add(arg);
            } while (tokenstream.tokens[tokenstream.Position()].Value == ",");
            if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);

        }
        else tokenstream.MoveForward(1);
        if (tokenstream.tokens[tokenstream.Position()].Value != "=")
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Expected, "'=' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
        }
        else tokenstream.MoveForward(1);
            function.Type = Node.NodeType.Fuction;
            Node function_name = new Node();
            function_name.Type = Node.NodeType.FucName;
            function_name.NodeExpression = name;
            Node body = ParseExpression();
            // Console.WriteLine(Arguments.Branches.Count);
            // Console.WriteLine(Arguments.Branches[0].Type);
            foreach (var item in Arguments.Branches)
            {
                if (item.Type!=Node.NodeType.Var)
                {
                    errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Invalid,"argument",tokenstream.tokens[tokenstream.Position()].TokenLocation));
                }
            }
            function.Branches = new List<Node> { function_name, Arguments, body };
            if (tokenstream.tokens[tokens.Count - 1].Value != ";")
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "statement",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            return function;

        // else
        // {
            
        //     // function.Type = Node.NodeType.Declared_Fuc;
        //     // Node func_name = new Node();
        //     // func_name.Type = Node.NodeType.Declared_FucName;
        //     // func_name.NodeExpression = name;
        //     // function.Branches = new List<Node> { func_name, Arguments };
        //     // return function;
        // }
    }

    public Node ParseOP()
    {
        Node left = ParseComparation();
        Node and_or = new Node();
        while (tokenstream.Position() < tokenstream.tokens.Count && (tokenstream.tokens[tokenstream.Position()].Type == Token.TokenType.Or || tokenstream.tokens[tokenstream.Position()].Type == Token.TokenType.And))
        {
            Token.TokenType whatkind = tokenstream.tokens[tokenstream.Position()].Type;
            tokenstream.MoveForward(1);
            Node right = ParseOP();
            if (right.NodeExpression is Error)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            if (whatkind == Token.TokenType.Or)
            {
                and_or.Type = Node.NodeType.Or;
            }
            else and_or.Type = Node.NodeType.And;
            and_or.Branches = new List<Node> { left, right };
        }
        if (and_or.Type != Node.NodeType.Indefined)
        {
            return and_or;
        }
        else return left;
    }

    public Node ParseComparation()
    {
        Node left = ParseSum_O_Sub();
        Node com = new Node();
        while (tokenstream.Position() < tokenstream.tokens.Count && (tokenstream.tokens[tokenstream.Position()].Value == "<" || tokenstream.tokens[tokenstream.Position()].Value == ">" || tokenstream.tokens[tokenstream.Position()].Value == ">=" || tokenstream.tokens[tokenstream.Position()].Value == "<=" || tokenstream.tokens[tokenstream.Position()].Value == "==" || tokenstream.tokens[tokenstream.Position()].Value == "!="))
        {

            Token.TokenType whatkind = tokenstream.tokens[tokenstream.Position()].Type;
            tokenstream.MoveForward(1);
            Node right = ParseComparation();
            if (right.NodeExpression is Error)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            if (whatkind == Token.TokenType.minor)
            {
                com.Type = Node.NodeType.Minor;
            }
            else if (whatkind == Token.TokenType.major)
            {
                com.Type = Node.NodeType.Major;
            }
            else if (whatkind == Token.TokenType.equal_major)
            {
                com.Type = Node.NodeType.Equal_Major;
            }
            else if (whatkind == Token.TokenType.equal_minor)
            {
                com.Type = Node.NodeType.Equal_Minor;
            }
            else if (whatkind == Token.TokenType.equal)
            {
                com.Type = Node.NodeType.Equal;
            }
            else com.Type = Node.NodeType.Diferent;
            com.Branches = new List<Node> { left, right };
        }
        if (com.Type != Node.NodeType.Indefined)
        {
            return com;
        }
        else return left;
    }
    public Node ParseSum_O_Sub()
    {
        Node left = ParseMul_O_Div();
        Node sus = new Node();
        while (tokenstream.Position() < tokenstream.tokens.Count && (tokenstream.tokens[tokenstream.Position()].Value == "+" || tokenstream.tokens[tokenstream.Position()].Value == "-"))
        {
            Token.TokenType whatkind = tokenstream.tokens[tokenstream.Position()].Type;
            tokenstream.MoveForward(1);
            Node right = ParseSum_O_Sub();
            if (right.NodeExpression is Error)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }

            if (whatkind == Token.TokenType.sum)
            {
                sus.Type = Node.NodeType.Sum;
                sus.NodeExpression = "+";
            }
            else sus.Type = Node.NodeType.Sub;
            sus.NodeExpression = "-";
            sus.Branches = new List<Node> { left, right };
        }
        if (sus.Type != Node.NodeType.Indefined)
        {
            return sus;
        }
        else return left;
    }
    public Node ParseMul_O_Div()
    {
        Node left = ParsePower();
        Node pro = new Node();
        while (tokenstream.Position() < tokenstream.tokens.Count && (tokenstream.tokens[tokenstream.Position()].Value == "*" || tokenstream.tokens[tokenstream.Position()].Value == "/" || tokenstream.tokens[tokenstream.Position()].Value == "%"))
        {

            Token.TokenType whatkind = tokenstream.tokens[tokenstream.Position()].Type;
            tokenstream.MoveForward(1);
            Node right = ParseMul_O_Div();
            if (right.NodeExpression is Error)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }

            if (whatkind == Token.TokenType.multiplication)
            {
                pro.Type = Node.NodeType.Mul;
            }
            else if (whatkind == Token.TokenType.module)
            {
                pro.Type = Node.NodeType.Module;
            }
            else pro.Type = Node.NodeType.Div;
            pro.Branches = new List<Node> { left, right };

        }
        if (pro.Type != Node.NodeType.Indefined)
        {
            return pro;
        }
        else return left;
    }
    public Node ParsePower()
    {
        Node left = Unit();
        Node pow = new Node();
        while (tokenstream.Position() < tokenstream.tokens.Count && tokenstream.tokens[tokenstream.Position()].Value == "^")
        {
            Token.TokenType whatkind = tokenstream.tokens[tokenstream.Position()].Type;
            tokenstream.MoveForward(1);
            Node right = ParsePower();
            if (right.NodeExpression is Error)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }

            pow.Type = Node.NodeType.Pow;
            pow.Branches = new List<Node> { left, right };
        }
        if (pow.Type != Node.NodeType.Indefined)
        {
            return pow;
        }
        else return left;
    }

    public Node Unit()
    {
        if (tokenstream.Position()>=tokenstream.tokens.Count)
        {
            errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Invalid,"expression",new Location(tokenstream.tokens[0].TokenLocation.File,tokenstream.tokens[0].TokenLocation.Line,((tokens.Count)-1).ToString())));
        }
        Token current = tokenstream.tokens[tokenstream.Position()];

        if (current.Type==Token.TokenType.left_bracket)
        {
            tokenstream.MoveForward(1);
            Node subnode=ParseExpression();
            if (tokenstream.Position()==tokenstream.tokens.Count-1&&tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
             tokenstream.MoveForward(1);
             return subnode;
        }
        if (current.Type==Token.TokenType.not)
        {
            tokenstream.MoveForward(1);
            Node value=ParseExpression();
            Node negation=new Node();
            negation.Type=Node.NodeType.Negation;
            negation.Branches=new List<Node>{value};
            return negation;
        }
     
        if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.number)
        {
            double value = (Convert.ToDouble(tokenstream.tokens[tokenstream.Position()].Value,CultureInfo.InvariantCulture));
            Node temp=new Node();
            temp.Type=Node.NodeType.Number;
            temp.NodeExpression=value;
            tokenstream.MoveForward(1);
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.text)
        {
           string value=(tokenstream.tokens[tokenstream.Position()].Value);
           Node temp=new Node();
           temp.Type=Node.NodeType.Text;
           temp.NodeExpression=value;
           tokenstream.MoveForward(1);
           return temp;
        }
        if (tokens[tokenstream.Position()].Type == Token.TokenType.identifier && tokens[tokenstream.Position() + 1].Type == Token.TokenType.left_bracket)
        {
            return Exp_Fuc();
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.PI)
        {
            Node temp=new Node();
            temp.Type=Node.NodeType.PI;
            temp.NodeExpression="PI";
            tokenstream.MoveForward(1);
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.E)
        {
            Node temp=new Node();
            temp.Type=Node.NodeType.E;
            temp.NodeExpression="E";
            tokenstream.MoveForward(1);
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.randoms)
        {
            if (tokenstream.tokens[tokenstream.Position()+1].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol after randoms function)",tokenstream.tokens[tokenstream.Position()+1].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()+1].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol to close randoms function)",tokenstream.tokens[tokenstream.Position()+1].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Randoms;
            temp.NodeExpression="randoms";
            tokenstream.MoveForward(1);
            return temp;
        }
        
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.sin)
        {
            tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node value= ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Sin;
            temp.Branches=new List<Node>{value};
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.cos)
        {
            tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node value= ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Cos;
            temp.Branches=new List<Node>{value};
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.sqrt)
        {
            tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node value= ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Sqrt;
            temp.Branches=new List<Node>{value};
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.points)
        {
            tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node value= ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Points;
            temp.Branches=new List<Node>{value};
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.log)
        {
            tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node base_of_log= ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Value!=",")
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"',' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node number = ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Log;
            temp.Branches=new List<Node>{base_of_log,number};
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.samples)
        {
            if (tokenstream.tokens[tokenstream.Position()+1].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol after samples function)",tokenstream.tokens[tokenstream.Position()+1].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()+1].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol to close samples function)",tokenstream.tokens[tokenstream.Position()+1].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Samples;
            temp.NodeExpression="samples";
            tokenstream.MoveForward(1);
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.count)
        {
            tokenstream.MoveForward(1);
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.left_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'(' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node value= ParseExpression();
            if (tokenstream.tokens[tokenstream.Position()].Type!=Token.TokenType.right_bracket)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"')' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            else tokenstream.MoveForward(1);
            Node temp=new Node();
            temp.Type=Node.NodeType.Count;
            temp.Branches=new List<Node>{value};
            return temp;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.identifier)
        {
            string value =tokenstream.tokens[tokenstream.Position()].Value;
            Node final =new Node();
            final.Type=Node.NodeType.Var;
            final.NodeExpression=value;
            Node location=new Node();
            location.NodeExpression=tokenstream.tokens[tokenstream.Position()].TokenLocation;
            final.Branches=new List<Node>{location};
            tokenstream.MoveForward(1);
            return final;
        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.left_key)
        {
            tokenstream.MoveForward(1);
            Node sequence=new Node();
            if (tokenstream.tokens[tokenstream.Position()].Type == Token.TokenType.right_key)
            {
                sequence.Type=Node.NodeType.Empty_Seq;
                Node value=new Node();
                value.Type=Node.NodeType.Undefined;
                value.NodeExpression="undefined";
                sequence.NodeExpression=value;
                tokenstream.MoveForward(1);
                return sequence;
            }
            Node firstvalue=ParseExpression();
            sequence.Branches.Add(firstvalue);
            if (tokenstream.tokens[tokenstream.Position()].Value == "...")
            {
                tokenstream.MoveForward(1);
                if (tokenstream.tokens[tokenstream.Position()].Type == Token.TokenType.right_key)
                {
                    sequence.Type=Node.NodeType.Infinite_Seq;
                    sequence.Branches=new List<Node>{firstvalue};
                    tokenstream.MoveForward(1);
                    return sequence;
                }
                Node finalvalue=ParseExpression();
                if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_key)
                {
                    errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Invalid,"sequence declaration",tokenstream.tokens[tokenstream.Position()].TokenLocation));
                }
                else tokenstream.MoveForward(1);
                sequence.Type=Node.NodeType.Enclosed_Infinite_Seq;
                sequence.Branches=new List<Node>{firstvalue,finalvalue};
                return sequence;
            }
            else if (tokenstream.tokens[tokenstream.Position()].Value == ",")
            {
                
                do
                {   tokenstream.MoveForward(1);
                    if(tokenstream.Position() >= tokens.Count - 1)
                   {
                    errors.Add(new Error(Error.TypeError.Syntactic_Error, Error.ErrorCode.Invalid, "expression",new Location(tokenstream.tokens[0].TokenLocation.File,tokenstream.tokens[0].TokenLocation.Line,((tokens.Count)-1).ToString())));
                    break;
                   }
                    Node value=ParseExpression();
                    sequence.Branches.Add(value);
                    
                } while (tokenstream.tokens[tokenstream.Position()].Value == ",");
                if(tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_key)
               {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"'}' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
               }
               else tokenstream.MoveForward(1);
               sequence.Type=Node.NodeType.Finite_Seq;
               return sequence;
            }
            else if (tokenstream.tokens[tokenstream.Position()].Type == Token.TokenType.right_key)
            {   tokenstream.MoveForward(1);
                sequence.Type=Node.NodeType.Finite_Seq;
               return sequence;
            }
            else if (tokenstream.tokens[tokenstream.Position()].Type != Token.TokenType.right_key)
            {
                errors.Add(new Error(Error.TypeError.Syntactic_Error,Error.ErrorCode.Expected,"',' or '...' symbol",tokenstream.tokens[tokenstream.Position()].TokenLocation));
            }
            
            return sequence;

        }
        else if (tokenstream.tokens[tokenstream.Position()].Type==Token.TokenType.undefined)
        {
            Node temp=new Node();
            temp.Type=Node.NodeType.Undefined;
            temp.NodeExpression="undefined";
            tokenstream.MoveForward(1);
            return temp;
        }
        else if(tokenstream.tokens[tokenstream.Position()]==null)
        {
            return new Node();
        }
        else
        {
            Node temp = new Node();
            temp.NodeExpression=new Error(Error.TypeError.Semantic_Error,Error.ErrorCode.Invalid,"expression",new Location(tokenstream.tokens[0].TokenLocation.File,tokenstream.tokens[0].TokenLocation.Line,((tokens.Count)-1).ToString()));
            return temp;
        }
    }
}