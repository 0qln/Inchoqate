using System.Windows;
using System.Windows.Data;
using Inchoqate.GUI.View.Converters;

namespace Inchoqate.GUI.View.DisplayComboBox;

public class DisplayComboBoxItemToNameConverter() : SelectConverter<DisplayComboBoxItem, string>(item => item.Name);