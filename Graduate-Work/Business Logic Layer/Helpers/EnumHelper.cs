using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Business_Logic_Layer.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum @enum)
        {
            FieldInfo fi = @enum.GetType().GetField(@enum.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return @enum.ToString();
        }
        /// <summary>
        /// Cast int value to description of TEnum value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Int value of enum (more than 255 - dangerous)</param>
        /// <returns></returns>
        public static string GetDescriptionByValue<T>(this int value)
        {
            var type = typeof(T);
            if (!type.IsEnum || !type.IsEnumDefined(value))
            {
                return "";
            }
            var enumValue = (T)Enum.ToObject(type, value) as Enum;
            return enumValue.GetDescription();   
        }

        public static T GetMemberByValue<T>(this int value)
        {
            var type = typeof(T);
            if (!type.IsEnum || !type.IsEnumDefined(value))
            {
                return default;
            }
            return (T)Enum.ToObject(type, value);
        }

        public static List<KeyValuePair<int, string>> GetEnumValues<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                return null;
            }
            var list = Enum.GetValues(type).Cast<Enum>();
            var values = list.Select(e => new KeyValuePair<int, string>((int)Enum.Parse(type, e.ToString()), e.GetDescription())).ToList();
            return values;
        }
    }
}
