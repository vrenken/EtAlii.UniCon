namespace Tests
{
    using System;
    using UnityEngine;
    using Serilog;
    using System.Collections;
    using Bogus.DataSets;
    using Random = UnityEngine.Random;

    public class SerilogLogTicker : MonoBehaviour
    {
        private readonly Lorem _lorem = new();
        
        private Serilog.ILogger _logger;

        public float interval = 1;
        private void OnEnable()
        {
            _logger = Log.ForContext<SerilogLogTicker>().ForContext("UniConSource", "Serilog");
            Debug.Log($"Starting SerilogLogTicker");
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
                switch (Random.Range(0,16))
                {
                    // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                    case 0: _logger.Verbose("New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something"); break;
                    case 1: _logger.Information("New information log entry"); break;
                    case 2: _logger.Debug("New debug log entry"); break;
                    case 3: _logger.Error("New error log entry"); break;
                    case 4: _logger.Warning("New warning log entry"); break;
                    case 5: _logger.Fatal("New fatal log entry {FatalError}", "Some weird crash"); break;
                    case 6: _logger.Information("Another log entry with some new {Information} from a specific {Time}", "Well known information", DateTimeOffset.Now); break;
                    case 7: _logger.Debug("Starting some more debugging"); break;
                    case 8: _logger.Verbose(_lorem.Sentence()); break;
                    case 9: _logger.Verbose(_lorem.Sentence()); break;
                    case 10: _logger.Verbose(_lorem.Sentence()); break;
                    case 11: _logger.Debug(_lorem.Sentence()); break;
                    case 12: _logger.Information(_lorem.Sentence()); break;
                    case 13: _logger.Warning(_lorem.Sentence()); break;
                    case 14: _logger.Information(_lorem.Sentence()); break;
                    case 15: _logger.Debug(_lorem.Sentence()); break;
                    // ReSharper restore TemplateIsNotCompileTimeConstantProblem
                }
            }
        }
    }
}