namespace Tests
{
    using System;
    using UnityEngine;
    using System.Collections;
    using Bogus.DataSets;
    using Random = UnityEngine.Random;

    public class UnityLogTicker : MonoBehaviour
    {
        private readonly Lorem _lorem = new();

        public float interval = 1;
        private void OnEnable()
        {
            Debug.Log("Starting UnityLogTicker", this);
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
                switch (Random.Range(0,13))
                {
                    case 0: Debug.Log("New information log entry", this); break;
                    case 1: Debug.LogException(new InvalidOperationException("Some invalid operation"), this); break;
                    case 2: Debug.LogError("New error log entry", this); break;
                    case 3: Debug.LogWarning("New warning log entry", this); break;
                    case 4: Debug.LogAssertion("New assertion log entry", this); break;
                    case 5: Debug.Log(_lorem.Sentence(), this); break;
                    case 6: Debug.Log(_lorem.Sentence(), this); break;
                    case 7: Debug.Log(_lorem.Sentence(), this); break;
                    case 8: Debug.Log(_lorem.Sentence(), this); break;
                    case 9: Debug.LogWarning(_lorem.Sentence(), this); break;
                    case 10: Debug.Log(_lorem.Sentence(), this); break;
                    case 11: Debug.LogWarning(_lorem.Sentence()); break;
                    case 12: Debug.Log(_lorem.Sentence(), this); break;
                    // ReSharper restore TemplateIsNotCompileTimeConstantProblem

                }
            }
        }
    }
}