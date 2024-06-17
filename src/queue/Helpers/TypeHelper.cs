using System.Collections.Concurrent;

namespace Sontiq.Queue.Helpers
{
    internal class TypeHelper
    {
        protected readonly ConcurrentDictionary<Type, List<Type>> typesCache = new();

        public virtual IEnumerable<Type> GetQueueMessageTypesFromHierarchy(Type messageType)
        {
            return typesCache.GetOrAdd(messageType, t => GetQueueMessageTypesFromHierarchyInternal(t).Distinct().ToList());
        }

        protected virtual IEnumerable<Type> GetQueueMessageTypesFromHierarchyInternal(Type messageType)
        {
            var mustHave = typeof(IQueueMessage);

            var candidates = new Stack<Type>(new[] { messageType });

            while (candidates.TryPop(out Type candidate))
            {
                foreach (var iface in candidate.GetInterfaces())
                {
                    candidates.Push(iface);
                }

                if (candidate.BaseType != null)
                {
                    candidates.Push(candidate.BaseType);
                }

                if (mustHave.IsAssignableFrom(candidate))
                {
                    yield return candidate;
                }
            }
        }

        public virtual string EncodeType(Type t)
        {
            return Encode(t);
        }

        public static string Encode(Type t)
        {
            return string.Join(", ", t.AssemblyQualifiedName.Split(',').Take(2).Select(x => x.Trim()));
        }

        public virtual Type DecodeType(string t)
        {
            return Decode(t);
        }

        public static Type Decode(string t)
        {
            return Type.GetType(t);
        }
    }
}
