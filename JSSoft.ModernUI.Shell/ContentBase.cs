using Ntreev.ModernUI.Framework;

namespace Ntreev.ModernUI.Shell
{
    class ContentBase : ViewModelBase, IContent
    {
        private string displayName;

        public ContentBase()
        {

        }

        public string DisplayName
        {
            get => this.displayName ?? string.Empty;
            set
            {
                this.displayName = value;
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }
    }
}
