﻿using Microservice.Common.RawRabbit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservice.Common.Models.Events
{
    public class TestEvent: IMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ListenNewOrder: IMessage
    {
        public string CollectionName { get; set; }
        public string ServeName { get; set; }
    }
}
