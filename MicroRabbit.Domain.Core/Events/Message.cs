using MediatR;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MicroRabbit.Domain.Core.Events
{
    /// <summary>
    /// Here we have implemented MediatR
    /// </summary>
    public abstract class Message : IRequest<bool>
    {
        public string MessageType { get; protected set; }

        public Message()
        {
            MessageType = GetType().Name;
        }
    }
}
