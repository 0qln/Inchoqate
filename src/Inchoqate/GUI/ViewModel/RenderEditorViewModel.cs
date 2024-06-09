using CommunityToolkit.Mvvm.ComponentModel;
using Inchoqate.GUI.Events;
using Inchoqate.GUI.Model;
using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI.ViewModel
{
    public abstract class RenderEditorViewModel : ObservableObject, IEditorModel<TextureModel, FrameBufferModel>, IEventHost
    {
        protected bool _computed;
        protected FrameBufferModel? _result;
        protected Size _renderSize;
        private Color _voidColor;

        public Events.EventManager EventManager { get; } = new();


        public Color VoidColor
        {
            get => _voidColor;
            set => SetProperty(ref _voidColor, value);
        }

        public Size RenderSize
        {
            get => _renderSize;
            set => SetProperty(ref _renderSize, value);
        }

        public bool Computed
        {
            get => _computed;
            protected set => SetProperty(ref _computed, value);
        }

        public FrameBufferModel? Result
        {
            get => _result;
            protected set => SetProperty(ref _result, value);
        }


        public RenderEditorViewModel()
        {
            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Result):
                        Computed = Result is not null;
                        break;

                    case nameof(RenderSize):
                        Invalidate();
                        break;
                }
            };
        }


        public abstract void Invalidate();

        public abstract bool Compute();

        public abstract void SetSource(TextureModel? source);
    }
}
