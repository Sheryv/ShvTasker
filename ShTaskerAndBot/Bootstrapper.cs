using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using ShTaskerAndBot.Models;
using ShTaskerAndBot.Utils;
using ShTaskerAndBot.ViewModels;

namespace ShTaskerAndBot
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

       /* private static readonly IValueConverter CmdTypeToStringConverter = new CmdTypeToStringConverter();

        protected override void Configure()
        {

            var oldApplyConverterFunc = ConventionManager.ApplyValueConverter;

            ConventionManager.ApplyValueConverter = (binding, bindableProperty, property) => {
                if (bindableProperty == UIElement.Opacity && typeof(CmdTypes).IsAssignableFrom(property.PropertyType))
                    //                                ^^^^^^^           ^^^^^^
                    //                             Property in XAML     Property in view-model
                    binding.Converter = CmdTypeToStringConverter;
                //                  ^^^^^^^^^^^^^^^^^^^^^^^^^
                //                 Our converter used here.

                // else we use the default converter
                else
                    oldApplyConverterFunc(binding, bindableProperty, property);

            };
        }*/
    }
}