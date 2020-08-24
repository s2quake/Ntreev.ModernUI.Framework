using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace Ntreev.ModernUI.Shell.MenuItems.ViewMenus.MessageBoxMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(MessageBoxMenuItem))]
    class OkMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public OkMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "_OK";
        }

        protected async override void OnExecute(object parameter)
        {
            await AppMessageBox.ShowAsync("OK", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
