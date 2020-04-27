using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Helpers
{
    public static class StringHelper
    {
        public static int ToInt(this string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return default;
            }
        }
    }
}
