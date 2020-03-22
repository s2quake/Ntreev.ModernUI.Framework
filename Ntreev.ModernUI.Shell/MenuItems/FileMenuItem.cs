using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Shell.Properties;

namespace Ntreev.ModernUI.Shell.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IShell))]
    [Order(0)]
    [CategoryDefinition("Settings")]
    [CategoryDefinition("Recent")]
    [CategoryDefinition("Quit")]
    class MessageBoxMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public MessageBoxMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = Resources.MenuItem_File;
        }
    }
}
