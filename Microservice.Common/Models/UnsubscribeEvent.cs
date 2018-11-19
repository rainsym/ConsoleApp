using Microservice.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Common.Models
{
    [SQLViewName("RawTransactionHistory")]
    public class UnsubscribeEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Register { get; set; }
        public string Real { get; set; }
        public Guid MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
