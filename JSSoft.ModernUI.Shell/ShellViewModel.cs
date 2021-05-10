﻿// Released under the MIT License.
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

using JSSoft.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace JSSoft.ModernUI.Shell
{
    [Export(typeof(IShell))]
    class ShellViewModel : ScreenBase, IShell
    {
        private readonly ObservableCollection<IContent> contents = new();
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
