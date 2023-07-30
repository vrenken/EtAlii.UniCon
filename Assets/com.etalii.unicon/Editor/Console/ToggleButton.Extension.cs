namespace EtAlii.UniCon.Editor
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class ToggleButtonExtension
    {
    
        private static Color _buttonNotToggledColor;
        private static Color _buttonToggledColor;

        public static void Init(Button button)
        {
            // Let's take the color of the tail button and use that to remember the toggled and not toggled colors.
            _buttonNotToggledColor = button.style.backgroundColor.value;
            _buttonToggledColor = new Color(
                0.5f - _buttonNotToggledColor.r, 
                0.5f - _buttonNotToggledColor.g,
                0.5f - _buttonNotToggledColor.b, 
                0.5f - _buttonNotToggledColor.a);

        }
        public static void UpdateToggleButton(this Button button, bool isToggled)
        {
            button.style.backgroundColor = isToggled 
                ? _buttonToggledColor 
                : _buttonNotToggledColor;
        }
    }
}