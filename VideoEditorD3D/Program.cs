namespace VideoEditorD3D;

public static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Direct3D.ApplicationForm(new Business.Application()));
    }
}