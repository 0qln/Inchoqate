using Inchoqate.GUI.Model;
using MvvmHelpers;
using System.Windows;

namespace Inchoqate.GUI.ViewModel
{
    public class StackEditorViewModel : BaseViewModel
    {
        private GpuEditQueueModel _editor = new();

        public EditorNodeCollectionLinear Nodes
        {
            get => new(_editor.Edits);
        }

        public GpuEditQueueModel QueueEditor
        {
            get => _editor;
            set => SetProperty(ref _editor, value);
        }


        public StackEditorViewModel()
        {
            // test
            Nodes.Add(new EditImplGrayscaleViewModel());
        }
    }
}
