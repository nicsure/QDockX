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
            case "Main":
                return base.OnBackButtonPressed();
            case "YesNo":
                MessageHub.Send("Pressed", Data.Instance.NoAction.Value);
                return true;
            case "Language":
                Data.Instance.Page.Value = "Settings";
                return true;
            case "ColorEdit":
                MessageHub.Send("Pressed", Data.Instance.ColEditCancelAction.Value);
                return true;
            default:
                MessageHub.Send("Pressed", "Main");
                return true;
        }        
    }
}

