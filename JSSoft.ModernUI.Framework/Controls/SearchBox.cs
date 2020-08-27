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
using System.Windows.Data;
using System.Windows.Input;

namespace JSSoft.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_EditableTextBox, Type = typeof(TextBox))]
    public class SearchBox : Control
    {
        public const string PART_EditableTextBox = nameof(PART_EditableTextBox);

        public static RoutedCommand ShowCommand = new RoutedUICommand(Properties.Resources.Command_Show, nameof(ShowCommand), typeof(SearchBox));

        public static RoutedCommand HideCommand = new RoutedUICommand(Properties.Resources.Command_Hide, nameof(HideCommand), typeof(SearchBox));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBox),
                new FrameworkPropertyMetadata(string.Empty));

        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register(nameof(Comment), typeof(string), typeof(SearchBox),
                new FrameworkPropertyMetadata(Properties.Resources.Comment_Filter));

        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(SearchBox));

        public static readonly DependencyProperty NextCommandParameterProperty =
            DependencyProperty.Register(nameof(NextCommandParameter), typeof(object), typeof(SearchBox));

        public static readonly DependencyProperty PrevCommandProperty =
            DependencyProperty.Register(nameof(PrevCommand), typeof(ICommand), typeof(SearchBox));

        public static readonly DependencyProperty PrevCommandParameterProperty =
            DependencyProperty.Register(nameof(PrevCommandParameter), typeof(object), typeof(SearchBox));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(SearchBox),
                new FrameworkPropertyMetadata(HideCommand));

        public static readonly DependencyProperty CloseCommandParameterProperty =
            DependencyProperty.Register(nameof(CloseCommandParameter), typeof(object), typeof(SearchBox));

        private TextBox textBox;
        private BindingExpressionBase bindingExpression;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.textBox = this.Template.FindName(PART_EditableTextBox, this) as TextBox;
            if (this.textBox != null)
            {
                this.bindingExpression = BindingOperations.SetBinding(this.textBox, TextBox.TextProperty, new Binding(SearchBox.TextProperty.Name)
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    Delay = 500,
                });
                this.textBox.KeyDown += TextBox_KeyDown;
            }
        }

        public void SelectAll()
        {
            if (this.textBox != null)
            {
                this.textBox.SelectAll();
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

        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public object NextCommandParameter
        {
            get => (object)GetValue(NextCommandParameterProperty);
            set => SetValue(NextCommandParameterProperty, value);
        }

        public ICommand PrevCommand
        {
            get => (ICommand)GetValue(PrevCommandProperty);
            set => SetValue(PrevCommandProperty, value);
        }

        public object PrevCommandParameter
        {
            get => (object)GetValue(PrevCommandParameterProperty);
            set => SetValue(PrevCommandParameterProperty, value);
        }

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public object CloseCommandParameter
        {
            get => (object)GetValue(CloseCommandParameterProperty);
            set => SetValue(CloseCommandParameterProperty, value);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.textBox != null)
            {
                this.textBox.Focus();
                this.textBox.SelectAll();
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.None)
                        {
                            if (this.bindingExpression != null)
                            {
                                this.bindingExpression.UpdateSource();
                            }

                            if (this.NextCommand != null && this.NextCommand.CanExecute(this.NextCommandParameter) == true)
                            {
                                this.NextCommand.Execute(this.NextCommandParameter);
                            }
                            e.Handled = true;
                        }
                        else if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            if (this.PrevCommand != null && this.PrevCommand.CanExecute(this.PrevCommandParameter) == true)
                            {
                                this.PrevCommand.Execute(PrevCommandParameter);
                            }
                            e.Handled = true;
                        }
                    }
                    break;
                case Key.Escape:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.None)
                        {
                            if (this.CloseCommand != null && this.CloseCommand.CanExecute(this.CloseCommandParameter) == true)
                            {
                                this.CloseCommand.Execute(this.CloseCommandParameter);
                            }
                            e.Handled = true;
                        }
                        break;
                    }
            }
        }
    }
}
