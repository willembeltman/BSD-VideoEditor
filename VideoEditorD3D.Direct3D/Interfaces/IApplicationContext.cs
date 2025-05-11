using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplicationContext : IDisposable
{
    /// <summary>
    /// The main logger for the application.
    /// </summary>
    ILogger? Logger { get; }

    /// <summary>
    /// This is the global switch for controlling when the application has to stop.
    /// All threads will exit their loops when this property is set to true. The DrawerThread
    /// will do the same, causing it to close the Direct3D ApplicationForm, which will in turn 
    /// dispose everything(including this application) making the frontend thread exit and 
    /// the application exit entirely.
    /// </summary>
    bool KillSwitch { get; set; }

    /// <summary>
    /// Retreives the application's drawer thread, if there is one. Some applications 
    /// want to manage their own drawing calls, like a video player that wants to refresh
    /// according to the video frame rate. In this case the application can return it's own
    /// DrawerThread here. This function is also used to signal the ApplicationForm instance
    /// to the application.
    /// 
    /// If you return null, the default 60fps DrawerThread will be created and used.
    /// </summary>
    /// <param name="applicationForm">
    /// The applicationForm which holds the Draw method.
    /// </param>
    /// <returns>
    /// The custom application's drawer thread. 
    /// Return null to use the default 60fps DrawerThread.
    /// </returns>
    IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm);

    /// <summary>
    /// Retreives the default / start form of the application. This is because the Direct3D 
    /// engine has no knowledge of the application and it's forms. The application can create
    /// it's own forms and pass them to the engine. The engine will then use this form to
    /// draw the Direct3D context and pass load, resize, keyboard and mouse events to the form.
    /// This function is also used to signal the ApplicationForm instance to the application.
    /// </summary>
    /// <param name="applicationForm">
    /// The applicationForm which holds the CurrentForm property.
    /// </param>
    /// <returns>
    /// The default / start form of the application.
    /// </returns>
    Forms.Form OnCreateStartForm(IApplicationForm applicationForm);

    /// <summary>
    /// This method is called when the Direct3D engine is ready for drawing and it has started
    /// it's stopwatch to start drawing. The application can use this method to start 
    /// it's own threads and initialize it's own engine. This call is blocking the render thread,
    /// so after this method has returned the Direct3D engine will begin drawing it's first frame,
    /// </summary>
    void Start(IApplicationForm applicationForm);
}