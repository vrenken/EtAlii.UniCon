﻿namespace Tests
{
    using UnityEngine;

    public class LeftHandController : MonoBehaviour
    {
        public InteractionSystem interactionSystem;
        
        public void DoLogEntry()
        {
            UnityLogEntryWriter.WriteEntry(this);
            interactionSystem.DoLogEntry();
        }
    }
}