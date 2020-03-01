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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_EditableTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class FilterBox : Control
    {
        public const string PART_EditableTextBox = nameof(PART_EditableTextBox);
        public const string PART_Popup = nameof(PART_Popup);

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FilterBox),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register(nameof(Comment), typeof(string), typeof(FilterBox),
                new PropertyMetadata(Properties.Resources.Comment_Filter));

        public static readonly DependencyProperty CaseSensitiveProperty =
            DependencyProperty.Register(nameof(CaseSensitive), typeof(bool), typeof(FilterBox));

        public static readonly DependencyProperty GlobPatternProperty =
            DependencyProperty.Register(nameof(GlobPattern), typeof(bool), typeof(FilterBox));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(FilterBox),
                new UIPropertyMetadata(false, IsDropDownOpenPropertyChangedCallback));

        private Popup popup;
        private TextBox textBox;
        private BindingExpressionBase bindingExpression;

        static FilterBox()
        {
            EventManager.RegisterClassHandler(typeof(FilterBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(Mouse_MouseDownHandler), true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.popup != null)
            {
                this.popup.Closed -= Popup_Closed;
            }

            this.popup = this.Template.FindName(PART_Popup, this) as Popup;
            this.textBox = this.Template.FindName(PART_EditableTextBox, this) as TextBox;
            if (this.textBox != null)
            {
                this.bindingExpression = BindingOperations.SetBinding(this.textBox, TextBox.TextProperty, new Binding(nameof(this.Text))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Delay = 500,
                });
                this.textBox.KeyDown += TextBox_KeyDown;
                this.textBox.GotFocus += TextBox_GotFocus;
            }

            if (this.popup != null)
            {
                this.popup.Closed += Popup_Closed;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsDropDownOpen == true)
            {
                this.IsDropDownOpen = false;
            }
        }

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        public string Comment
        {
            get => (string)this.GetValue(CommentProperty);
            set => this.SetValue(CommentProperty, value);
        }

        public bool CaseSensitive
        {
            get => (bool)this.GetValue(CaseSensitiveProperty);
            set => this.SetValue(CaseSensitiveProperty, value);
        }

        public bool GlobPattern
        {
            get => (bool)this.GetValue(GlobPatternProperty);
            set => this.SetValue(GlobPatternProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public event EventHandler DropDownClosed;

        public event EventHandler DropDownOpened;

        protected virtual void OnDropDownClosed(EventArgs e)
        {
            this.DropDownClosed?.Invoke(this, e);
        }

        protected virtual void OnDropDownOpened(EventArgs e)
        {
            this.DropDownOpened?.Invoke(this, e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (this.IsDropDownOpen == true)
                    this.IsDropDownOpen = false;
            }
            else if (e.Key == Key.F2)
            {
                if (this.IsDropDownOpen == false)
                    this.IsDropDownOpen = true;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (this.popup == null)
                return;

            if (Mouse.Captured == this && e.OriginalSource == this && this.popup.IsMouseOver == false)
            {
                if (this.IsDropDownOpen == true)
                    this.IsDropDownOpen = false;
            }
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if (this.IsDropDownOpen == true)
                this.IsDropDownOpen = false;
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.None)
                        {
                            this.Text = string.Empty;
                            if (this.bindingExpression != null)
                            {
                                this.bindingExpression.UpdateSource();
                            }
                        }
                    }
                    break;
                case Key.Enter:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.None)
                        {
                            if (this.bindingExpression != null)
                            {
                                this.bindingExpression.UpdateSource();
                            }
                        }
                    }
                    break;
            }
        }

        private static void IsDropDownOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var filterBox = d as FilterBox;
            if ((bool)e.NewValue == true)
            {
                Mouse.Capture(filterBox, CaptureMode.SubTree);
                filterBox.OnDropDownOpened(EventArgs.Empty);
            }
            else
            {
                if (Mouse.Captured == filterBox)
                {
                    Mouse.Capture(null);
                }
            }
        }

        private static void Mouse_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            var filterBox = sender as FilterBox;

            if (filterBox.IsDropDownOpen == true && (filterBox.popup.IsMouseOver == false && filterBox.IsMouseOver == false))
            {
                filterBox.IsDropDownOpen = false;
            }
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            this.OnDropDownClosed(e);
        }
    }
}