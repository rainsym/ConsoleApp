using Google.Cloud.Firestore;
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

    public interface IListenHandler<TMessage> where TMessage : IMessage
    {
        Task HandleAsync(TMessage eve, QuerySnapshot e);
    }
}
