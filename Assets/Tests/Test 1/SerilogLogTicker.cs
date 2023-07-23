namespace Tests
{
    using System;
    using UnityEngine;
    using Serilog;
    using System.Collections;
    using Bogus.DataSets;
    using ILogger = Serilog.ILogger;
    using Random = UnityEngine.Random;

    public class SerilogLogTicker : MonoBehaviour
    {
        private readonly Lorem _lorem = new();
        
        private ILogger _logger;

        public float interval = 1;
        private void OnEnable()
        {
            _logger = Log.ForContext<SerilogLogTicker>().ForContext("UniConSource", "Serilog");
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
                var logger = PrepareLogger();
                yield return new WaitForSeconds(interval);
                switch (Random.Range(0,28))
                {
                    // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                    case 0: logger.Verbose("New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something"); break;
                    case 1: logger.Information("New information log entry"); break;
                    case 2: logger.Debug("New debug log entry"); break;
                    case 3: logger.Error("New error log entry"); break;
                    case 4: logger.Warning("New warning log entry"); break;
                    case 5: logger.Fatal("New fatal log entry {FatalError}", "Some weird crash"); break;
                    case 6: logger.Information("Another log entry with some new {Information} from a specific {Time}", "Well known information", DateTimeOffset.Now); break;
                    case 7: logger.Debug("Starting some more debugging"); break;
                    case 8: logger.Verbose(_lorem.Sentence()); break;
                    case 9: logger.Verbose(_lorem.Sentence()); break;
                    case 10: logger.Verbose(_lorem.Sentence()); break;
                    case 11: logger.Debug(_lorem.Sentence()); break;
                    case 12: logger.Information(_lorem.Sentence()); break;
                    case 13: logger.Warning(_lorem.Sentence()); break;
                    case 14: logger.Information(_lorem.Sentence()); break;
                    case 15: logger.Debug(_lorem.Sentence()); break;
                    case 16:
                        var (sentence, word) = BuildSentenceWithProperty();
                        logger.Verbose(sentence, word); 
                        break;
                    case 17:
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Information(sentence, word); 
                        break;
                    case 18:
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Debug(sentence, word); 
                        break;
                    case 19:
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Warning(sentence, word); 
                        break;
                    case 20:
                        var exception = new TestException(_lorem.Sentence(), Environment.StackTrace);
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Verbose(exception, sentence, word); 
                        break;
                    case 21:
                        exception = new TestException(_lorem.Sentence(), Environment.StackTrace);
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Information(exception, sentence, word); 
                        break;
                    case 22:
                        exception = new TestException(_lorem.Sentence(), Environment.StackTrace);
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Debug(exception, sentence, word); 
                        break;
                    case 23:
                        exception = new TestException(_lorem.Sentence(), Environment.StackTrace);
                        (sentence, word) = BuildSentenceWithProperty();
                        logger.Warning(exception, sentence, word); 
                        break;
                    case 24: logger.Fatal("New fatal log entry {FatalError}", "Some other crash"); break;
                    case 25: logger.Fatal("New fatal log entry {FatalError}", "Some really weird crash"); break;
                    case 26: logger.Verbose("New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something else"); break;
                    case 27: logger.Verbose("New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something completely different"); break;

                    // ReSharper restore TemplateIsNotCompileTimeConstantProblem
                }
            }
        }

        private (string sentence, string propertyValue) BuildSentenceWithProperty()
        {
            var sentence = _lorem.Sentence();
            var words = sentence.Split(" ");
            var indexToReplace=  Random.Range(0, words.Length);
            words[indexToReplace] = $"{{{words[indexToReplace].TrimEnd('.')}}}";
            var propertyValue = _lorem.Word();
            sentence = string.Join(" ", words);
            return (sentence, propertyValue);
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
        
        private class TestException : Exception
        {
            public override string StackTrace { get; }

            public TestException(string message, string stackTrace) 
                : base(message)
            {
                StackTrace = stackTrace;
            }
        }
    }
}