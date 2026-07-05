namespace Wall_E.Domain;

public class RenderScene
{
    public List<DrawObject> ToDraw { get; set; } = new();
    public Stack<string> UtilizedColors { get; set; } = new();

    public RenderScene()
    {
        UtilizedColors.Push("black");
    }

    public void Clear()
    {
        ToDraw.Clear();
        UtilizedColors.Clear();
        UtilizedColors.Push("black");
    }
}
