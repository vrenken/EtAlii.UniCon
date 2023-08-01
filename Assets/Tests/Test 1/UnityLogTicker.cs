namespace Tests
{
    using System;
    using UnityEngine;
    using System.Collections;
    using Random = UnityEngine.Random;

    public class UnityLogTicker : MonoBehaviour
    {
        public LeftHandController leftHandController;
        
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
                //var logger = PrepareLogger();

                switch (Random.Range(0, 10))
                {
                    case 0:
                        // We want to be able to track method calls throughout the whole application stack.
                        // Including across network and process boundaries.
                        // For this we create a unique correlationId and pass it through all involved systems.
                        using (Debug.unityLogger.BeginCorrelationScope("CorrelationId", Environment.TickCount.ToString(), false))
                        {
                            UnityLogEntryWriter.WriteEntry(this);
                            leftHandController.DoLogEntry();
                        }
                        break;
                    case 1:
                        // We want to be able to track method calls throughout the whole application stack.
                        // Including across network and process boundaries.
                        // For this we create a unique correlationId and pass it through all involved systems.
                        using (Debug.unityLogger.BeginCorrelationScope("CorrelationId", false))
                        {
                            UnityLogEntryWriter.WriteEntry(this);
                            leftHandController.DoLogEntry();
                        }
                        break;
                    default:
                        UnityLogEntryWriter.WriteEntry(this);
                        break;
                }
            }
        }

        // private ILogger PrepareLogger()
        // {
        //     return Random.Range(0, 6) switch
        //     {
        //         0 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Database"),
        //         1 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Avatar"),
        //         2 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Audio.EnvironmentSoundManager"),
        //         3 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Audio.VoiceExchange"),
        //         4 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Networking.Server1GrpcClient"),
        //         5 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Networking.Server2RestClient"),
        //         _ => throw new ArgumentOutOfRangeException()
        //     };
        // }
    }
}