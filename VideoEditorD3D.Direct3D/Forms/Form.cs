using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Form : BackControl
{
    public Form(IApplicationForm applicationForm) : base(applicationForm)
    {
        // Override the base Controls with a special `Form`-ControlCollection
        // (specified by the (this, this) constructor). This is necessary
        // because the current one, created by the base constructor is broken
        // because it thinks `ParentForm` is null, which is not true, because:
        //
        // I'm the parent form, baby :)
        Controls = new ControlCollection(this, this);
    }
}
