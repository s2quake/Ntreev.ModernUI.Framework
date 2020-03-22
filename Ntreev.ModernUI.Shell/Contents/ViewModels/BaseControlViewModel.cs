using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class BaseControlViewModel : ContentBase, IContent
    {
        private string settingsName;
        private string address;
        private string userID;

        public BaseControlViewModel()
        {
            this.DisplayName = "BaseControl";
        }

        public string SettingsName
        {
            get => this.settingsName ?? string.Empty;
            set
            {
                this.settingsName = value;
                this.NotifyOfPropertyChange(nameof(SettingsName));
            }
        }

        public string Address
        {
            get => this.address ?? string.Empty;
            set
            {
                this.address = value;
                this.NotifyOfPropertyChange(nameof(Address));
            }
        }

        public string UserID
        {
            get => this.userID ?? string.Empty;
            set
            {
                this.userID = value;
                this.NotifyOfPropertyChange(nameof(UserID));
            }
        }
    }
}
