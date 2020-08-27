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

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework
{
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

        public override Task DeactivateItemAsync(T item, bool close, CancellationToken cancellationToken)
        {
            return base.DeactivateItemAsync(item, close, cancellationToken);
        }

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            return base.CanCloseAsync(cancellationToken);
        }

        public IEnumerable<T> Documents => this.Items;

        public T SelectedDocument
        {
            get => this.ActiveItem;
            set => this.ActiveItem = value as T;
        }

        public Dispatcher Dispatcher => Application.Current.Dispatcher;

        public event EventHandler SelectionChanged;

        public event EventHandler Closed;

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

        IEnumerable<IDocument> IDocumentService.Documents => this.Items;

        IDocument IDocumentService.SelectedDocument
        {
            get => this.ActiveItem;
            set => this.ActiveItem = value as T;
        }

        #endregion
    }
}
