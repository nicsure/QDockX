using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public class AutoSizeEntry : Entry, IAutoFontSizable
    {
        public AutoSizeEntry() : base() { }

        public string Group
        {
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }
        public static readonly BindableProperty GroupProperty =
            BindableProperty.Create(nameof(Group), typeof(string), typeof(AutoSizeEntry),
                defaultValue: null,
                propertyChanged: (b, o, n) => IAutoFontSizable.Register(b, o, n));
    }
}
