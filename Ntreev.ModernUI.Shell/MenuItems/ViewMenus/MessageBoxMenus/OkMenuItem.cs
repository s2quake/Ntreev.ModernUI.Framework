using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Shell.Properties;
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
            this.DisplayName = "MessageBox";
        }

        protected async override void OnExecute(object parameter)
        {
            await AppMessageBox.ShowAsync("123", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
