using Sorting;
using Sorting.Pixels;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;

namespace Inchoqate.GUI.ViewModel;

public class CombSorterViewModel : BaseViewModel, IViewModel<Sorter32Bit.ISorter>
{
    private readonly Sorter<Pixel32bitUnion>.CombSorter _model = new(new PixelComparer.Descending.Red());

    public Sorter<Pixel32bitUnion>.ISorter Model => _model;


    public IPixelComparer<Pixel32bitUnion> Comparer
    {
        get => _model.Comparer;
        set => SetProperty(() => _model.Comparer, value);
    }

    public Sorter32Bit.Threshold? Threshold
    {
        get => _model.Threshold;
        set => SetProperty(val => _model.Threshold = val, value);
    }

    public int? Pureness
    {
        get => _model.Pureness;
        set => SetProperty(val => _model.Pureness = val, value, validateValue: (_, val) => val > 0);
    }
}