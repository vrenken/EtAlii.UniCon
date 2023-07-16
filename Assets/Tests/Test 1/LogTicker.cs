namespace Tests
{
    using UnityEngine;
    using Serilog;
    using System.Collections;
    using Random = UnityEngine.Random;

    public class LogTicker : MonoBehaviour
    {
        private Serilog.ILogger _logger;

        private void Awake()
        {
            _logger = Log.ForContext<LogTicker>();
            Debug.Log($"Starting LogTicker");
            StartCoroutine(WriteLogEntries());
        }

        private IEnumerator WriteLogEntries()
        {
            while (gameObject.activeSelf)
            {
                yield return new WaitForSeconds(1);
                switch (Random.Range(0,6))
                {
                    case 0: _logger.Verbose("New verbose log entry"); break;
                    case 1: _logger.Information("New information log entry"); break;
                    case 2: _logger.Debug("New debug log entry"); break;
                    case 3: _logger.Error("New error log entry"); break;
                    case 4: _logger.Warning("New warning log entry"); break;
                    case 5: _logger.Fatal("New fatal log entry"); break;
                }
            }
        }
    }
}