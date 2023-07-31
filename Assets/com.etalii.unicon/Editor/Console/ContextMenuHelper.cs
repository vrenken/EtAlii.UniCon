namespace EtAlii.UniCon.Editor
{
    using System;
    using UnityEngine.UIElements;

    internal class ContextMenuHelper
    {
        /// <summary>
        /// Helper method to register a (left-mouse enabled) context menu for the specified visual element.
        /// Please note that the ElementAwareContextualMenuManipulator does some nifty layout magic to ensure
        /// that the context menu shows up below the visual element.
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="evt"></param>
        public static void Register(VisualElement visualElement, Action<ContextualMenuPopulateEvent> evt)
        {
            var manipulator = new ElementAwareContextualMenuManipulator(evt)
            {
                target = visualElement
            };
            manipulator.activators.Clear();
            manipulator.activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
            visualElement.AddManipulator(manipulator);
        }
    }
}