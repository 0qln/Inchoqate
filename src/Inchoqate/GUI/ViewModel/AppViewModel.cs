using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;

namespace Inchoqate.GUI.ViewModel
{
    public class AppViewModel : ObservableObject
    {
        private ProjectViewModel? _project;

        public ProjectViewModel? Project
        {
            get => _project;
            set => SetProperty(ref _project, value);
        }
    }
}
