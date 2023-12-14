//Implementar lógica para recibir el archivo
public class ArchiveAnalysis
{
    string Code { get; set; }
    string File { get; set; }
    public ArchiveAnalysis(string code, string file)
    {
        Code = code;
        File = file;
    }
    public Context Analyze(Context basecontext)
    {
        GeneralLexer startlexing = new GeneralLexer(Code, File);
        List<List<Token>> tokenizedcode = startlexing.Process(startlexing.lines);
        List<List<Error>> lexicalerrors = startlexing.LexicalErrors();
        if (lexicalerrors.Count > 0)
        {
            foreach (var item in lexicalerrors)
            {
                foreach (var error in item)
                {
                    Console.WriteLine(error.ToString());
                }
            }
            basecontext.issuedcontext = true;
            return basecontext;
        }
        else
        {
            GeneralParser startparsing = new GeneralParser(tokenizedcode, File);
            List<Node> parsedcode = startparsing.ParseArchive();
            List<List<Error>> syntacticerrors = startparsing.ParserErrors();
            if (syntacticerrors.Count > 0)
            {
                foreach (var item in syntacticerrors)
                {
                    foreach (var error in item)
                    {
                        Console.WriteLine(error.ToString());
                    }
                }
                basecontext.issuedcontext = true;
                return basecontext;
            }
            else
            {
                GeneralEvaluation startevaluation = new GeneralEvaluation(parsedcode, File);
                Context result = startevaluation.EvaluateArchive(basecontext);
                List<List<Error>> semanticerrors = startevaluation.Semantic_Errors();
                if (semanticerrors.Count > 0)
                {
                    foreach (var item in semanticerrors)
                    {
                        foreach (var error in item)
                        {
                            Console.WriteLine(error.ToString());
                        }
                    }
                    basecontext.issuedcontext = true;
                    return basecontext;
                }
                else
                {
                    return result;
                }
            }
        }
    }
}