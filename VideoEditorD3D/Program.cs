namespace VideoEditorD3D;

public static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new Direct3D.ApplicationForm(new Application.ApplicationContext())); // Yes this is a flex
    }
}