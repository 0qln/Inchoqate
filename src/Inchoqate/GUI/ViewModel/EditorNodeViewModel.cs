using Inchoqate.GUI.Model;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeViewModel<TEdit> : BaseViewModel
        where TEdit : IEditModel
    {
        public required TEdit Edit { get; init; }
    }
}
