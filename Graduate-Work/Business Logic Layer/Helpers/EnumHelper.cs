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
    }
}
