using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Business_Logic_Layer.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Преобразует время формата hh:mm:ss в TimeSpan
        /// </summary>
        /// <param name="hhmmss">Строка в формате hh:mm:ss</param>
        /// <returns>Время = hh+mm+ss</returns>
        public static TimeSpan ParseTime(string time)
        {
            if (time == null)
            {
                return default;
            }
            var values = time.Split(":");
            var valid = values.All(v => int.TryParse(v, NumberStyles.Integer,CultureInfo.InvariantCulture, out int _));
            if (values.Length != 3 || !valid)
            {
                return default;
            }
            var result = TimeSpan.FromHours(int.Parse(values[0])) + TimeSpan.FromMinutes(int.Parse(values[1])) + TimeSpan.FromSeconds(int.Parse(values[2]));
            return result;
        }
    }
}
