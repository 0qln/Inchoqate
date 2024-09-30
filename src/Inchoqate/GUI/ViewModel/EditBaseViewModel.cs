﻿using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.Graphics;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel;

public abstract class EditBaseViewModel : BaseViewModel, IEdit
{
    [JsonIgnore]
    public abstract ObservableCollection<(Control, string)> OptionControls { get; }

    public abstract int ExpectedInputCount { get; }

    public abstract bool Apply();

    public override string ToString()
    {
        return Title ?? GetType().Name;
    }
}