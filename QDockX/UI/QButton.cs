using QDockX.Context;
using QDockX.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public class QButton : Button
    {
        public QButton() : base() 
        {
            MeasureInvalidated += QButton_MeasureInvalidated;
            SizeChanged += QButton_MeasureInvalidated;
            Pressed += QButton_Pressed;
            Released += QButton_Released;
            Unfocused += QButton_Released;            
            Padding = new(4,0,4,4);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if(propertyName.Equals("IsEnabled"))
            {
                Opacity = IsEnabled ? 1 : 0.5;
            }
        }

        private void QButton_Released(object sender, EventArgs e)
        {
            Background = ReleasedBackground;
            if (Tag is string tag)
                MessageHub.Send("Released", tag);
        }

        private void QButton_Pressed(object sender, EventArgs e)
        {
            Background = PressedBackground;
            if (Tag is string tag)
                MessageHub.Send("Pressed", tag);
        }

        private static void QButton_MeasureInvalidated(object sender, EventArgs e)
        {
            if (sender is QButton button && button.AutoSize && button.Width > 0 && button.Height > 0 && button.Text != null && button.Text.Length > 0)
            {
                double width = button.Width;
                double height = button.Height;
                button.MeasureInvalidated -= QButton_MeasureInvalidated;
                Size requiredSize;
                button.FontSize = 0.1;
                do
                {
                    button.FontSize++;
                    requiredSize = button.Measure(double.PositiveInfinity, double.PositiveInfinity).Request;
                }
                while (button.FontSize < 1000 && requiredSize.Width < width && requiredSize.Height < height);
                button.FontSize--;
                button.MeasureInvalidated += QButton_MeasureInvalidated;
                button.MasterFontSize = button.FontSize;
            }
        }

        public Brush ReleasedBackground
        {
            get { return (Brush)GetValue(ReleasedBackgroundProperty); }
            set { SetValue(ReleasedBackgroundProperty, value); }
        }
        public static readonly BindableProperty ReleasedBackgroundProperty =
            BindableProperty.Create(nameof(ReleasedBackground), typeof(Brush), typeof(QButton), null,
                propertyChanged: (BindableObject bindable, object _, object newValue)
                    => (bindable as QButton).Background = newValue as Brush);

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }
        public static readonly BindableProperty PressedBackgroundProperty =
            BindableProperty.Create(nameof(PressedBackground), typeof(Brush), typeof(QButton), null);

        public object Tag
        {
            get { return GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }
        public static readonly BindableProperty TagProperty =
            BindableProperty.Create(nameof(Tag), typeof(object), typeof(QButton), null);

        public double MasterFontSize
        {
            get { return (double)GetValue(MasterFontSizeProperty); }
            set { SetValue(MasterFontSizeProperty, value); }
        }
        public static readonly BindableProperty MasterFontSizeProperty =
            BindableProperty.Create(nameof(MasterFontSize), typeof(double), typeof(QButton), 10.0);

        public bool AutoSize
        {
            get { return (bool)GetValue(AutoSizeProperty); }
            set { SetValue(AutoSizeProperty, value); }
        }
        public static readonly BindableProperty AutoSizeProperty =
            BindableProperty.Create(nameof(AutoSize), typeof(bool), typeof(QButton), defaultValue: true);


        private static bool fShown = false;
        public static void SetFunctionLabels(bool f)
        {
            if(f)
            {
                if (!fShown)
                {
                    Data.Instance.Button0.Value = Lang.ButtonF0;
                    Data.Instance.Button1.Value = Lang.ButtonF1;
                    Data.Instance.Button2.Value = Lang.ButtonF2;
                    Data.Instance.Button3.Value = Lang.ButtonF3;
                    Data.Instance.Button4.Value = Lang.ButtonF4;
                    Data.Instance.Button5.Value = Lang.ButtonF5;
                    Data.Instance.Button6.Value = Lang.ButtonF6;
                    Data.Instance.Button7.Value = Lang.ButtonF7;
                    Data.Instance.Button8.Value = Lang.ButtonF8;
                    Data.Instance.Button9.Value = Lang.ButtonF9;
                    Data.Instance.Button14.Value = Lang.ButtonF14;
                    fShown = true;
                }
            }
            else
            {
                if (fShown)
                {
                    Data.Instance.Button0.Value = Lang.Button0;
                    Data.Instance.Button1.Value = Lang.Button1;
                    Data.Instance.Button2.Value = Lang.Button2;
                    Data.Instance.Button3.Value = Lang.Button3;
                    Data.Instance.Button4.Value = Lang.Button4;
                    Data.Instance.Button5.Value = Lang.Button5;
                    Data.Instance.Button6.Value = Lang.Button6;
                    Data.Instance.Button7.Value = Lang.Button7;
                    Data.Instance.Button8.Value = Lang.Button8;
                    Data.Instance.Button9.Value = Lang.Button9;
                    Data.Instance.Button14.Value = Lang.Button14;
                    fShown = false;
                }
            }
        }

    }
}
