using QDockX.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Context
{
    public class DataConverter : Converter
    {
        //private static readonly Data data = Data.Instance;

        public override object PerformConvert(string key, object value)
        {
            switch (key)
            {
                case "RGBEdit": return new Color(
                        (float)Data.Instance.ColEditRed.Value,
                        (float)Data.Instance.ColEditGreen.Value,
                        (float)Data.Instance.ColEditBlue.Value
                    );
                case "RGBEditBack":
                    if (value is Color c)
                    {
                        Data.Instance.ColEditRed.Value = c.Red;
                        Data.Instance.ColEditGreen.Value = c.Green;
                        Data.Instance.ColEditBlue.Value = c.Blue;
                    }
                    return null;                
                case "NotEN": return !(value == null || value.ToString().Length == 0 || value.ToString().ToLower().Equals("en"));
                case "Not": return !((bool)value);
                case "ToBrush": return new SolidColorBrush(value as Color);
                case "ToArgb": return (value as Color).ToArgbHex();
                case "ToArgbBack": return Color.FromArgb(value as string);
                case var n when n.StartsWith("EqIC"): return key[4..].ToLower().Equals(value.ToString().ToLower());
                case var n when n.StartsWith("NeIC"): return !key[4..].ToLower().Equals(value.ToString().ToLower());
                case var n when n.StartsWith("Eq"): return key[2..].Equals(value.ToString());
                case var n when n.StartsWith("Ne"): return !key[2..].Equals(value.ToString());
            }
            if (key.Length > 2 && ToDoubles(key[2..], value, out var A, out var B))
            {
                switch (key)
                {
                    case var n when n.StartsWith("Gt"): return B > A;
                    case var n when n.StartsWith("Lt"): return B < A;
                    case var n when n.StartsWith("Ge"): return B >= A;
                    case var n when n.StartsWith("Le"): return B <= A;
                }
            }
            throw new NotSupportedException($"{Lang.InvalidConv}: {key}");
        }

        private static bool ToDoubles(string key, object value, out double A, out double B)
        {
            if(double.TryParse(key, out A) && double.TryParse(value.ToString(), out B))
                return true;
            A = B = 0;
            return false;
        }
    }
}
;