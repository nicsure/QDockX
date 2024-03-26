using QDockX.Buttons;
using QDockX.Context;
using QDockX.Debug;
using QDockX.Network;
using QDockX.Radio;
using QDockX.Sound;
using QDockX.Util;
using System.Globalization;
using System.Reflection;

namespace QDockX;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
        Shared.Page = this;
        BindingContext = Data.Instance;
        ConnectionPreset.Populate();
        Watchdog.Watch = GetPermission();
        InitializeComponent();
        Shared.LanguageEditor = LangEditor;
        Watchdog.Init();
        ButtonProcessor.Init();
        Playback.Init();
        Capture.Init();
        QDNH.Init();
        Serial.Init();
    }

    private static async Task GetPermission()
    {
        try
        {
            using var task = Permissions.RequestAsync<Permissions.Microphone>();
            var status = await task;
            if (status != PermissionStatus.Granted)
            {
                using var task2 = Shared.Alert("Alert", "Without granting permission for the microphone you will not be able to transmit.", "OK");
                await task2;
            }
            else
                Data.Instance.AllowPTT.Value = true;
        }
        catch(Exception ex) { DebugLog.Exception(ex); }
    }

    protected override async void OnDisappearing()
    {
        Shared.LanguageEditor = null;
        ConnectionPreset.Serialize();
        IVM.Save();
        await Watchdog.Delay(200);
        base.OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shared.LanguageEditor = LangEditor;
        MessageHub.Send(Msg._keepscreenon, Data.Instance.KeepScreenOn.Value);
    }

    protected override bool OnBackButtonPressed()
    {
        switch(Data.Instance.Page.Value)
        {
            case var n when n == Msg._main:
                return base.OnBackButtonPressed();
            case var n when n == Msg._yesno:
                MessageHub.Send(Msg._pressed, Data.Instance.NoAction.Value);
                return true;
            case var n when n == Msg._language:
                Data.Instance.Page.Value = Msg._settings;
                return true;
            case var n when n == Msg._coloredit:
                MessageHub.Send(Msg._pressed, Data.Instance.ColEditCancelAction.Value);
                return true;
            case var n when n == Msg._stringinput:
                MessageHub.Send(Msg._pressed, Data.Instance.StringInputCancelAction.Value);
                return true;
            default:
                MessageHub.Send(Msg._pressed, Msg._main);
                return true;
        }        
    }
}

