using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business_Logic_Layer.Helpers
{
    public static class MathHelper
    {
        public static double CalculateRate(int min, int max, int[] priorities)
        {
            var avg = priorities.Average();
            return avg == 0 ? 0 : 100 - avg / (max - min);
        }
    }
}
