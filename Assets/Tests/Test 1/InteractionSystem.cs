namespace Tests
{
    using UnityEngine;

    public class InteractionSystem : MonoBehaviour
    {
        public void DoLogEntry()
        {
            UnityLogEntryWriter.WriteEntry(this);
            //_holographicMenu.DoLogEntry();
        }
    }
}