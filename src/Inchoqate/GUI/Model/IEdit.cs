namespace Inchoqate.GUI.Model
{
    public interface IEdit<TSource, TDesination>
        where TSource : IEditSource 
        where TDesination : IEditDestination
    {
        public int ExpectedInputCount { get; }

        /// <summary>
        /// Apply's the edit to the contents of <paramref name="sources"/> and writes
        /// into the buffer <paramref name="destination"/>.
        /// </summary>
        /// <param name="sources">The source data.</param>
        /// <param name="destination">The destinaiton buffer.</param>
        bool Apply(TDesination destination, params TSource[] sources);
    }

    public interface IEdit : IEdit<IEditSource, IEditDestination>
    {
    }

    public abstract class LinearEdit<TSource, TDestination> : IEdit<TSource, TDestination> 
        where TSource : IEditSource 
        where TDestination : IEditDestination
    {
        public int ExpectedInputCount => 1;

        public abstract bool Apply(TDestination destination, params TSource[] sources);
    }

}
