using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.Model;

public interface IIntensityProperty
{
    public double Intensity { get; set; }
}