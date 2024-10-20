using Sorting;

namespace Inchoqate.GUI.Model;

public interface ISorterProperty : IProperty
{
    public Sorter32Bit.ISorter? Sorter { get; set; }
}