using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Singulink.WPF.Data
{
    /// <summary>
    /// Markup extension that provides direct-to-method binding capabilities for events.
    /// </summary>
    public class MethodBindingExtension : MarkupExtension
    {
        private static readonly List<DependencyProperty> s_storageProperties = new List<DependencyProperty>();

        private readonly object?[] _arguments;
        private readonly List<DependencyProperty> _argumentProperties = new List<DependencyProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? method) : this(new[] { method }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1) : this(new[] { arg0, arg1 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2) : this(new[] { arg0, arg1, arg2 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2, object? arg3) : this(new[] { arg0, arg1, arg2, arg3 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2, object? arg3, object? arg4) : this(new[] { arg0, arg1, arg2, arg3, arg4 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBindingExtension"/> class.
        /// </summary>
        public MethodBindingExtension(object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8) : this(new[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }) { }

        private MethodBindingExtension(object?[] arguments)
        {
            _arguments = arguments;
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            if (provideValueTarget == null)
                return this;

            Type? eventHandlerType = null;

            if (provideValueTarget.TargetProperty is EventInfo eventInfo) {
                eventHandlerType = eventInfo.EventHandlerType;
            }
            else if (provideValueTarget.TargetProperty is MethodInfo methodInfo) {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length == 2)
                    eventHandlerType = parameters[1].ParameterType;
            }

            if (provideValueTarget.TargetObject is DependencyObject target && eventHandlerType != null) {
                foreach (object? argument in _arguments) {
                    var argumentProperty = SetUnusedStorageProperty(target, argument);
                    _argumentProperties.Add(argumentProperty);
                }

                return CreateEventHandler(target, eventHandlerType);
            }

            return this;
        }

        private Delegate CreateEventHandler(DependencyObject source, Type eventHandlerType)
        {
            EventHandler handler = (sender, eventArgs) => {
                object arg0 = source.GetValue(_argumentProperties[0]);

                if (arg0 == null) {
                    Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] First method binding argument is required and cannot resolve to null - method name or method target expected.");
                    return;
                }

                int methodArgsStart;
                object methodTarget;

                // If the first argument is a string then it must be the name of the method to invoke on the data context.
                // If not then it is the excplicit method target object and the second argument will be name of the method to invoke.

                string? methodName;

                if ((methodName = arg0 as string) != null) {
                    if (source is FrameworkElement element) {
                        if (element.DataContext == null) {
                            Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] Null data context on element '{element}'.");
                            return;
                        }

                        methodTarget = element.DataContext;
                        methodArgsStart = 1;
                    }
                    else {
                        Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] Shorthand syntax not supported on element type '{source.GetType()}' because it is not a FrameworkElement and has no data context. Method target must be specified.");
                        return;
                    }
                }
                else if (_argumentProperties.Count >= 2) {
                    methodTarget = arg0;
                    methodArgsStart = 2;

                    object arg1 = source.GetValue(_argumentProperties[1]);

                    if (arg1 == null) {
                        Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] First argument resolved as a method target object of type '{methodTarget.GetType()}', second argument must resolve to a method name and cannot resolve to null.");
                        return;
                    }

                    if ((methodName = arg1 as string) == null) {
                        Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] First argument resolved as a method target object of type '{methodTarget.GetType()}', second argument (method name) must resolve to a '{typeof(string)}' (actual type: '{arg1.GetType()}').");
                        return;
                    }
                }
                else {
                    Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] Method name must resolve to a '{typeof(string)}' (actual type: '{arg0.GetType()}').");
                    return;
                }

                object?[] arguments = new object[_argumentProperties.Count - methodArgsStart];

                for (int i = methodArgsStart; i < _argumentProperties.Count; i++) {
                    object? argValue = source.GetValue(_argumentProperties[i]);

                    if (argValue is EventSenderExtension)
                        argValue = sender;
                    else if (argValue is EventArgsExtension eventArgsEx)
                        argValue = eventArgsEx.GetArgumentValue(eventArgs, (source as FrameworkElement)?.Language);

                    arguments[i - methodArgsStart] = argValue;
                }

                var methodTargetType = methodTarget.GetType();

                // Try invoking the method by resolving it based on the arguments provided

                try {
                    methodTargetType.InvokeMember(methodName, BindingFlags.InvokeMethod, null, methodTarget, arguments, null);
                    return;
                }
                catch (MissingMethodException) { }

                // Couldn't match a method with the raw arguments, so check if we can find a method with the same name
                // and parameter count and try to convert any XAML string arguments to match the method parameter types

                MethodInfo? method = methodTargetType.GetMethods().SingleOrDefault(m => m.Name == methodName && m.GetParameters().Length == arguments.Length);

                if (method != null) {
                    var parameters = method.GetParameters();

                    for (int i = 0; i < _arguments.Length; i++) {
                        if (arguments[i] == null) {
                            if (parameters[i].ParameterType.IsValueType) {
                                method = null;
                                break;
                            }
                        }
                        else if (_arguments[i] is string stringArg && parameters[i].ParameterType != typeof(string)) {
                            // The original value provided for this argument was a XAML string so try to convert it
                            arguments[i] = TypeDescriptor.GetConverter(parameters[i].ParameterType).ConvertFromString(stringArg);
                        }
                        else if (!parameters[i].ParameterType.IsInstanceOfType(arguments[i])) {
                            method = null;
                            break;
                        }
                    }

                    method?.Invoke(methodTarget, arguments);
                }

                if (method == null)
                    Trace.TraceWarning($"[{nameof(MethodBindingExtension)}] Could not find a method '{methodName}' on target type '{methodTargetType}' that accepts the parameters provided.");
            };

            return Delegate.CreateDelegate(eventHandlerType, handler.Target, handler.Method);
        }

        private static DependencyProperty SetUnusedStorageProperty(DependencyObject obj, object? value)
        {
            var property = s_storageProperties.Find(p => obj.ReadLocalValue(p) == DependencyProperty.UnsetValue);

            if (property == null) {
                property = DependencyProperty.RegisterAttached("Storage" + s_storageProperties.Count, typeof(object), typeof(MethodBindingExtension), new PropertyMetadata());
                s_storageProperties.Add(property);
            }

            if (value is MarkupExtension markupExtension) {
                object resolvedValue = markupExtension.ProvideValue(new ServiceProvider(obj, property));
                obj.SetValue(property, resolvedValue);
            }
            else {
                obj.SetValue(property, value);
            }

            return property;
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
