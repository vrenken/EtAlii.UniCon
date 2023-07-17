namespace Tests
{
    using System;
    using UnityEngine;
    using System.Collections;
    using Random = UnityEngine.Random;

    public class UnityLogTicker : MonoBehaviour
    {
        public float interval = 1;
        private void OnEnable()
        {
            Debug.Log("Starting UnityLogTicker");
            StartCoroutine(WriteLogEntries());
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(WriteLogEntries));
        }

        private IEnumerator WriteLogEntries()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(interval);
                switch (Random.Range(0,5))
                {
                    case 0: Debug.Log("New information log entry", this); break;
                    case 1: Debug.LogException(new InvalidOperationException("Some invalid operation"), this); break;
                    case 2: Debug.LogError("New error log entry", this); break;
                    case 3: Debug.LogWarning("New warning log entry", this); break;
                    case 4: Debug.LogAssertion("New assertion log entry", this); break;
                }
            }
        }
    }
}