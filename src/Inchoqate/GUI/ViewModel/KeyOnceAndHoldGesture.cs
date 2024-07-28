using System.Windows.Input;
using System.Windows.Threading;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel;

public class KeyOnceAndHoldGesture : KeyGesture
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<KeyOnceAndHoldGesture>();

    private readonly DispatcherTimer _timer = new();

    public TimeSpan ActivationDelay
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    } 

    public bool Active { get; private set; }
        

    public KeyOnceAndHoldGesture(Key key) : base(key)
    {
        Initiate();
    }

    public KeyOnceAndHoldGesture(Key key, ModifierKeys modifiers) : base(key, modifiers)
    {
        Initiate();
    }

    public KeyOnceAndHoldGesture(Key key, ModifierKeys modifiers, string displayString) : base(key, modifiers, displayString)
    {
        Initiate();
    }

    private void Initiate()
    {
        ActivationDelay = TimeSpan.FromMilliseconds(400);

        _timer.Tick += delegate
        {
            Active = true;
            _timer.Stop();
        };
    }

    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
        var keyEventArgs = inputEventArgs as KeyEventArgs;

        if (keyEventArgs is null) 
            return false;

        if (!base.Matches(targetElement, inputEventArgs)) 
            return Active = false;

        if (_timer.IsEnabled || !keyEventArgs.IsToggled)
            return Active = false;

        if (Active) 
            return true;

        _timer.Start();
        return false;

    }
}