//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework
{
    [InheritedExport(typeof(IDocumentService))]
    public abstract class DocumentServiceBase<T> : Conductor<T>.Collection.OneActive, IDocumentService where T : class, IDocument
    {
        public DocumentServiceBase()
        {

        }

        public async void Close(bool save)
        {
            var documentList = this.Documents.ToList();
            foreach (var item in documentList.ToArray())
            {
                item.Disposed += (s, e) => documentList.Remove(item);
                await this.Dispatcher.InvokeAsync(() =>
                {
                    if (item.IsModified == true && save == false)
                        item.IsModified = false;
                    item.Dispose();
                });
                await Task.Delay(1);
            }

            while (documentList.Any())
            {
                await Task.Delay(1);
            }
        }

        public override void DeactivateItem(T item, bool close)
        {
            base.DeactivateItem(item, close);
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        public IEnumerable<T> Documents
        {
            get { return this.Items; }
        }

        public T SelectedDocument
        {
            get { return this.ActiveItem; }
            set { this.ActiveItem = value as T; }
        }

        public Dispatcher Dispatcher
        {
            get { return Application.Current.Dispatcher; }
        }

        public event EventHandler SelectionChanged;

        public event EventHandler Closed;

        protected override void ChangeActiveItem(T newItem, bool closePrevious)
        {
            base.ChangeActiveItem(newItem, closePrevious);
        }

        protected override void OnActivationProcessed(T item, bool success)
        {
            base.OnActivationProcessed(item, success);

            this.NotifyOfPropertyChange(nameof(this.SelectedDocument));
            this.OnSelectionChanged(EventArgs.Empty);
        }

        protected virtual void OnClosed(EventArgs e)
        {
            this.Closed?.Invoke(this, e);
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        #region IDocumentService

        IEnumerable<IDocument> IDocumentService.Documents
        {
            get { return this.Items; }
        }

        IDocument IDocumentService.SelectedDocument
        {
            get { return this.ActiveItem; }
            set { this.ActiveItem = value as T; }
        }

        #endregion
    }
}
