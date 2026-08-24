using Wall_E.Application.Pipeline;

namespace Wall_E.Application.Tests;

public static class DslRunner
{
    public static PipelineOrchestrator Run(string code)
    {
        var pipeline = new PipelineOrchestrator();
        pipeline.Execute(code, "test");
        return pipeline;
    }

    public static PipelineOrchestrator Run(string code, PipelineOrchestrator pipeline)
    {
        pipeline.Execute(code, "test");
        return pipeline;
    }
}
