namespace Inchoqate.GUI.Model
{
    public interface IEditResult<out TResult>
        where TResult : IEditDestination
    {
        /// <summary>
        /// Returns a reference to the final result of the render queue.
        /// Returns null if there is no source given.
        /// </summary>
        /// <returns></returns>
        public TResult? Compute(out bool success);
    }

    public interface IEditResult : IEditResult<IEditDestination>
    {
    }
}
