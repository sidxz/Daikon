
namespace CQRS.Core.Comparators
{
    public static class DictionaryExtensions
    {
        public static bool DictionaryEqual<TKey, TValue>(
            this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (first == second) return true;
            if ((first == null) || (second == null)) return false;
            if (first.Count != second.Count) return false;

            var comparer = EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                if (!second.TryGetValue(kvp.Key, out TValue secondValue)) return false;
                if (!comparer.Equals(kvp.Value, secondValue)) return false;
            }
            return true;
        }
    }
}