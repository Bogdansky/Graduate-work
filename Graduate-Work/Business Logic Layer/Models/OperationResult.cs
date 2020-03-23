using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Models
{
    public class OperationResult
    {
        public object Result { get; set; }
        public Error Error { get; set; }
    }

    public class Error
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
