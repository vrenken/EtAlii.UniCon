namespace EtAlii.UniCon.Editor
{
  using System;
  using System.Reflection;
  using UnityEngine;
  using UnityEngine.UIElements;

  /// <summary>
  ///        <para>
  /// Manipulator that displays a contextual menu when the user clicks the right mouse button or presses the menu key on the keyboard.
  /// </para>
  ///      </summary>
  public class ElementAwareContextualMenuManipulator : MouseManipulator
  {
    private readonly Action<ContextualMenuPopulateEvent> _menuBuilder;

    public ElementAwareContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
    {
      _menuBuilder = menuBuilder;
      activators.Add(new ManipulatorActivationFilter
      {
        button = MouseButton.RightMouse
      });
      if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer)
        return;
      activators.Add(new ManipulatorActivationFilter
      {
        button = MouseButton.LeftMouse,
        modifiers = EventModifiers.Control
      });
    }

    /// <summary>
    ///        <para>
    /// Register the event callbacks on the manipulator target.
    /// </para>
    ///      </summary>
    protected override void RegisterCallbacksOnTarget()
    {
      if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
      {
        target.RegisterCallback<MouseDownEvent>(OnMouseDownEventOSX);
        target.RegisterCallback<MouseUpEvent>(OnMouseUpEventOSX);
      }
      else
        target.RegisterCallback<MouseUpEvent>(OnMouseUpDownEvent);

      target.RegisterCallback<KeyUpEvent>(OnKeyUpEvent);
      target.RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
    }

    /// <summary>
    ///        <para>
    /// Unregister the event callbacks from the manipulator target.
    /// </para>
    ///      </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
      if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
      {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDownEventOSX);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUpEventOSX);
      }
      else
      {
        target.UnregisterCallback<MouseUpEvent>(OnMouseUpDownEvent);
      }

      target.UnregisterCallback<KeyUpEvent>(OnKeyUpEvent);
      target.UnregisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
    }

    private void OnMouseUpDownEvent(IMouseEvent evt)
    {
      if (!CanStartManipulation(evt))
        return;
      evt = MouseUpEvent.GetPooled(new Vector2(target.worldBound.xMin + 5, target.worldBound.yMax + 2), evt.button, evt.clickCount, evt.mouseDelta, evt.modifiers);
      DoDisplayMenu((EventBase)evt);
    }

    private void OnMouseDownEventOSX(MouseDownEvent evt)
    {
      if (target.panel?.contextualMenuManager != null)
      {
        var property = target.panel.contextualMenuManager.GetType().GetProperty("displayMenuHandledOSX", BindingFlags.NonPublic | BindingFlags.Instance)!;
        property.SetValue(target.panel.contextualMenuManager, false);
        // target.panel.contextualMenuManager.displayMenuHandledOSX = false;
      }

      if (evt.isDefaultPrevented)
        return;
      OnMouseUpDownEvent(evt);
    }

    private void OnMouseUpEventOSX(MouseUpEvent evt)
    {
      // if (target.panel?.contextualMenuManager != null && target.panel.contextualMenuManager.displayMenuHandledOSX)
      if (target.panel?.contextualMenuManager != null)
      {
        var property = target.panel.contextualMenuManager.GetType().GetProperty("displayMenuHandledOSX", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var displayMenuHandledOSX = (bool)property.GetValue(target.panel.contextualMenuManager);
        if (displayMenuHandledOSX)
        {
          return;
        }
      }

      OnMouseUpDownEvent(evt);
    }

    private void OnKeyUpEvent(KeyUpEvent evt)
    {
      if (evt.keyCode != KeyCode.Menu)
        return;
      DoDisplayMenu(evt);
    }

    private void DoDisplayMenu(EventBase evt)
    {
      if (target.panel?.contextualMenuManager == null)
        return;
      target.panel.contextualMenuManager.DisplayMenu(evt, target);
      evt.StopPropagation();
      evt.PreventDefault();
    }

    private void OnContextualMenuEvent(ContextualMenuPopulateEvent evt)
    {
      Action<ContextualMenuPopulateEvent> menuBuilder = _menuBuilder;
      if (menuBuilder == null)
      {
        return;
      }

      menuBuilder(evt);
    }
  }
}
