using JSSoft.ModernUI.Framework;
using JSSoft.ModernUI.Shell.Dialogs.ViewModels;
using System;
using System.ComponentModel.Composition;

namespace JSSoft.ModernUI.Shell.MenuItems.ViewMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(ViewMenuItem))]
    class GridControlMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public GridControlMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "GridControl";
        }

        protected async override void OnExecute(object parameter)
        {
            var dialog = new GridControlViewModel();
            await dialog.ShowDialogAsync();
        }
    }
}
