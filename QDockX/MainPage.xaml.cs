using QDockX.Buttons;
using QDockX.Context;
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
        BindingContext = Data.Instance;
		InitializeComponent();
        Shared.LanguageEditor = LangEditor;
        Watchdog.Init();
        ButtonProcesor.Init();
        Playback.Init();
        Capture.Init();
        QDNH.Init();
        Serial.Init();
    }

    protected override async void OnDisappearing()
    {
        Shared.LanguageEditor = null;
        IChildVM.Save();
        using var delay = Task.Delay(200);
        await delay;
        base.OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shared.LanguageEditor = LangEditor;
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
            default:
                MessageHub.Send(Msg._pressed, Msg._main);
                return true;
        }        
    }
}

