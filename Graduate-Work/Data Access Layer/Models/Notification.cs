using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Notification : BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        // ссылка для кнопки "Принять", к примеру
        public string AcceptLink { get; set; }
        // ссылка для кнопки "Отмена", к примеру
        public string CancelLink { get; set; }
        // дата отправления
        public DateTime SendTime { get; set; }
        // время жизни
        public TimeSpan LifeTime { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}
