using Inchoqate.GUI.Model;
using Sorting;
using Sorting.Pixels._32;

namespace Inchoqate.GUI.ViewModel.Events;

public class SorterChangedEvent :
    PropertyChangedEvent<ISorterProperty, Sorter<Pixel32bitUnion>.ISorter>,
    IDeserializable<SorterChangedEvent>
{
    protected override void Setter(ISorterProperty prop, Sorter32Bit.ISorter? val) => prop.Sorter = val;
}