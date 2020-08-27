using JSSoft.ModernUI.Framework;
using JSSoft.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace JSSoft.ModernUI.Shell.MenuItems.ViewMenus.MessageBoxMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(MessageBoxMenuItem))]
    class OkCancelMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public OkCancelMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "OK_Cancel";
        }

        protected async override void OnExecute(object parameter)
        {
            await AppMessageBox.ShowAsync("OKCancel", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        }
    }
}
