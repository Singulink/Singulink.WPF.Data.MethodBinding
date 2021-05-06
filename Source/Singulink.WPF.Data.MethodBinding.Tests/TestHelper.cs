using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Singulink.WPF.Data.Tests
{
    public static class TestHelper
    {
        public static void RunMethodBinding(object? dataContext, MethodBindingExtension binding)
        {
            var obj = new FrameworkElement {
                Name = "TestElement",
                DataContext = dataContext,
            };

            var handler = (RoutedEventHandler)binding.ProvideValue(new ServiceProvider(obj, typeof(FrameworkElement).GetEvent("Loaded")!));
            handler.Invoke(obj, new RoutedEventArgs(FrameworkElement.LoadedEvent, obj));
        }

        private class ServiceProvider : IServiceProvider, IProvideValueTarget
        {
            public object TargetObject { get; }

            public object TargetProperty { get; }

            public ServiceProvider(object targetObject, object targetProperty)
            {
                TargetObject = targetObject;
                TargetProperty = targetProperty;
            }

            public object? GetService(Type serviceType) => serviceType.IsInstanceOfType(this) ? this : null;
        }
    }
}
