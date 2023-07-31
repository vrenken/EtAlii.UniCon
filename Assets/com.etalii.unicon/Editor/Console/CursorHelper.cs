namespace EtAlii.UniCon.Editor
{
    using System.Reflection;
    using UnityEditor;
    using UnityEngine.UIElements;

    internal class CursorHelper
    {
        /// <summary>
        /// A helper method to quickly assign a cursor to a visual element when it is hovered over by the mouse.  
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public static StyleCursor GetCursor(MouseCursor cursor)
        {
            object cursorInstance = new Cursor();
            var fields = typeof(Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance)!;
            fields.SetValue(cursorInstance, (int)cursor);
            return new StyleCursor((Cursor)cursorInstance);
        }
    }
}