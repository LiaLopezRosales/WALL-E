//Implementar lógica para recibir el archivo
//Clase principal que interpreta un archivo recibido
public class ArchiveAnalysis
{
    string Code { get; set; }
    string File { get; set; }
    //Se recibe el nombre del archivo y su contenido como string
    public ArchiveAnalysis(string code, string file)
    {
        Code = code;
        File = file;
    }
    //Devuelve un contexto modificado por el análisis hecho en el archivo indicado
    public Context Analyze(Context basecontext)
    {   //Se divide el código en expresiones delimitadas por ;(excepto el caso especial de expresión let-in)
        GeneralLexer startlexing = new GeneralLexer(Code, File);
        List<List<Token>> tokenizedcode = startlexing.Process(startlexing.lines);
        List<List<Error>> lexicalerrors = startlexing.LexicalErrors();
        //Se encuentra errores léxicos los imprime e interrumpe el proceso
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
        //De lo contrario procede a realizar el análisis sintáctico
        else
        {   //Se realizan procesos similares al léxico para el parser y la evaluación 
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