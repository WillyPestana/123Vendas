using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Infrastructure.MessageBus
{
    public interface IMessageBus
    {
        void Publish(string eventName, string message);
    }
}
