
using Microsoft.Maui.Controls.Shapes;
using QDockX.Debug;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public class Potentiometer : ContentView
    {
        private readonly Grid grid, notchGrid;
        private readonly Ellipse notch, ring, inner;
        private double panY;

        public Potentiometer() : base()
        {
            PanGestureRecognizer pan = new();
            GestureRecognizers.Add(pan);
            pan.PanUpdated += Gesture_PanUpdated;     
            Content = grid = new Grid();
            grid.Add(ring = new Ellipse() { Fill = RingColor });
            grid.Add(inner = new Ellipse() { Fill = InnerColor, ScaleX = 0.985, ScaleY = 0.985 });
            notchGrid = new();
            grid.Add(notchGrid);
            notchGrid.Add(notch = new Ellipse() { Fill = NotchColor, ScaleX = 0.2, ScaleY = 0.2 });
            SizeChanged += Potentiometer_SizeChanged;
        }

        private void Potentiometer_SizeChanged(object sender, EventArgs e)
        {
            double m = Math.Min(Width, Height);
            grid.WidthRequest = grid.HeightRequest = m;
            notch.TranslationY = -(m * 0.35);
        }
        
        private void Gesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {            
            switch(e.StatusType)
            {
                case GestureStatus.Canceled:
                case GestureStatus.Completed:
                    Gesturing = false;
                    break;
                case GestureStatus.Started:
                    Gesturing = true;
                    panY = 0;
                    break;
                case GestureStatus.Running:
                    double dy = e.TotalY - panY;
                    panY = e.TotalY;
                    double a = notchGrid.Rotation - (dy * 2);
                    while (a > 180) a -= 360;
                    while (a < -180) a += 360;
                    a = a < -150 ? -150 : a > 150 ? 150 : a;
                    Value = (a + 150) / 300;
                    break;
            }
        }

        public bool Gesturing
        {
            get { return (bool)GetValue(GesturingProperty); }
            set { SetValue(GesturingProperty, value); }
        }
        public static readonly BindableProperty GesturingProperty =
            BindableProperty.Create(nameof(Gesturing), typeof(bool), typeof(Potentiometer), defaultValue: false);

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(double), typeof(Potentiometer), defaultValue: 0.5, propertyChanged: ValueChanged);
        private static void ValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is Potentiometer p && newValue is double d)
            {
                p.notchGrid.Rotation = (d * 300) - 150;
            }
        }

        public Brush RingColor
        {
            get { return (Brush)GetValue(RingColorProperty); }
            set { SetValue(RingColorProperty, value); }
        }
        public static readonly BindableProperty RingColorProperty =
            BindableProperty.Create(nameof(RingColor), typeof(Brush), typeof(Potentiometer), defaultValue: Brush.Red, propertyChanged: RingColorChanged);
        private static void RingColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Potentiometer p && newValue is Brush b)
            {
                p.ring.Fill = b;
            }
        }

        public Brush InnerColor
        {
            get { return (Brush)GetValue(InnerColorProperty); }
            set { SetValue(InnerColorProperty, value); }
        }
        public static readonly BindableProperty InnerColorProperty =
            BindableProperty.Create(nameof(InnerColor), typeof(Brush), typeof(Potentiometer), defaultValue: Brush.Green, propertyChanged: InnerColorChanged);
        private static void InnerColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Potentiometer p && newValue is Brush b)
            {
                p.inner.Fill = b;
            }
        }

        public Brush NotchColor
        {
            get { return (Brush)GetValue(NotchColorProperty); }
            set { SetValue(NotchColorProperty, value); }
        }
        public static readonly BindableProperty NotchColorProperty =
            BindableProperty.Create(nameof(NotchColor), typeof(Brush), typeof(Potentiometer), defaultValue: Brush.Blue, propertyChanged: NotchColorChanged);
        private static void NotchColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Potentiometer p && newValue is Brush b)
            {
                p.notch.Fill = b;
            }
        }

    }
}
