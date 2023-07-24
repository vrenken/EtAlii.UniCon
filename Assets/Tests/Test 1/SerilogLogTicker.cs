namespace Tests
{
    using System;
    using UnityEngine;
    using Serilog;
    using System.Collections;
    using ILogger = Serilog.ILogger;
    using Random = UnityEngine.Random;

    public class SerilogLogTicker : MonoBehaviour
    {
        private AvatarSystem _avatarSystem;
        
        private ILogger _logger;

        public float interval = 1;

        private void Awake()
        {
            // In MonoBehaviours the Logger needs to be configured in the Awake method.
            _logger = Log
                .ForContext<SerilogLogTicker>()
                .ForContext("UniConSource", "Serilog");
        }

        private void OnEnable()
        {
            _avatarSystem = new(); // TODO: It should be possible to make this a readonly field but in the current case it'll result in a SilentLogger.
            Debug.Log($"Starting SerilogLogTicker", this);
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
                var logger = PrepareLogger();

                switch (Random.Range(0, 10))
                {
                    case 0:
                        // We want to be able to track method calls throughout the whole application stack.
                        // Including across network and process boundaries.
                        // For this we create a unique correlationId and pass it through all involved systems.
                        using (_logger.BeginCorrelationScope("CorrelationId", Environment.TickCount.ToString(), false))
                        {
                            LogEntryWriter.WriteEntry(logger);
                            _avatarSystem.DoLogEntry();
                        }
                        break;
                    case 1:
                        // We want to be able to track method calls throughout the whole application stack.
                        // Including across network and process boundaries.
                        // For this we create a unique correlationId and pass it through all involved systems.
                        using (_logger.BeginCorrelationScope("ShortGuidCorrelationId", false))
                        {
                            LogEntryWriter.WriteEntry(logger);
                            _avatarSystem.DoLogEntry();
                        }
                        break;
                    default:
                        LogEntryWriter.WriteEntry(logger);
                        break;
                }
            }
        }

        private ILogger PrepareLogger()
        {
            return Random.Range(0, 6) switch
            {
                0 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Database"),
                1 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Avatar"),
                2 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Audio.EnvironmentSoundManager"),
                3 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Audio.VoiceExchange"),
                4 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Networking.Server1GrpcClient"),
                5 => _logger.ForContext("SourceContext", "EtAlii.TestApp.Networking.Server2RestClient"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
    }
}