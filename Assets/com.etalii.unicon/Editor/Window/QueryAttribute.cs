namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine.UIElements;

    [AttributeUsage(AttributeTargets.Field)]
    public class QueryAttribute : Attribute
    {
        public string Q { get; }

        public QueryAttribute(string q)
        {
            Q = q;
        }
    }


    public static class QueryAttributeExtensions
    {
        private static readonly IDictionary<Type, IList<Tuple<FieldInfo, QueryAttribute>>> TypeCache =
            new Dictionary<Type, IList<Tuple<FieldInfo, QueryAttribute>>>();

        public static void ResolveQueryAttributes<T>(this T view) where T : IView
        {
            var viewType = typeof(T);
            if (!TypeCache.ContainsKey(viewType))
            {
                var result = new List<Tuple<FieldInfo, QueryAttribute>>();

                var fieldInfos = viewType.Assembly.GetTypes()
                    .Where(type => type.IsClass)
                    .Where(type => type.IsAssignableFrom(viewType))
                    .SelectMany(type => type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));

                foreach (var fieldInfo in fieldInfos)
                {
                    var attribute = (QueryAttribute) fieldInfo.GetCustomAttributes(false)
                        .FirstOrDefault(attribute => attribute is QueryAttribute);
                    if (attribute != null)
                    {
                        result.Add(Tuple.Create(fieldInfo, attribute));
                    }
                }

                TypeCache.Add(viewType, result);
            }

            var fields = TypeCache[viewType];
            for (var i = 0; i < fields.Count; i++)
            {
                var (fieldInfo, attribute) = fields[i];

                var qParts = attribute.Q.Split('.');
                var visualElement = view.TemplateRoot;
                for (var iQPart = 0; iQPart < qParts.Length; iQPart++)
                {
                    var qPart = qParts[iQPart];
                    if (iQPart < qParts.Length - 1 || fieldInfo.FieldType == typeof(VisualElement))
                    {
                        visualElement = visualElement.Q(qPart);
                    }
                    else
                    {
                        visualElement = visualElement.Query()
                            .Where(e => e.GetType() == fieldInfo.FieldType && qPart.Equals(e.name))
                            .Build()
                            .First();
                    }
                }

                fieldInfo.SetValue(view, visualElement);
            }
        }
    }
}