using QDockX.Context;
using QDockX.Language;
using QDockX.Util;
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
                            case var n when "EditLang".Equals(n):
                                Data.Instance.LanguageDesignator.Value = Data.Instance.Language.Value;
                                Data.Instance.LanguageData.Value = Lang.GetLanguageData();
                                Data.Instance.Page.Value = "Language";
                                break;
                            case var n when "EditLangReturn".Equals(n):
                                Data.Instance.Page.Value = "Language";
                                break;
                            case var n when "ApplyLang".Equals(n):
                                Lang.SaveLanguage(Data.Instance.LanguageDesignator.Value, Data.Instance.LanguageData.Value);
                                Lang.FindAvailable();
                                break;
                            case var n when "DeleteLangAsk".Equals(n):
                                Data.Instance.NoAction.Value = "EditLangReturn";
                                Data.Instance.YesAction.Value = "DeleteLang";
                                Data.Instance.YesNoQuestion.Value = $"{Lang.ConfirmDelLang} {Data.Instance.LanguageDesignator.Value}";
                                Data.Instance.Page.Value = "YesNo";
                                break;
                            case var n when "DeleteLang".Equals(n):
                                string ltd = Data.Instance.LanguageDesignator.Value;
                                Data.Instance.Language.Value = "en";
                                Lang.DeleteLanguge(ltd);
                                Data.Instance.Page.Value = "Settings";
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
