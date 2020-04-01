using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services
{
    public class NotificationService: BaseService
    {
        public NotificationService(ILogger logger, IMapper mapper, ContextFactory contextFactory) : base(logger, mapper, contextFactory)
        {

        }
    }
}
