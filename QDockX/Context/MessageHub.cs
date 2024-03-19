using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Context
{
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);

    public static class MessageHub
    {
        public static event MessageEventHandler Message = null;
        public static void Send(string message) => Send(null, message, null);
        public static void Send(string message, object param) => Send(null, message, param);
        public static void Send(object sender, string message, object param) => (_ = Message)?.Invoke(sender, new(message, param));
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; }
        public object Parameter { get; }
        public MessageEventArgs(string message, object parameter) => (Message, Parameter) = (message, parameter);        
    }
}
