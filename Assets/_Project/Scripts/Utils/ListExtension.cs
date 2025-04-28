using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Scripts.Utils {
    public static class ListExtension {
        public static void ForEach<T>(this IReadOnlyList<T> list, Action<T, int> action) {
            for (int i = 0; i < list.Count; i++) {
                action(list[i], i);
            }
        }

        public static bool ForEachAny<T>(this IReadOnlyList<T> list, Func<T, bool> action) {
            if (list == null) {
                return false;
            }
            bool isAny = false;

            for (int i = 0; i < list.Count; i++) {
                isAny |= action(list[i]);
            }
            return isAny;
        }

        [ContractAnnotation("list:null => false")]
        public static bool IsFilled<T>(this List<T> list) {
            return list != null && list.Count > 0;
        }

        [ContractAnnotation("list:null => true")]
        public static bool IsEmpty<T>(this List<T> list) {
            return list == null || list.Count == 0;
        }

        [ContractAnnotation("list:null => false")]
        public static bool IsFilled<T>(this IReadOnlyList<T> list) {
            return list != null && list.Count > 0;
        }

        [ContractAnnotation("list:null => true")]
        public static bool IsEmpty<T>(this IReadOnlyList<T> list) {
            return list == null || list.Count == 0;
        }


        public static void AddIfNewAndNotNull<T>(this IList<T> list, T value) {
            if (!typeof(T).IsValueType && Equals(value, null)) {
                return;
            }

            if (list == null) {
                return;
            }

            if (!list.Contains(value)) {
                list.Add(value);
            }
        }

        public static void AddRangeNewNotNull<T>(this IList<T> list, IEnumerable<T> collection) {
            if (list == null) {
                return;
            }

            foreach (T element in collection) {
                list.AddIfNewAndNotNull(element);
            }
        }

        public static void AddRangeSafe<T>(this IList<T> list, IEnumerable<T> collection) {
            if (collection == null) {
                return;
            }

            foreach (T element in collection) {
                list.Add(element);
            }
        }

        public static List<T> CloneList<T>(this IList<T> list) where T : ICloneable {
            if (list == null) {
                return null;
            }

            return list.Select(item => item == null ? default : (T)item.Clone()).ToList();
        }

        public static bool RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate) {
            var startCount = list.Count;
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i])) {
                    list.RemoveAt(i);
                    i--;
                }
            }
            return list.Count != startCount;
        }

        public static IEnumerable<T> GetAndRemoveWhere<T>(this IList<T> list, Func<T, bool> predicate) {
            for (int i = 0; i < list.Count; i++) {
                var item = list[i];
                if (predicate(item)) {
                    list.RemoveAt(i);
                    i--;
                    yield return item;
                }
            }
        }

        public static T GetFirstAndRemove<T>(this IList<T> list) => list.GetAndRemove(0);

        public static T GetAndRemove<T>(this IList<T> list, int index) {
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public static T GetFirstOrAdd<T>(this IList<T> list, Func<T, bool> predicate, Func<T> constructor) {
            var item = list.FirstOrDefault(predicate);
            if (item == null) {
                item = constructor();
                list.Add(item);
            }
            return item;
        }

        public static void MoveToEnd<T>(this IList<T> list, int index) {
            T value = list.GetAndRemove(index);
            list.Add(value);
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, Predicate<T> condition) {
            for (int i = 0; i < list.Count; i++) {
                if (condition(list[i])) {
                    return i;
                }
            }
            return -1;
        }

        public static bool IndexOutOfBounds<T>(this IReadOnlyList<T> list, int index) {
            return list == null || index < 0 || index > list.Count - 1;
        }

        public static bool IsIndexInBounds<T>(this IReadOnlyList<T> list, int index) {
            return !list.IndexOutOfBounds(index);
        }

        public static bool UnorderedEqualTo<T>(this IReadOnlyList<T> seq1, IReadOnlyList<T> seq2) {
            if (seq1 == null && seq2 == null) {
                return true;
            }
            if (seq1 == null || seq2 == null) {
                return false;
            }

            return seq1.Count == seq2.Count && seq1.All(seq2.Contains);
        }

        public static T GetAtOrDefault<T>(this IReadOnlyList<T> list, int index) {
            if (list == null || index >= list.Count || index < 0) {
                return default;
            }
            return list[index];
        }

        public static IEnumerable<T> GetSafeRange<T>(this IReadOnlyList<T> list, int index, int count) {
            if (list.IndexOutOfBounds(index)) {
                yield break;
            }

            var indexMax = Math.Min(list.Count, index + count);
            for (int i = index; i < indexMax; i++) {
                yield return list[i];
            }
        }

        /// <summary>
        /// Производит бинарный поиск по отсортированной коллекции и возвращает ближайший элемент снизу (нижняя граница интервала)
        /// </summary>
        public static TListElement BinaryIntervalSearch<TListElement, TValue>(
            this IReadOnlyList<TListElement> list,
            Func<TListElement, TValue> getter,
            TValue searchValue
        ) where TValue : IComparable {
            int from = 0;
            int to = list.Count - 1;

            while (to >= from) {
                int mid = (from + to) / 2;
                var currentValue = getter(list[mid]);
                if (currentValue.Equals(searchValue)) {
                    return list[mid];
                }

                if (currentValue.CompareTo(searchValue) > 0) {
                    to = mid - 1;
                } else {
                    from = mid + 1;
                }
            }

            return list[Math.Max(0, to)];
        }

        public static int GetHashCode<T>(this IReadOnlyList<T> list) {
            if (list == null) {
                return 0;
            }

            unchecked {
                var hash = 17;
                foreach (var element in list) {
                    hash = hash * 31 + element.GetHashCode();
                }
                return hash;
            }
        }

        public static string ListToString<T>(this IEnumerable<T> list, string elementsSeparator) {
            if (list == null) {
                return "(null)";
            }

            var sb = new StringBuilder("{");
            
            foreach (var element in list) {
                sb.Append(element + elementsSeparator);
            }
            
            sb.Append("}");
            return sb.ToString();
        }
        
        public static List<object> WithoutNulls(this IEnumerable<object> collection) {
            List<object> result = new();

            foreach (var element in collection) {
                if (element == null) {
                    continue;
                }

                result.Add(element);
            }
            
            return result;
        }
    }
}