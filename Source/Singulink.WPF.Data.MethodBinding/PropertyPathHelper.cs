using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Singulink.WPF.Data
{
    internal static class PropertyPathHelper
    {
        private static readonly object s_fallbackValue = new object();

        public static object? Evaluate(PropertyPath path, object source)
        {
            var target = new DependencyTarget();
            var binding = new Binding() { Path = path, Source = source, Mode = BindingMode.OneTime, FallbackValue = s_fallbackValue };
            BindingOperations.SetBinding(target, DependencyTarget.ValueProperty, binding);

            if (target.Value == s_fallbackValue)
                throw new ArgumentException($"Could not resolve property path '{path.Path}' on source object type '{source.GetType()}'.");

            return target.Value;
        }

        private class DependencyTarget : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DependencyTarget));

            public object? Value
            {
                get => GetValue(ValueProperty);
                set => SetValue(ValueProperty, value);
            }
        }
    }
}
