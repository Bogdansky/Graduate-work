using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Business_Logic_Layer.Enums
{
    public enum TaskTypeEnum
    {
        [Description("Не указано")]
        None,
        [Description("Таск")]
        Task,
        [Description("Баг")]
        Bug
    }
}
