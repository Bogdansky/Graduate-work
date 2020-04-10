using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Business_Logic_Layer.Enums
{
    public enum TaskStatusEnum
    {
        [Description("Создан")]
        New=1,
        [Description("Активный")]
        Active,
        [Description("Готов к тестированию")]
        ReadyForQA,
        [Description("Закрыт")]
        Closed
    }
}
