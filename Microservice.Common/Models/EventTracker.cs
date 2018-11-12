using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Microservice.Common.Models
{
    public class RawRabbitEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EventTracker: BaseObject
    {
        public int Id { get; set; }
        public Guid MessageId { get; set; }
        public string Name { get; set; }
        public string PayLoad { get; set; }
        public EventType Type { get; set; }
    }

    public enum EventType { Publish, Subscribe }
}
