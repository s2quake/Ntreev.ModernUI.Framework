using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Shell.Properties;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace Ntreev.ModernUI.Shell.MenuItems.FileMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(MessageBoxMenuItem))]
    [Category("Quit")]
    [Order(int.MaxValue)]
    class QuitMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public QuitMenuItem()
        {
            this.DisplayName = Resources.MenuItem_Quit;
            this.InputGesture = new KeyGesture(Key.F4, ModifierKeys.Alt);
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
