using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class NotificationDTO : BaseModelDTO
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
        public virtual UserDTO Sender { get; set; }
        public virtual UserDTO Receiver { get; set; }
    }
}
