using JSSoft.ModernUI.Framework;
using JSSoft.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace JSSoft.ModernUI.Shell.MenuItems.ViewMenus.MessageBoxMenus
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
