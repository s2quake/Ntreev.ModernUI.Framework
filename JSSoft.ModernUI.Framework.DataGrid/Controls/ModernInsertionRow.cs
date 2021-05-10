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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    [TemplatePart(Name = PART_InsertButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_InsertManyButton, Type = typeof(Button))]
    public class ModernInsertionRow : Control
    {
        public const string PART_InsertButton = nameof(PART_InsertButton);
        public const string PART_InsertManyButton = nameof(PART_InsertManyButton);

        public static readonly DependencyProperty InsertCommandProperty =
            DependencyProperty.Register(nameof(InsertCommand), typeof(ICommand), typeof(ModernInsertionRow));

        public static readonly DependencyProperty InsertManyCommandProperty =
            DependencyProperty.Register(nameof(InsertManyCommand), typeof(ICommand), typeof(ModernInsertionRow));

        public static readonly DependencyProperty ColumnManagerRowProperty =
            DependencyProperty.Register(nameof(ColumnManagerRow), typeof(ModernColumnManagerRow), typeof(ModernInsertionRow),
                new UIPropertyMetadata(ColumnManagerRowPropertyChangedCallback));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public ICommand InsertCommand
        {
            get => (ICommand)this.GetValue(InsertCommandProperty);
            set => this.SetValue(InsertCommandProperty, value);
        }

        public ICommand InsertManyCommand
        {
            get => (ICommand)this.GetValue(InsertManyCommandProperty);
            set => this.SetValue(InsertManyCommandProperty, value);
        }

        public ModernColumnManagerRow ColumnManagerRow
        {
            get => (ModernColumnManagerRow)this.GetValue(ColumnManagerRowProperty);
            set => this.SetValue(ColumnManagerRowProperty, value);
        }

        private static void ColumnManagerRowPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
