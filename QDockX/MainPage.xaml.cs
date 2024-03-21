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

}

