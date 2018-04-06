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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class ModernInsertionCell : InsertionCell
    {
        public static readonly DependencyProperty EditingContentProperty =
            DependencyProperty.Register(nameof(EditingContent), typeof(object), typeof(ModernInsertionCell));

        static ModernInsertionCell()
        {
            ContentControl.ContentProperty.OverrideMetadata(typeof(ModernInsertionCell), 
                new FrameworkPropertyMetadata(null, ContentPropertyChangedCallback, ContentPropertyCoerceValueCallback));
        }

        private static void ContentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {


        }

        private static object ContentPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (d.GetValue(Cell.ParentRowProperty) is ModernInsertionRow parentRow)
            {
                if (parentRow.IsBeginEnding == true)
                    return null;
            }
            return baseValue;
        }

        public ModernInsertionCell()
        {

        }

        public object EditingContent
        {
            get { return (object)GetValue(EditingContentProperty); }
            set { SetValue(EditingContentProperty, value); }
        }

        protected override void OnEditBeginning(CancelRoutedEventArgs e)
        {
            base.OnEditBeginning(e);
        }

        protected override void OnEditEnded()
        {
            base.OnEditEnded();
            if (this.ParentRow is ModernInsertionRow parentRow && parentRow.IsBeginEnding == false)
            {
                parentRow.SetField(this.FieldName, this.Content);
            }
        }

        protected override void OnEditEnding(CancelRoutedEventArgs e)
        {
            base.OnEditEnding(e);
            this.Content = this.EditingContent;
        }

        protected override void OnEditCanceled()
        {
            base.OnEditCanceled();
            this.EditingContent = this.Content;
            this.Focus();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void InitializeCore(DataGridContext dataGridContext, Row parentRow, ColumnBase parentColumn)
        {
            base.InitializeCore(dataGridContext, parentRow, parentColumn);
            if (parentRow is ModernInsertionRow insertionRow)
            {
                insertionRow.Inserted += InsertionRow_Inserted;
                insertionRow.EditBegun += InsertionRow_EditBegun;
                insertionRow.Detached += InsertionRow_Detached;
            }
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);
        }

        private async void InsertionRow_EditBegun(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var parentRow = sender as ModernInsertionRow;
                var field = parentRow.GetField(this.FieldName);
                if (field != null)
                {
                    this.Content = null;
                    this.Content = field;
                }
                this.EditingContent = this.Content;
            }, System.Windows.Threading.DispatcherPriority.Render);
        }

        private void InsertionRow_Detached(object sender, EventArgs e)
        {
            this.Content = this.EditingContent;
        }

        private void InsertionRow_Inserted(object sender, EventArgs e)
        {
            this.Content = null;
            this.EditingContent = null;
        }
    }
}
