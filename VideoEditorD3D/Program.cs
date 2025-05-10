using VideoEditorD3D.Direct3D;

namespace VideoEditorD3D;

public static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new ApplicationForm(new Application()));
    }
}