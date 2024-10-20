using Sorting;
using Sorting.Pixels;
using Sorting.Pixels._32;
using Sorting.Pixels.Comparer;

namespace Inchoqate.GUI.ViewModel.Edits;

public class CombSorterSorterViewModel : BaseViewModel, ISorterViewModel
{
    private readonly Sorter32Bit.CombSorter _model = new(new PixelComparer.Descending.Red());

    public Sorter32Bit.ISorter Sorter => _model;


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