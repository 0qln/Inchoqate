using Inchoqate.GUI.Model;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class EditorNodeCollectionDynamic
        : ObservableCollection<EditBaseDynamic>
    {
    }

    public class EditorNodeCollectionLinear
        : ObservableCollection<EditBaseLinear>
    {
        public EditorNodeCollectionLinear(List<EditBaseLinear> list) : base(list)
        {
        }
    }
}
