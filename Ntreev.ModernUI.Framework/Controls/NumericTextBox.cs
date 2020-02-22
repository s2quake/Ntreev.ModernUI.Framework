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
    public class NumericTextBox : TextBox
    {
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

        public NumericTextBox()
        {
            this.TextChanged += TextBox_TextChanged;
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_CanExecute));
            this.Text = $"{this.Value}";
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

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Handled == false)
            {
                if (e.Key == Key.Delete)
                {
                    e.Handled = this.ProcessDeleteKey();

                }
                else if (e.Key == Key.Back)
                {
                    e.Handled = this.ProcessBackspaceKey();
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (e.Handled == false)
            {
                var text = this.Text;
                if (this.SelectionLength > 0)
                    text = text.Remove(this.SelectionStart, this.SelectionLength);
                text = text.Insert(this.CaretIndex, e.Text);
                var parser = parserByType[this.NumericType];
                if (parser(text) is decimal == false)
                    e.Handled = true;
            }
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
            var value = parser(this.Text);
            if (this.Value != value)
            {
                this.TextChanged -= TextBox_TextChanged;
                this.Text = $"{this.Value}";
                this.TextChanged += TextBox_TextChanged;
            }
        }

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = this.Text;
            var clipboardText = Clipboard.GetText();
            var parser = parserByType[this.NumericType];
            if (this.SelectionLength > 0)
                text = text.Remove(this.SelectionStart, this.SelectionLength);
            text = text.Insert(this.CaretIndex, clipboardText);
            e.CanExecute = parser(text) is decimal;
            e.Handled = true;
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Paste();
        }

        private void Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = this.Text;
            var clipboardText = Clipboard.GetText();
            var parser = parserByType[this.NumericType];
            if (this.SelectionLength > 0)
                text = text.Remove(this.SelectionStart, this.SelectionLength);
            e.CanExecute = parser(text) is decimal;
            e.Handled = true;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Cut();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var text = this.Text;
                var parser = parserByType[this.NumericType];
                var value = parser(text).Value;
                if (this.Value != value)
                {
                    this.Value = value;
                }
            }
        }

        private bool ProcessDeleteKey()
        {
            var text = this.Text;
            var parser = parserByType[this.NumericType];
            if (this.SelectionLength > 0)
            {
                text = text.Remove(this.SelectionStart, this.SelectionLength);
            }
            else
            {
                text = text.Remove(this.SelectionStart, 1);
            }
            return parser(text) is decimal == false;
        }

        private bool ProcessBackspaceKey()
        {
            var text = this.Text;
            var parser = parserByType[this.NumericType];
            if (this.SelectionLength > 0)
            {
                text = text.Remove(this.SelectionStart, this.SelectionLength);
            }
            else if (this.SelectionStart > 0)
            {
                text = text.Remove(this.SelectionStart - 1, 1);
            }
            return parser(text) is decimal == false;
        }
    }
}
