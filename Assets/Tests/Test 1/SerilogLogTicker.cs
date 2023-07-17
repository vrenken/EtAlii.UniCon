﻿namespace Tests
{
    using System;
    using UnityEngine;
    using Serilog;
    using System.Collections;
    using Random = UnityEngine.Random;

    public class SerilogLogTicker : MonoBehaviour
    {
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
                switch (Random.Range(0,6))
                {
                    case 0: _logger.Verbose("New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something"); break;
                    case 1: _logger.Information("New information log entry"); break;
                    case 2: _logger.Debug("New debug log entry"); break;
                    case 3: _logger.Error("New error log entry"); break;
                    case 4: _logger.Warning("New warning log entry"); break;
                    case 5: _logger.Fatal("New fatal log entry {FatalError}", "Some weird crash"); break;
                }
            }
        }
    }
}