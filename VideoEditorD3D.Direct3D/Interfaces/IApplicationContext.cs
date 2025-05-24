using Bsd.Logger;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplicationContext : IDisposable
{
    /// <summary>
    /// The main logger for the application. Does not need to be set for application to work.
    /// </summary>
    ILogger? Logger { get; }

    /// <summary>
    /// Retreives the default / start form of the application. This is because the Direct3D 
    /// engine has no knowledge of the application and it's forms. The application can create
    /// it's own forms and pass them to the engine. The engine will then use this form to
    /// draw the Direct3D context and pass load, resize, keyboard and mouse events to the form.
    /// This function is also used to signal the ApplicationForm instance to the application
    /// context.
    /// </summary>
    /// <param name="applicationForm">
    /// The ApplicationForm which holds the CurrentForm property.
    /// </param>
    /// <returns>
    /// The default / start form of the application.
    /// </returns>
    Forms.Form OnCreateMainForm(IApplicationForm applicationForm);

    /// <summary>
    /// Retreives the application's drawer thread, if there is one. Some applications 
    /// want to manage their own drawing calls, like a video player that wants to refresh
    /// according to the video frame rate. In this case the application can return it's own
    /// DrawerThread here. This function is also used to signal the ApplicationForm instance
    /// to the application context.
    /// 
    /// If you return null, the default 60fps DrawerThread will be created and used.
    /// </summary>
    /// <param name="applicationForm">
    /// The ApplicationForm which holds the TryDraw method.
    /// </param>
    /// <returns>
    /// The custom application's drawer thread. 
    /// Return null to use the default 60fps DrawerThread.
    /// </returns>
    IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm);

}