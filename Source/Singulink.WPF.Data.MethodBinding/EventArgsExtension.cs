using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Singulink.WPF.Data
{
    /// <summary>
    /// Markup extension that enables passing of EventArgs data as a parameter to a bound method.
    /// </summary>
    public sealed class EventArgsExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// </summary>
        public PropertyPath? Path { get; set; }

        /// <summary>
        /// Gets or sets the converter to use.
        /// </summary>
        public IValueConverter? Converter { get; set; }

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Converter"/>.
        /// </summary>
        public object? ConverterParameter { get; set; }

        /// <summary>
        /// Gets or sets the converter target type to pass to the <see cref="Converter"/>. Default is '<see cref="object"/>'.
        /// </summary>
        public Type ConverterTargetType { get; set; } = typeof(object);

        /// <summary>
        /// Gets or sets the culture in which to evaluate the converter.
        /// </summary>
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo? ConverterCulture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgsExtension"/> class.
        /// </summary>
        public EventArgsExtension() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgsExtension"/> class using the specified path.
        /// </summary>
        public EventArgsExtension(string path)
        {
            Path = new PropertyPath(path);
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        internal object? GetArgumentValue(EventArgs eventArgs, XmlLanguage? language)
        {
            if (Path == null)
                return eventArgs;

            object? value = PropertyPathHelper.Evaluate(Path, eventArgs);

            if (Converter != null)
                value = Converter.Convert(value, ConverterTargetType, ConverterParameter, ConverterCulture ?? language?.GetSpecificCulture() ?? CultureInfo.CurrentUICulture);

            return value;
        }
    }
}
