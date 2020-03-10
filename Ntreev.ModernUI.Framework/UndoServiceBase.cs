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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Framework
{
    public abstract class UndoServiceBase : Caliburn.Micro.PropertyChangedBase, IUndoService
    {
        private readonly ObservableCollection<IUndo> undoItems = new ObservableCollection<IUndo>();
        private readonly ObservableCollection<IUndo> redoItems = new ObservableCollection<IUndo>();
        private readonly ReadOnlyObservableCollection<IUndo> undoItemsReadOnly;
        private readonly ReadOnlyObservableCollection<IUndo> redoItemsReadOnly;
        private BatchAction transaction;

        public UndoServiceBase()
        {
            this.undoItemsReadOnly = new ReadOnlyObservableCollection<IUndo>(this.undoItems);
            this.redoItemsReadOnly = new ReadOnlyObservableCollection<IUndo>(this.redoItems);
        }

        public bool CanUndo
        {
            get
            {
                if (this.transaction != null)
                    return false;
                return this.undoItems.Any();
            }
        }

        public bool CanRedo
        {
            get { return this.redoItems.Any(); }
        }

        public IEnumerable<IUndo> UndoItems => this.undoItemsReadOnly;

        public IEnumerable<IUndo> RedoItems => this.redoItemsReadOnly;

        public void Execute(IUndo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            item.Redo();
            this.Push(item);
        }

        public void Push(IUndo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (this.transaction != null)
            {
                this.transaction.Items.Push(item);
            }
            else
            {
                this.undoItems.Insert(0, item);
                this.redoItems.Clear();
                this.OnChanged(EventArgs.Empty);
                this.NotifyOfPropertyChange(nameof(this.CanUndo));
                this.NotifyOfPropertyChange(nameof(this.CanRedo));
            }
        }

        public void Undo()
        {
            this.ValidateUndo();
            this.UndoInternal();
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        public void Redo()
        {
            this.ValidateRedo();
            this.RedoInternal();
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        public void Undo(IUndo item)
        {
            this.ValidateUndo(item);

            while (this.undoItems.Any())
            {
                if (this.UndoInternal() == item)
                    break;
            }
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        public void Redo(IUndo item)
        {
            this.ValidateRedo(item);

            while (this.redoItems.Any())
            {
                if (this.RedoInternal() == item)
                    break;
            }
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        public void Clear()
        {
            this.undoItems.Clear();
            this.redoItems.Clear();
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        public IUndoTransaction BeginTransaction(string name)
        {
            try
            {
                this.ValidateBeginTransaction(name);
                this.transaction = new BatchAction(this, name);
                return this.transaction;
            }
            finally
            {
                this.NotifyOfPropertyChange(nameof(this.CanUndo));
                this.NotifyOfPropertyChange(nameof(this.CanRedo));
            }
        }

        public event EventHandler Changed;

        protected virtual void OnChanged(EventArgs e)
        {
            this.Changed?.Invoke(this, e);
        }

        private void ValidateBeginTransaction(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (this.transaction != null)
                throw new InvalidOperationException();
        }

        private void ValidateUndo(IUndo item)
        {
            if (this.transaction != null)
                throw new InvalidOperationException();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (this.undoItems.Contains(item) == false)
                throw new ArgumentException(nameof(item));
        }

        private void ValidateUndo()
        {
            if (this.transaction != null)
                throw new InvalidOperationException();
        }

        private void ValidateRedo()
        {

        }

        private void ValidateRedo(IUndo item)
        {
            if (this.transaction != null)
                throw new InvalidOperationException();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (this.redoItems.Contains(item) == false)
                throw new ArgumentException(nameof(item));
        }

        private void Commit()
        {
            if (this.transaction.Items.Any())
                this.undoItems.Insert(0, this.transaction);
            this.redoItems.Clear();
            this.transaction = null;
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        private void Rollback()
        {
            this.transaction.Undo();
            this.transaction = null;
            this.OnChanged(EventArgs.Empty);
            this.NotifyOfPropertyChange(nameof(this.CanUndo));
            this.NotifyOfPropertyChange(nameof(this.CanRedo));
        }

        private IUndo UndoInternal()
        {
            var item = this.undoItems.First();
            item.Undo();
            this.undoItems.RemoveAt(0);
            this.redoItems.Insert(0, item);
            return item;
        }

        private IUndo RedoInternal()
        {
            var item = this.redoItems.First();
            item.Redo();
            this.redoItems.RemoveAt(0);
            this.undoItems.Insert(0, item);
            return item;
        }

        #region classes

        class BatchAction : UndoBase, IUndoTransaction
        {
            private UndoServiceBase undoService;
            private readonly string name;

            public BatchAction(UndoServiceBase undoService, string name)
            {
                this.undoService = undoService;
                this.name = name;
            }

            public override string ToString()
            {
                return this.name;
            }

            public void Commit()
            {
                if (this.undoService == null)
                    throw new InvalidOperationException();
                this.undoService.Commit();
                this.undoService = null;
            }

            public void Rollback()
            {
                if (this.undoService == null)
                    throw new InvalidOperationException();
                this.undoService.Rollback();
                this.undoService = null;
            }

            public Stack<IUndo> Items { get; } = new Stack<IUndo>();

            protected override void OnRedo()
            {
                foreach (var item in this.Items.Reverse())
                {
                    item.Redo();
                }
            }

            protected override void OnUndo()
            {
                foreach (var item in this.Items)
                {
                    item.Undo();
                }
            }
        }

        #endregion
    }
}
