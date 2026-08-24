using Wall_E.Domain;
using Wall_E.Application.DSL;
using Wall_E.Application.Interfaces;
using Wall_E.Application.Caching;

namespace Wall_E.Application.Pipeline;

public class PipelineOrchestrator : IPipeline
{
    private readonly ExpressionCache _cache;
    private readonly List<Error> _errors = new();
    private readonly IGeoLibrarySource? _librarySource;
    private CancellationTokenSource? _cts;

    public List<Error> Errors => _errors;
    public ExpressionCache Cache => _cache;
    public EvaluationContext Context { get; private set; } = new();
    public FigureRepository Figures { get; private set; } = new();
    public RenderScene Scene { get; private set; } = new();

    /// <summary>Optional initial ink (DSL color name or "#RRGGBB") pushed onto
    /// the scene's color stack when a run starts, so programs without an
    /// explicit `color` statement draw with it. Pushing over the black base
    /// keeps `restore;` returning to black. Null = default (black).</summary>
    public string? InitialInk { get; set; }

    public PipelineOrchestrator(IGeoLibrarySource? librarySource = null)
    {
        _librarySource = librarySource;
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
        if (!string.IsNullOrWhiteSpace(InitialInk))
            scene.PushColor(InitialInk);
        Context = context;
        Figures = figures;
        Scene = scene;

        var evaluator = new EvaluatorVisitor(context, figures, scene, file);
        evaluator.CancellationToken = token;

        // Import wiring (T3): libraries share this visitor and context so their
        // definitions become available to the main program. Results produced
        // inside a library are deliberately NOT added to Context.Results.
        // Only wired when a source exists; otherwise VisitImport keeps reporting
        // that imports require the UI/Infrastructure layer.
        if (_librarySource is not null)
        {
            var loadingLibraries = new HashSet<string>();
            evaluator.ImportHandler = name =>
            {
                if (!loadingLibraries.Add(name))
                    return ImportFailure($"circular import of library '{name}'", file);
                try
                {
                    string? content = _librarySource.Resolve(name);
                    if (content is null)
                        return ImportFailure($"library '{name}' not found in GeoLibrary", file);
                    EvaluateImportContent(content, name, evaluator, context);
                    return new VoidResult();
                }
                finally
                {
                    loadingLibraries.Remove(name);
                }
            };
        }

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

    // Import failures are reported on both channels: pipeline.Errors (for the
    // UI error list) and as the statement's ErrorResult.
    private ErrorResult ImportFailure(string message, string file)
    {
        var error = new Error(Error.TypeError.Semantic_Error, Error.ErrorCode.Invalid,
            message, new Location(file, "0", "-1"));
        _errors.Add(error);
        return new ErrorResult(error);
    }

    // Same front half as Execute (lex + parse), without cancellation checks:
    // library files are bounded. Errors accumulate in the pipeline's list.
    private void EvaluateImportContent(string content, string libName, EvaluatorVisitor evaluator, EvaluationContext context)
    {
        var generalLexer = new GeneralLexer(content, libName);
        var allTokens = new List<List<Token>>();
        foreach (string line in generalLexer.lines)
        {
            var lexer = new Lexer(libName, line);
            var tokens = lexer.Tokens(line);
            allTokens.Add(tokens);
            _errors.AddRange(lexer.lexererrors);
        }
        if (_errors.Count > 0) return;

        var generalParser = new GeneralParser(allTokens, libName);
        var trees = generalParser.ParseArchive();
        foreach (var errorList in generalParser.ParserErrors())
            _errors.AddRange(errorList);
        if (_errors.Count > 0) return;

        int count = 0;
        foreach (var node in trees)
        {
            evaluator.SetLine($"import:{libName}:{count}");
            evaluator.Visit(node);
            _errors.AddRange(evaluator.SemanticErrors);
            count++;
        }
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }
}
