using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.ViewModel;
using Inchoqate.GUI.ViewModel.Editors;
using Inchoqate.GUI.ViewModel.Editors.StackEditor;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.Model;

public class ProjectModel : IDeserializable<ProjectModel>
{
    /// <summary>
    /// The initial event from the 
    /// </summary>
    public EventModel? State
    { 
        get;
        // Serialization
        set;
    }

    // todo: these dependencies should have as few state caching as possible.
    // todo: or introduce models for the dependencies instead?

    #region Dependencies

    // The setters exist for serialization and should not be used after initialization.

    public StackEditorViewModel? StackEditor { get; set; }

    public PreviewImageViewModel? PreviewImage { get; set; }

    #endregion
}