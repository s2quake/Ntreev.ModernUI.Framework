using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = nameof(PART_TextBox), Type = typeof(TextBox))]
    public class NumericTextBox : Control
    {
        public const string PART_TextBox = nameof(PART_TextBox);
        public static readonly DependencyProperty NumericTypeProperty =
            DependencyProperty.Register(nameof(NumericType), typeof(NumericType), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(NumericType.Int32, NumericTypePropertyChangedCallback));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(ValuePropertyChangedCallback, ValuePropertyCoerceValueCallback));

        private static readonly Dictionary<NumericType, Func<string, decimal?>> parserByType = new Dictionary<NumericType, Func<string, decimal?>>()
        {
            { NumericType.Int8, (text) => { if (sbyte.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.UInt8, (text) => { if (byte.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.Int16, (text) => { if (short.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.UInt16, (text) => { if (ushort.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.Int32, (text) => { if (int.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.UInt32, (text) => { if (uint.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.Int64, (text) => { if (long.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.UInt64, (text) => { if (ulong.TryParse(text, out var v) == true) return v; return null; } },
            { NumericType.Single, (text) => { if (float.TryParse(text, out var v) == true) return (decimal)v; return null; } },
            { NumericType.Double, (text) => { if (double.TryParse(text, out var v) == true) return (decimal)v; return null; } },
            { NumericType.Decimal, (text) => { if (decimal.TryParse(text, out var v) == true) return v; return null; } }
        };

        private TextBox textBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template.FindName(PART_TextBox, this) is TextBox textBox)
            {
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_CanExecute));
                textBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_CanExecute));
                textBox.Text = $"{this.Value}";
                this.textBox = textBox;
            }
        }

        public NumericType NumericType
        {
            get => (NumericType)this.GetValue(NumericTypeProperty);
            set => this.SetValue(NumericTypeProperty, value);
        }

        public decimal Value
        {
            get => (decimal)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericTextBox self)
            {
                self.UpdateText();
            }
        }

        private static object ValuePropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (d is NumericTextBox self && baseValue is decimal value)
            {
                var parser = parserByType[self.NumericType];


                return baseValue;
            }
            return decimal.Zero;
        }

        private static void NumericTypePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ValueProperty);
        }

        private void UpdateText()
        {
            var parser = parserByType[this.NumericType];
            var value = parser(this.textBox.Text);
            if (this.Value != value)
            {
                this.textBox.TextChanged -= TextBox_TextChanged;
                this.textBox.Text = $"{this.Value}";
                this.textBox.TextChanged += TextBox_TextChanged;
            }
        }

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = this.textBox.Text;
            var clipboardText = Clipboard.GetText();
            var parser = parserByType[this.NumericType];
            if (this.textBox.SelectionLength > 0)
                text = text.Remove(this.textBox.SelectionStart, this.textBox.SelectionLength);
            text = text.Insert(this.textBox.CaretIndex, clipboardText);
            e.CanExecute = parser(text) is decimal;
            e.Handled = true;
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.textBox.Paste();
        }

        private void Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = this.textBox.Text;
            var clipboardText = Clipboard.GetText();
            var parser = parserByType[this.NumericType];
            if (this.textBox.SelectionLength > 0)
                text = text.Remove(this.textBox.SelectionStart, this.textBox.SelectionLength);
            e.CanExecute = parser(text) is decimal;
            e.Handled = true;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.textBox.Cut();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var text = textBox.Text;
                var parser = parserByType[this.NumericType];
                var value = parser(text).Value;
                if (this.Value != value)
                {
                    this.Value = value;
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Delete)
                {
                    e.Handled = this.ProcessDeleteKey(textBox);

                }
                else if (e.Key == Key.Back)
                {
                    e.Handled = this.ProcessBackspaceKey(textBox);
                }
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox && e.Handled == false)
            {
                var text = textBox.Text;
                if (textBox.SelectionLength > 0)
                    text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                text = text.Insert(textBox.CaretIndex, e.Text);
                var parser = parserByType[this.NumericType];
                if (parser(text) is decimal == false)
                    e.Handled = true;
            }
        }

        private bool ProcessDeleteKey(TextBox textBox)
        {
            var text = textBox.Text;
            var parser = parserByType[this.NumericType];
            if (textBox.SelectionLength > 0)
            {
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            }
            else
            {
                text = text.Remove(textBox.SelectionStart, 1);
            }
            return parser(text) is decimal == false;
        }

        private bool ProcessBackspaceKey(TextBox textBox)
        {
            var text = textBox.Text;
            var parser = parserByType[this.NumericType];
            if (textBox.SelectionLength > 0)
            {
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            }
            else if (textBox.SelectionStart > 0)
            {
                text = text.Remove(textBox.SelectionStart - 1, 1);
            }
            return parser(text) is decimal == false;
        }
    }
}
