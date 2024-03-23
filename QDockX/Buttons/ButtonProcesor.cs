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
        private static ViewModel<Color> editedColor = null;

        public static void Init()
        {
            MessageHub.Message += (object sender, MessageEventArgs e) => 
            {                
                switch(e.Message)
                {
                    case var m when m == Msg._pressed:
                        switch(e.Parameter)
                        {
                            case var n when Msg._settings.Equals(n):
                                Data.Instance.Page.Value = Msg._settings;
                                break;
                            case var n when Msg._main.Equals(n):
                                if (Data.Instance.Page.Value.Equals(Msg._settings))
                                {
                                    IChildVM.Save();
                                    if(Status.LatencyChanged)
                                    {
                                        Status.LatencyChanged = false;
                                        Data.Instance.NoAction.Value = Msg._main;
                                        Data.Instance.YesAction.Value = Msg._exit;
                                        Data.Instance.YesNoQuestion.Value = Lang.ConfirmExit.Replace(@"\n", "\r\n");
                                        Data.Instance.Page.Value = Msg._yesno;
                                        break;
                                    }
                                }
                                Data.Instance.Page.Value = Msg._main;
                                break;
                            case var n when Msg._exit.Equals(n):
                                Application.Current.Quit();
                                break;
                            case var n when Msg._editlang.Equals(n):
                                Data.Instance.LanguageDesignator.Value = Data.Instance.Language.Value;
                                Data.Instance.LanguageData.Value = Lang.GetLanguageData().LinesToString();
                                Data.Instance.Page.Value = Msg._language;
                                break;
                            case var n when Msg._editlangreturn.Equals(n):
                                Data.Instance.Page.Value = Msg._language;
                                break;
                            case var n when Msg._applylang.Equals(n):
                                Lang.SaveLanguage(Data.Instance.LanguageDesignator.Value, Data.Instance.LanguageData.Value.StringtoLines());
                                Lang.FindAvailable();
                                if (Data.Instance.Language.Value == Data.Instance.LanguageDesignator.Value)
                                    Data.Instance.InitLanguageModels();
                                else
                                    Data.Instance.Language.Value = Data.Instance.LanguageDesignator.Value;
                                Data.Instance.Page.Value = Msg._settings;
                                break;
                            case var n when Msg._factoryask.Equals(n):
                                Data.Instance.NoAction.Value = Msg._settings;
                                Data.Instance.YesAction.Value = Msg._factory;
                                Data.Instance.YesNoQuestion.Value = Lang.ConfirmFactory.Replace(@"\n", "\r\n");
                                Data.Instance.Page.Value = Msg._yesno;
                                break;
                            case var n when Msg._factory.Equals(n):
                                IChildVM.Clear();
                                Application.Current.Quit();
                                break;
                            case var n when Msg._deletelangask.Equals(n):
                                Data.Instance.NoAction.Value = Msg._editlangreturn;
                                Data.Instance.YesAction.Value = Msg._deletelang;
                                Data.Instance.YesNoQuestion.Value = $"{Lang.ConfirmDelLang.Replace(@"\n","\r\n")} {Data.Instance.LanguageDesignator.Value}";
                                Data.Instance.Page.Value = Msg._yesno;
                                break;
                            case var n when Msg._deletelang.Equals(n):
                                string ltd = Data.Instance.LanguageDesignator.Value;
                                Data.Instance.Language.Value = "en";
                                Lang.DeleteLanguge(ltd);
                                Data.Instance.Page.Value = Msg._settings;
                                break;
                            case var n when Msg._editbgcol.Equals(n):
                                EditColor(Lang.LCDBackground, Data.Instance.LCDBackground);
                                break;
                            case var n when Msg._editfgcol.Equals(n):
                                EditColor(Lang.LCDForeground, Data.Instance.LCDForeground);
                                break;
                            case var n when Msg._coloreditok.Equals(n):
                                if(editedColor != null)
                                   editedColor.Value = Data.Instance.ColEditColor.Value;
                                Data.Instance.Page.Value = Msg._settings;
                                break;
                            case var n when n is string s && int.TryParse(s, out int i):
                                keys |= (1 << i);
                                MessageHub.Send(Msg._keypress, i);
                                break;
                        }
                        break;
                    case var m when m == Msg._released:
                        switch(e.Parameter)
                        {
                            case var n when n is string s && int.TryParse(s, out int i):
                                keys &= ~(1 << i);
                                if (keys == 0) MessageHub.Send(Msg._keypress, 19);
                                break;
                        }
                        break;
                }
            };
        }

        private static void EditColor(string caption, ViewModel<Color> color)
        {
            editedColor = color;
            Data.Instance.ColEditCaption.Value = caption;
            Data.Instance.ColEditColor.Value = color.Value;
            Data.Instance.ColEditOKAction.Value = Msg._coloreditok;
            Data.Instance.ColEditCancelAction.Value = Msg._settings;
            Data.Instance.Page.Value = Msg._coloredit;
        }
    }
}
