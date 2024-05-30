namespace Inchoqate.GUI.Model
{
    public interface IEdit
    {
    }

    public interface IEdit<TSource, TDesination> : IEdit
    {
        /// <summary>
        /// Apply's the edit to the contents of <paramref name="source"/> and writes
        /// into the buffer <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">The source data.</param>
        /// <param name="destination">The destinaiton buffer.</param>
        bool Apply(TSource source, TDesination destination);
    }
}
