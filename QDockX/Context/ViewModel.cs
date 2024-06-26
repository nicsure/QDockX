﻿using QDockX.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Context
{
    public class Conversion
    {
        private readonly IVM VM;
        public Conversion(IVM vm) => VM = vm;
        public object this[string key]
        {
            get => Converter.Convert(key, VM.Get());
            set => VM.Set(Converter.Convert($"{key}Back", value));
        }
    }

    public abstract class Converter
    {
        private static Converter converter;
        public static void SetConverter(Converter converterClass) => converter = converterClass;
        public static object Convert(string key, object value) => converter.PerformConvert(key, value);
        public abstract object PerformConvert(string key, object value);
    }

    public class ViewModel<T> : IVM, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs eventArgs1 = new(nameof(Value));
        private static readonly PropertyChangedEventArgs eventArgs2 = new(nameof(Convert));
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IVM[] children = Array.Empty<IVM>();
        private readonly string config = null;
        private readonly string autoConvert = null;
        private Conversion conversion = null;
        public Conversion Convert => conversion ??= new(this);
        
        public T Value
        {
            get
            {
                return autoConvert == null ? val : (T)Converter.Convert(autoConvert, null);
            }
            set
            {
                if (autoConvert != null)
                {
                    Converter.Convert($"{autoConvert}Back", value);
                }
                else
                {
                    if (value?.Equals(val) ?? false)
                        return;
                    val = value;
                    OnChange();
                }
            }
        }
        private T val;

        public int ForceUpdate
        {
            get => 0;
            set => OnChange();
        }

        public ViewModel(string autoConvert)
        {
            this.autoConvert = autoConvert;
        }

        public ViewModel(T initialValue, string config, params IVM[] children)
        {
            val = IVM.ConfigFile.Read(config, initialValue);
            this.config = config;
            this.children = children;
        }

        public object Get() => val;

        public void Set(object obj) => Value = (T)obj;

        public bool SuspendChildUpdates { get; set; } = false;

        public void OnChange()
        {
            (_ = PropertyChanged)?.Invoke(this, eventArgs1);
            (_ = PropertyChanged)?.Invoke(this, eventArgs2);
            if (!SuspendChildUpdates)
            {
                foreach (var child in children)
                    child.OnChange();
            }
            IVM.ConfigFile.Write(config, val);
        }
    }

    public interface IVM
    {
        public static ConfigFile ConfigFile { get; } = new("main");
        public static void Clear() => ConfigFile.Delete();
        public static void Save() => ConfigFile.Save();
        public object Get();
        public void Set(object obj);
        public void OnChange();
    }

}
