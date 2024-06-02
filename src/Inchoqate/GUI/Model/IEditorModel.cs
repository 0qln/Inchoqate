using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI.Model
{
    public interface IEditorModel<in TSource, out TResult>
        where TResult : IEditDestinationModel
        where TSource : IEditSourceModel
    {
        event NotifyCollectionChangedEventHandler? EditsChanged;

        /// <summary>
        /// Returns a reference to the final result of the render queue.
        /// Returns null if there is no source given.
        /// </summary>
        /// <returns></returns>
        public TResult? Compute(out bool success);

        /// <summary>
        /// Set the source to be used.
        /// </summary>
        /// <param name="source"></param>
        public void SetSource(TSource? source);

        /// <summary>
        /// The size in which the final output is rendered.
        /// </summary>
        public Size RenderSize { get; set; }

        /// <summary>
        /// The color to use for void pixels.
        /// </summary>
        public Color VoidColor { get; set; }
    }

    public interface IRenderEditorModel : IEditorModel<TextureModel, FrameBufferModel>
    {
    }
}
