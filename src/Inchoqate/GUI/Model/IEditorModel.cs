using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI.Model
{
    public interface IEditorModel<in TSource, out TResult>
        where TResult : IEditDestinationModel
        where TSource : IEditSourceModel
    {
        /// <summary>
        /// Compute the result of the render queue.
        /// </summary>
        /// <returns>True if the result was computed.</returns>
        public bool Compute();

        /// <summary>
        /// Clear the result of the render queue.
        /// </summary>
        /// <returns></returns>
        public void Invalidate();

        /// <summary>
        /// Wether the result is computed or not.
        /// </summary>
        public bool Computed { get; }

        /// <summary>
        /// The final result of the render queue.
        /// </summary>
        public TResult? Result { get; }

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
}
