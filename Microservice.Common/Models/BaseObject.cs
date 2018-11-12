using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Common.Models
{
    public class BaseObject
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
