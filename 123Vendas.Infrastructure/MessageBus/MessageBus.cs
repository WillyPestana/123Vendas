using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _123Vendas.Infrastructure.MessageBus
{
    public class MessageBus : IMessageBus
    {
        public void Publish(string eventName, string message)
        {
            using (LogContext.PushProperty("MessageBus", true))
            {
                Log.Information("Evento Publicado: {EventName} - Mensagem: {Message}", eventName, message);
            }
        }
    }
}
