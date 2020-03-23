using Business_Logic_Layer.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Helpers
{
    public class EntityHelper
    {
        public static BaseModelDTO SetId(BaseModelDTO entity, int id)
        {
            entity.Id = id;
            return entity;
        }
    }
}
