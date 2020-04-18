using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Business_Logic_Layer.Enums
{
    public enum MonthEnum
    {
        [Description("Январь")]
        January = 1, 
        [Description("Февраль")]
        February,    
        [Description("Март")]
        March,       
        [Description("Апрель")]
        April,       
        [Description("Май")]
        May,         
        [Description("Июнь")]
        June,        
        [Description("Июль")]
        July,        
        [Description("Август")]
        August,      
        [Description("Сентябрь")]
        September,   
        [Description("Октябрь")]
        October,     
        [Description("Ноябрь")]
        November,
        [Description("Декабрь")]
        December
    }
}
