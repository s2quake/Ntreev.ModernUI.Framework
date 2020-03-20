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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_EditableTextBox, Type = typeof(TextBox))]
    public class GuidControl : Control
    {
        public const string PART_EditableTextBox = nameof(PART_EditableTextBox);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Guid?), typeof(GuidControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValuePropertychangedCallback));

        public static readonly RoutedCommand NewCommand = new RoutedUICommand(Ntreev.ModernUI.Framework.Properties.Resources.Command_NewGuid, nameof(NewCommand), typeof(GuidControl));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GuidControl));

        private TextBox textBox;

        public GuidControl()
        {
            this.CommandBindings.Add(new CommandBinding(NewCommand, NewCommand_Executed));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.textBox != null)
            {
                this.textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                this.textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
                CommandManager.RemovePreviewExecutedHandler(this.textBox, ExecutedRoutedEventHandler);
            }
            this.textBox = (TextBox)this.Template.FindName(PART_EditableTextBox, this);
            if (this.textBox != null)
            {
                this.textBox.Text = $"{this.Value}";
                this.textBox.PreviewTextInput += TextBox_PreviewTextInput;
                this.textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                CommandManager.AddPreviewExecutedHandler(this.textBox, ExecutedRoutedEventHandler);
            }
        }

        [TypeConverter(typeof(NullableGuidConverter))]
        public Guid? Value
        {
            get => (Guid?)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Value = Guid.NewGuid();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                var text = Clipboard.GetText();
                if (Guid.TryParse(text, out var guid) == false)
                {
                    e.Handled = true;
                    this.Value = guid;
                }
            }
        }

        private static void ValuePropertychangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GuidControl control)
            {
                if (e.NewValue is Guid guid)
                {
                    if (control.textBox != null)
                        control.textBox.Text = $"{guid}";
                }
                else
                {
                    if (control.textBox != null)
                        control.textBox.Text = string.Empty;
                }
                control.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        #region NullableGuidConverter

        class NullableGuidConverter : NullableConverter
        {
            public NullableGuidConverter()
                : base(typeof(Guid))
            {

            }
        }

        #endregion
    }
}
