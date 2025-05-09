using VideoEditorD3D.Direct3D;

namespace VideoEditorD3D;

public static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new ApplicationForm(new Application()));
    }
}