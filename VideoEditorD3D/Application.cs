using VideoEditorD3D.Configs;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Forms;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D
{
    public class Application : IApplication
    {
        public ILogger Logger { get; }
        public ApplicationConfig Config { get; }
        public ApplicationDbContext Db { get; }
        public bool KillSwitch { get; set; }
        public IApplicationForm? ApplicationForm { get; private set; }

        public Application()
        {
            Logger = new ConsoleLogger();
            Config = ApplicationConfig.Load();
            if (Config.LastDatabaseFullName == null)
            {
                Db = new ApplicationDbContext($"NewProject_{DateTime.Now:yyyy-MM-dd HH-mm}.zip");
                Config.LastDatabaseFullName = Db.FullName;
                Config.Save();
            }
            else
            {
                Db = new ApplicationDbContext(Config.LastDatabaseFullName);
            }
        }

        public IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm)
        {
            ApplicationForm = applicationForm;
            return new DrawerThread(applicationForm, this);
        }
        public Direct3D.Forms.Form OnCreateStartForm(IApplicationForm applicationForm)
        {
            ApplicationForm = applicationForm;
            return new MainForm(this, applicationForm);
        }

        public void Dispose()
        {
            KillSwitch = true;
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
