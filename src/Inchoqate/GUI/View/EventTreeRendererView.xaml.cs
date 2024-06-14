using Inchoqate.GUI.Events;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for EventTreeNodeView.xaml
    /// </summary>
    public partial class EventTreeRendererView : UserControl
    {
        public static readonly DependencyProperty InitialEventProperty = 
            DependencyProperty.Register(
                "InitialEvent",
                typeof(Event),
                typeof(EventTreeRendererView),
                new FrameworkPropertyMetadata(
                    null, 
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Event InitialEvent
        {
            get => (Event)GetValue(InitialEventProperty);
            set => SetValue(InitialEventProperty, value);
        }


        public EventTreeRendererView()
        {
            InitializeComponent();
        }
    }


    public class EventTreeNextNodesConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new ObservableCollection<EventTreeRendererView>();
            if (value is null) { return result; }
            var @event = (Event)value;
            foreach (var nextEvent in @event.Next)
            {
                result.Add(new EventTreeRendererView { InitialEvent = nextEvent.Value });
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EventArgsInfoConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new ObservableCollection<string>();
            if (value is null) { return result; }
            foreach (var arg in value.GetType().GetProperties())
            {
                string argV = arg.GetValue(value)?.ToString() ?? "";
                string argN = arg.Name;
                result.Add($"{argN}: {argV}");
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
