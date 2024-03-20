using QDockX.Buttons;
using QDockX.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Context
{
    public class Data : INotifyPropertyChanged
    {
        public static Data Instance { get; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => (_ = PropertyChanged)?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        // Data models 
        public ViewModel<string> Language { get; } = new("en", nameof(Language));
        public ViewModel<string> Button17 { get; private set; } = new("💡", nameof(Button17));
        public ViewModel<string> Button18 { get; private set; } = new("SQ", nameof(Button18));
        public ViewModel<Color> LCDBackground { get; } = new(Colors.Black, nameof(LCDBackground));
        public ViewModel<Color> LCDForeground { get; } = new(Colors.LimeGreen, nameof(LCDForeground));
        public ViewModel<Color> LED { get; } = new(Colors.Black, null);
        public ViewModel<string> Page { get; } = new("Main", null);
        public ViewModel<string> Host { get; } = new("192.168.0.2", nameof(Host));
        public ViewModel<int> Port { get; } = new(18822, nameof(Port));
        public ViewModel<string> Password { get; } = new(string.Empty, nameof(Password));
        public ViewModel<int> Latency { get; } = new(100, nameof(Latency));
        public ViewModel<int> SmallWidth { get; } = new(100, nameof(Latency));
        public ViewModel<int> SmallHeight { get; } = new(0, nameof(Latency));
        public ViewModel<int> MediumWidth { get; } = new(150, nameof(Latency));
        public ViewModel<int> MediumHeight { get; } = new(0, nameof(Latency));
        public ViewModel<int> LargeWidth { get; } = new(190, nameof(Latency));
        public ViewModel<int> LargeHeight { get; } = new(12, nameof(Latency));
        public IList<string> Languages
        {
            get => Lang.Available;
            set => OnPropertyChanged(nameof(Languages));
        }

        // Language Models
        public ViewModel<string> Button0 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button1 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button2 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button3 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button4 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button5 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button6 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button7 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button8 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button9 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button10 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button11 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button12 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button13 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button14 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button15 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> Button16 { get; private set; } = new(string.Empty, null);
        public ViewModel<string> QDNHHostLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> QDNHPortLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> QDNHPasswordLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> LCDBackgroundLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> LCDForegroundLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> ExitLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> User1Label { get; private set; } = new(string.Empty, null);
        public ViewModel<string> User2Label { get; private set; } = new(string.Empty, null);
        public ViewModel<string> LatencyLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> LanguageLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> FontLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> SizeLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> OffsetLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> SmallLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> MediumLabel { get; private set; } = new(string.Empty, null);
        public ViewModel<string> LargeLabel { get; private set; } = new(string.Empty, null);

        public Data()
        {
            Converter.SetConverter(new DataConverter());
            InitLanguageModels();
            Language.PropertyChanged += (object sender, PropertyChangedEventArgs e) => InitLanguageModels();
        }

        public void InitLanguageModels()
        {
            Lang.LoadLanguage(Language.Value);
            Button0.Value = Lang.Button0;
            Button1.Value = Lang.Button1;
            Button2.Value = Lang.Button2;
            Button3.Value = Lang.Button3;
            Button4.Value = Lang.Button4;
            Button5.Value = Lang.Button5;
            Button6.Value = Lang.Button6;
            Button7.Value = Lang.Button7;
            Button8.Value = Lang.Button8;
            Button9.Value = Lang.Button9;
            Button10.Value = Lang.Button10;
            Button11.Value = Lang.Button11;
            Button12.Value = Lang.Button12;
            Button13.Value = Lang.Button13;
            Button14.Value = Lang.Button14;
            Button15.Value = Lang.Button15;
            Button16.Value = Lang.Button16;
            QDNHHostLabel.Value = Lang.QDNHHost;
            QDNHPortLabel.Value = Lang.QDNHPort;
            QDNHPasswordLabel.Value = Lang.QDNHPassword;
            LCDBackgroundLabel.Value = Lang.LCDBackground;
            LCDForegroundLabel.Value = Lang.LCDForeground;
            ExitLabel.Value = Lang.Exit;
            User1Label.Value = Lang.User1;
            User2Label.Value = Lang.User2;
            LatencyLabel.Value = Lang.Latency;
            LanguageLabel.Value = Lang.Language;
            FontLabel.Value = Lang.Font;
            SizeLabel.Value = Lang.Size;
            OffsetLabel.Value = Lang.Offset;
            SmallLabel.Value = Lang.Small;
            MediumLabel.Value = Lang.Medium;
            LargeLabel.Value = Lang.Large;
        }
    }


}
