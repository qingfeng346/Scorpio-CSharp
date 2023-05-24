using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    internal class ScorpioStringUtil {
        private static Dictionary<string, uint> stringToIndex = new Dictionary<string, uint>();
        private static string[] indexToString = new string[1024];
        private static uint stringIndex = 0;
        public static uint StringToIndex(string key) {
            if (stringToIndex.TryGetValue(key, out var index))
                return index;
            if (index >= indexToString.Length) {
                var newArray = new string[indexToString.Length + 1024];
                Array.Copy(indexToString, newArray, indexToString.Length);
                indexToString = newArray;
            }
            indexToString[stringIndex] = key;
            return stringToIndex[key] = stringIndex++;
        }
        public static string IndexToString(uint index) {
            if (index < indexToString.Length)
                return indexToString[index];
            return null;
        }
    }
}
