using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace Ntreev.ModernUI.Shell.MenuItems.ViewMenus.MessageBoxMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(MessageBoxMenuItem))]
    class YesNoCancelMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public YesNoCancelMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "Yes_NoCancel";
        }

        protected async override void OnExecute(object parameter)
        {
            await AppMessageBox.ShowAsync("YesNoCancel", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
        }
    }
}
