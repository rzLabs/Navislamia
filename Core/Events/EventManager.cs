using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Events
{
    public class EventManager
    {
        public static EventManager Instance;

        public EventManager() => Instance = this;

        public event EventHandler<MessageArgs> MessageOccured;
        public event EventHandler<ExceptionArgs> ExceptionOccured;

        public void OnMessage(MessageArgs m) => MessageOccured?.Invoke(this, m);

        public void OnException(ExceptionArgs e) => ExceptionOccured?.Invoke(this, e);
    }
}
