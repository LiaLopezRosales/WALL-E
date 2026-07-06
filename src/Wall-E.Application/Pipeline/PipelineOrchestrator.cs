using Wall_E.Domain;
using Wall_E.Application.DSL;
using Wall_E.Application.Interfaces;

namespace Wall_E.Application.Pipeline;

public class PipelineOrchestrator : IPipeline
{
    private readonly List<Error> _errors = new();

    public List<Error> Errors => _errors;

    public void Execute(string source, string file)
    {
        _errors.Clear();

        var generalLexer = new GeneralLexer(source, file);

        List<List<Token>> allTokens = new();
        foreach (string line in generalLexer.lines)
        {
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

        // TODO: wire evaluation with EvaluatorVisitor
    }
}
