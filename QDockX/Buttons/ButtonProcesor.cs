using QDockX.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Buttons
{
    public static class ButtonProcesor
    {
        private static int keys = 0;
        public static void Init()
        {
            MessageHub.Message += (object sender, MessageEventArgs e) => 
            {                
                switch(e.Message)
                {
                    case "Pressed":
                        switch(e.Parameter)
                        {
                            case var n when "Settings".Equals(n):
                                Data.Instance.Page.Value = "Settings";
                                break;
                            case var n when "Main".Equals(n):
                                Data.Instance.Page.Value = "Main";
                                break;
                            case var n when n is string s && int.TryParse(s, out int i):
                                keys |= (1 << i);
                                MessageHub.Send("KeyPress", i);
                                break;
                        }
                        break;
                    case "Released":
                        switch(e.Parameter)
                        {
                            case var n when n is string s && int.TryParse(s, out int i):
                                keys &= ~(1 << i);
                                if (keys == 0) MessageHub.Send("KeyPress", 19);
                                break;
                        }
                        break;
                }
            };
        }
    }
}
