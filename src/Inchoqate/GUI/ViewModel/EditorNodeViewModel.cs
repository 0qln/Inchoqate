using Inchoqate.GUI.Model;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.ViewModel
{
    //public class EditorNodeViewModel<TEdit> : BaseViewModel
    //    where TEdit : IEditModel
    //{
    //    private readonly TEdit _edit;

    //    public required TEdit Edit
    //    {
    //        get => _edit;
    //        init
    //        {
    //            _edit = value;
    //            Title = _edit.Title;
    //        }
    //    }
    //}

    //public class EditorNodeDynamicViewModel
    //    : EditorNodeViewModel<EditBaseDynamic>
    //{
    //}

    //public class EditorNodeLinearViewModel 
    //    : EditorNodeViewModel<EditBaseLinear>
    //{
    //}


    public class ObservableItemCollection<T> : ObservableCollection<T>
            where T : INotifyPropertyChanged
    {
        public ObservableItemCollection() : base()
        {
            CollectionChanged += ObservableItemCollection_CollectionChanged;
        }

        private void ObservableItemCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // The entire collection could be changed, thus we pass the 'Reset' flag.
            OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
        }
    }


    public class EditorNodeCollectionDynamic
        : ObservableItemCollection<EditBaseDynamic>
    {
    }


    public class EditorNodeCollectionLinear
        : ObservableItemCollection<EditBaseLinear>
    {
    }
}
