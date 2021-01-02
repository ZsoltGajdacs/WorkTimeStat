using System;
using System.ComponentModel;

namespace WorkTimeStat.Enums
{
    /// <summary>
    /// Taken from: https://stackoverflow.com/a/19621488
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Extensible base method, can be used to get the attributes of an Enum
        /// </summary>
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            Type type = value.GetType();
            System.Reflection.MemberInfo[] memberInfo = type.GetMember(value.ToString());
            object[] attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }

        /// <summary>
        /// Retrieves the Description attribute of the given Enum
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
