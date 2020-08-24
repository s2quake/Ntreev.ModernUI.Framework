using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Ntreev.ModernUI.Shell
{
    [Export(typeof(IShell))]
    class ShellViewModel : ScreenBase, IShell
    {
        private readonly ObservableCollection<IContent> contents = new ObservableCollection<IContent>();
        private IContent selectedContent;

        [ImportingConstructor]
        public ShellViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "Controls";
            this.Dispatcher.InvokeAsync(() =>
            {
                if (this.ServiceProvider.GetService(typeof(IEnumerable<IContent>)) is IEnumerable<IContent> contents)
                {
                    foreach (var item in contents)
                    {
                        this.contents.Add(item);
                    }
                    this.SelectedContent = this.contents.FirstOrDefault();
                }
            });
        }

        public IEnumerable<IContent> Contents => this.contents;

        public IContent SelectedContent
        {
            get => this.selectedContent;
            set
            {
                this.selectedContent = value;
                this.NotifyOfPropertyChange(nameof(SelectedContent));
            }
        }
    }
}
