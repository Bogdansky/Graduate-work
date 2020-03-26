using Business_Logic_Layer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Graduate_Work.Helpers
{
    public class OperationResult
    {
        public object Entity { get; set; }
        public Error Error { get; set; }
    }
}
