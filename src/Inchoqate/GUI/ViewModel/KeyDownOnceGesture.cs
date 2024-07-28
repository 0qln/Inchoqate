using System.Windows.Input;
using System.Timers;
using System.Windows;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel;

[Obsolete]
public class KeyDownOnceGesture : KeyGesture
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<KeyOnceAndHoldGesture>();

    public bool Activated { get; private set; }

    private bool _wasActivated;


    public KeyDownOnceGesture(Key key, ModifierKeys modifiers, Action<KeyEventHandler> addKeyDownHandler, Action<KeyEventHandler> addKeyUpHandler) 
        : base(key, modifiers)
    {
        addKeyDownHandler((s, e) =>
        {
            if (e.Key == key && Keyboard.Modifiers == modifiers)
            {
                if (Activated && !_wasActivated)
                {
                    Activated = false;
                }
                else
                {
                    Activated = true;
                    _wasActivated = true;
                }
            }
        });
        addKeyUpHandler((s, e) =>
        {
            if (e.Key == key && Keyboard.Modifiers == modifiers)
            {
                Activated = false;
                _wasActivated = false;
            }
        });
    }


    public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
    {
        return Activated;
    }
}