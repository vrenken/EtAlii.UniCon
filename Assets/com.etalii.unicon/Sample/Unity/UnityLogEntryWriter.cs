namespace EtAlii.UniCon.Sample
{
    using System;
    using Bogus.DataSets;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public static class UnityLogEntryWriter
    {
        private static readonly Lorem Lorem = new();

        public static void WriteEntry(Object context)
        {
            switch (Random.Range(0,25))
            {
                // ReSharper disable TemplateIsNotCompileTimeConstantProblem
                case 0: Debug.LogFormat(LogType.Log, LogOption.None, context, "New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something"); break;
                case 1: Debug.LogFormat(LogType.Log, LogOption.None, context, "New information log entry"); break;
                case 2: Debug.LogFormat(LogType.Log, LogOption.None, context, "New debug log entry"); break;
                case 3: Debug.LogFormat(LogType.Error, LogOption.None, context, "New error log entry"); break;
                case 4: Debug.LogFormat(LogType.Warning, LogOption.None, context, "New warning log entry"); break;
                case 5: Debug.LogFormat(LogType.Error, LogOption.None, context, "New fatal log entry {FatalError}", "Some weird crash"); break;
                case 6: Debug.LogFormat(LogType.Log, LogOption.None, context, "Another log entry with some new {Information} from a specific {Time}", "Well known information", DateTimeOffset.Now); break;
                case 7: Debug.LogFormat(LogType.Log, LogOption.None, context, "Starting some more debugging"); break;
                case 8: Debug.LogFormat(LogType.Log, LogOption.None, context, Lorem.Sentence()); break;
                case 9: Debug.LogFormat(LogType.Log, LogOption.None, context, Lorem.Sentence()); break;
                case 10: Debug.LogFormat(LogType.Log, LogOption.None, context, Lorem.Sentence()); break;
                case 11: Debug.LogFormat(LogType.Assert, LogOption.None, context, Lorem.Sentence()); break;
                case 12: Debug.LogFormat(LogType.Log, LogOption.None, context, Lorem.Sentence()); break;
                case 13: Debug.LogFormat(LogType.Warning, LogOption.None, context, Lorem.Sentence()); break;
                case 14: Debug.LogFormat(LogType.Log, LogOption.None, context, Lorem.Sentence()); break;
                case 15: Debug.LogFormat(LogType.Log, LogOption.None, context, Lorem.Sentence()); break;
                case 16:
                    var (sentence, word) = BuildSentenceWithProperty();
                    Debug.LogFormat(LogType.Log, LogOption.None, context, sentence, word); 
                    break;
                case 17:
                    (sentence, word) = BuildSentenceWithProperty();
                    Debug.LogFormat(LogType.Log, LogOption.None, context, sentence, word); 
                    break;
                case 18:
                    (sentence, word) = BuildSentenceWithProperty();
                    Debug.LogFormat(LogType.Log, LogOption.None, context, sentence, word); 
                    break;
                case 19:
                    (sentence, word) = BuildSentenceWithProperty();
                    Debug.LogFormat(LogType.Warning, LogOption.None, context, sentence, word); 
                    break;
                case 20:
                    var exception = new TestException(Lorem.Sentence(), Environment.StackTrace);
                    Debug.LogException(exception, context); 
                    break;
                case 21: Debug.LogFormat(LogType.Exception, LogOption.None, context, "New fatal log entry {FatalError}", "Some other crash"); break;
                case 22: Debug.LogFormat(LogType.Exception, LogOption.None, context, "New fatal log entry {FatalError}", "Some really weird crash"); break;
                case 23: Debug.LogFormat(LogType.Log, LogOption.None, context, "New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something else"); break;
                case 24: Debug.LogFormat(LogType.Log, LogOption.None, context, "New verbose log entry {Property1}, and {Property2}", Environment.TickCount, "something completely different"); break;
                // ReSharper restore TemplateIsNotCompileTimeConstantProblem
            }
        }

        private static (string sentence, string propertyValue) BuildSentenceWithProperty()
        {
            var sentence = Lorem.Sentence();
            var words = sentence.Split(" ");
            var indexToReplace=  Random.Range(0, words.Length);
            words[indexToReplace] = $"{{{words[indexToReplace].TrimEnd('.')}}}";
            var propertyValue = Lorem.Word();
            sentence = string.Join(" ", words);
            return (sentence, propertyValue);
        }
    }
}