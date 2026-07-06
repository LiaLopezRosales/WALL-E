using Wall_E.Domain;
using Wall_E.Application.DSL;
using Wall_E.Application.Interfaces;
using Wall_E.Application.Caching;

namespace Wall_E.Application.Pipeline;

public class PipelineOrchestrator : IPipeline
{
    private readonly ExpressionCache _cache;
    private readonly List<Error> _errors = new();
    private CancellationTokenSource? _cts;

    public List<Error> Errors => _errors;
    public ExpressionCache Cache => _cache;

    public PipelineOrchestrator()
    {
        _cache = new ExpressionCache();
    }

    public void Execute(string source, string file)
    {
        _errors.Clear();
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        var generalLexer = new GeneralLexer(source, file);

        List<List<Token>> allTokens = new();
        foreach (string line in generalLexer.lines)
        {
            token.ThrowIfCancellationRequested();
            var lexer = new Lexer(file, line);
            var tokens = lexer.Tokens(line);
            allTokens.Add(tokens);
            _errors.AddRange(lexer.lexererrors);
        }

        if (_errors.Count > 0) return;

        var generalParser = new GeneralParser(allTokens, file);
        var trees = generalParser.ParseArchive();
        foreach (var errorList in generalParser.ParserErrors())
            _errors.AddRange(errorList);

        if (_errors.Count > 0) return;

        var context = new EvaluationContext();
        var figures = new FigureRepository();
        var scene = new RenderScene();

        var evaluator = new EvaluatorVisitor(context, figures, scene, file);
        evaluator.CancellationToken = token;

        int count = 0;
        foreach (var node in trees)
        {
            token.ThrowIfCancellationRequested();
            evaluator.SetLine(count.ToString());
            var result = evaluator.Visit(node);
            context.Results.Add(result);
            _errors.AddRange(evaluator.SemanticErrors);
            count++;
        }

        context.HasErrors = _errors.Count > 0;
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }
}
