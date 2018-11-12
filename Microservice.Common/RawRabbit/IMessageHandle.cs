using RawRabbit.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Common.RawRabbit
{
    public interface IMessageHandle<TMessage> where TMessage : IMessage
    {
        Task HandleAsync(TMessage e, IMessageContext context);
    }
}
