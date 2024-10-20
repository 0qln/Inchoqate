using Inchoqate.GUI.Model.Graphics;

namespace Inchoqate.GUI.ViewModel.Edits;

public abstract class EditBaseViewModel : BaseViewModel, IEdit
{
    public abstract int ExpectedInputCount { get; }

    public abstract bool Apply();

    public override string ToString()
    {
        return Title ?? GetType().Name;
    }
}