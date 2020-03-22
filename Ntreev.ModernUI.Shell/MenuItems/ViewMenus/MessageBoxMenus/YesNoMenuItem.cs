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
    class YesNoMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public YesNoMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "_YesNo";
        }

        protected async override void OnExecute(object parameter)
        {
            await AppMessageBox.ShowAsync("YesNo", MessageBoxButton.YesNo, MessageBoxImage.Information);
        }
    }
}
