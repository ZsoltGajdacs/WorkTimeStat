using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterWork.Enums
{
    internal static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static EnumMatchResult<T> GetEnumForString<T>(string enumDescription) where T : Enum
        {
            IEnumerable<T> enumElements = GetValues<T>();

            EnumMatchResult<T> result = null;
            foreach (T enumElem in enumElements)
            {
                if (enumElem.GetDescription() == enumDescription)
                {
                    result = new EnumMatchResult<T>(enumElem);
                }
            }

            return result;
        }
    }

    internal class EnumMatchResult<T> where T : Enum
    {
        public T FoundEnum { get; private set; }

        public EnumMatchResult(T foundEnum)
        {
            FoundEnum = foundEnum;
        }
    }
}
