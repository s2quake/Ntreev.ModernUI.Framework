// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/Ntreev.ModernUI.Framework
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace JSSoft.ModernUI.Framework.ViewModels
{
    public class CheckableTreeViewItemViewModel : TreeViewItemViewModel
    {
        private bool? isChecked = false;

        public CheckableTreeViewItemViewModel()
        {
            this.Items.CollectionChanged += Items_CollectionChanged;
        }

        public bool? IsChecked
        {
            get
            {
                if (this.IsThreeState == false)
                {
                    return this.isChecked ?? true;
                }
                return this.isChecked;
            }
            set
            {
                if (this.isChecked == (value ?? false))
                    return;

                this.isChecked = value ?? false;
                this.NotifyOfPropertyChange(nameof(this.IsChecked));

                foreach (var item in this.Items.OfType<CheckableTreeViewItemViewModel>())
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }

                foreach (var item in this.Items.OfType<CheckableTreeViewItemViewModel>())
                {
                    if (item.CanCheck == false)
                        continue;
                    item.IsChecked = this.isChecked;
                }

                foreach (var item in this.Items.OfType<CheckableTreeViewItemViewModel>())
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }

                if (this.isChecked == true)
                {
                    this.OnChecked();
                }
                else if (this.isChecked == false)
                {
                    this.OnUnchecked();
                }
                else
                {
                    this.OnIndeterminate();
                }
            }
        }

        public virtual bool IsThreeState => true;

        public virtual bool DependsOnChilds => true;

        public virtual bool DependsOnParent => false;

        public virtual bool CanCheck => true;

        protected virtual void OnChecked()
        {

        }

        protected virtual void OnUnchecked()
        {

        }

        protected virtual void OnIndeterminate()
        {

        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            if (item is INotifyPropertyChanged == true)
                            {
                                (item as INotifyPropertyChanged).PropertyChanged += Item_PropertyChanged;
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (item is INotifyPropertyChanged == true)
                            {
                                (item as INotifyPropertyChanged).PropertyChanged -= Item_PropertyChanged;
                            }
                        }
                    }
                    break;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                var targetViewModel = sender as CheckableTreeViewItemViewModel;
                var total = 0;
                var count = 0;
                foreach (var item in this.Items)
                {
                    if (item is CheckableTreeViewItemViewModel == false)
                        continue;

                    total++;
                    var viewModel = item as CheckableTreeViewItemViewModel;
                    if (viewModel.IsChecked != false)
                        count++;
                }

                if (this.DependsOnChilds == true && total > 0)
                {
                    if (count == 0)
                        this.isChecked = false;
                    else if (count == total)
                        this.isChecked = true;
                    else
                        this.isChecked = null;
                }

                if (targetViewModel.DependsOnParent == true && targetViewModel.isChecked == true)
                {
                    this.isChecked = true;
                }

                this.NotifyOfPropertyChange(nameof(this.IsChecked));
            }
        }
    }
}
