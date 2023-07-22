namespace EtAlii.UniCon.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class EditorInputDialog : EditorWindow
    {
        private string _description, _inputText;
        private string _okButton, _cancelButton;
        private bool _initializedPosition;
        private Action _onOkButton;
        private Func<string, bool> _textValidation;

        private bool _shouldClose;

        private EditorWindow _parentWindow;

        private bool _initialized;

        private void OnGUI()
        {
            if (!_initialized)
            {
                var target = _parentWindow != null 
                    ? _parentWindow.position.center 
                    : EditorGUIUtility.GetMainWindowPosition().center;
                position = new Rect(target - position.size / 2, position.size);
                _initialized = true;
            }

            // Check if Esc/Return have been pressed
            var e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    // Escape pressed
                    case KeyCode.Escape:
                        _shouldClose = true;
                        break;

                    // Enter pressed
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        _onOkButton?.Invoke();
                        _shouldClose = true;
                        break;
                }
            }

            if (_shouldClose)
            {
                // Close this dialog
                Close();
                //return;
            }

            // Draw our control
            var rect = EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField(_description);

            EditorGUILayout.Space(8);
            GUI.SetNextControlName("inText");
            _inputText = EditorGUILayout.TextField("", _inputText);
            GUI.FocusControl("inText"); // Focus text field
            EditorGUILayout.Space(12);

            // Draw OK / Cancel buttons
            var r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            GUI.enabled = _textValidation(_inputText);
            if (GUI.Button(r, _okButton))
            {
                _onOkButton?.Invoke();
                _shouldClose = true;
            }

            GUI.enabled = true;

            r.x += r.width;
            if (GUI.Button(r, _cancelButton))
            {
                _inputText = null; // Cancel - delete inputText
                _shouldClose = true;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();

            // Force change size of the window
            if (rect.width != 0 && minSize != rect.size)
            {
                minSize = maxSize = rect.size;
            }
        }

        /// <summary>
        /// Returns text player entered, or null if player cancelled the dialog.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="inputText"></param>
        /// <param name="parentWindow"></param>
        /// <param name="okButton"></param>
        /// <param name="cancelButton"></param>
        /// <param name="textValidation"></param>
        /// <returns></returns>
        public static string Show(
            string title, 
            string description, 
            string inputText, 
            EditorWindow parentWindow,
            string okButton = "OK",
            string cancelButton = "Cancel",
            Func<string, bool> textValidation = null)
        {
            string result = null;
            var window = CreateInstance<EditorInputDialog>();

            window._parentWindow = parentWindow;
            window.titleContent = new GUIContent(title);
            window._description = description;
            window._inputText = inputText;
            window._okButton = okButton;
            window._cancelButton = cancelButton;
            window._onOkButton += () => result = window._inputText;
            window._textValidation = textValidation ?? (_ => true);
            window.ShowModal();

            return result;
        }
    }
}