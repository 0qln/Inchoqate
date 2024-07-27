using System.Windows.Input;

namespace GUI.ViewModel
{
    public class KeyDownGesture : KeyGesture
    {
        public bool Pressed { get; private set; }

        public KeyDownGesture(Key key) : base(key)
        {
        }

        public KeyDownGesture(Key key, ModifierKeys modifiers) : base(key, modifiers)
        {
        }

        public KeyDownGesture(Key key, ModifierKeys modifiers, string displayString) : base(key, modifiers, displayString)
        {
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var matches = base.Matches(targetElement, inputEventArgs);

            if (!matches)
                return Pressed = false;

            if (!Pressed)
                return Pressed = true;

            return false;
        }
    }
}
