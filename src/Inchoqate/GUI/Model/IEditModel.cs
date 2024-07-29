namespace Inchoqate.GUI.Model;

public interface IEditModel
{
    public int ExpectedInputCount { get; }

    /// <summary>
    /// Apply's the edit to the contents of <paramref name="sources"/> and writes
    /// into the buffer <paramref name="destination"/>.
    /// </summary>
    /// <param name="sources">The source data.</param>
    /// <param name="destination">The destinaiton buffer.</param>
    bool Apply(IEditDestinationModel destination, params IEditSourceModel [] sources);
}