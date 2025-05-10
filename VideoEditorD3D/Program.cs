namespace VideoEditorD3D;

public static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Thread.CurrentThread.Name = "Forms thread";
        System.Windows.Forms.Application.Run(new Direct3D.ApplicationForm(new Application.ApplicationContext()));
    }
}