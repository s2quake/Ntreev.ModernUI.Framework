using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
<<<<<<< master
    public class NumericTextBox : TextBox
    {
=======
    //[TemplatePart(Name = nameof(PART_TextBox), Type = typeof(TextBox))]
    public class NumericTextBox : TextBox
    {
        //public const string PART_TextBox = nameof(PART_TextBox);
>>>>>>> local
        public static readonly DependencyProperty NumericTypeProperty =
            DependencyProperty.Register(nameof(NumericType), typeof(Type), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(typeof(double)), NumericTypePropertyValidateValueCallback);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(ValuePropertyChangedCallback));

<<<<<<< master
        private bool isUpdating;
=======
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
>>>>>>> local

        public Type NumericType
        {
            get => (Type)this.GetValue(NumericTypeProperty);
            set => this.SetValue(NumericTypeProperty, value);
        }

        public decimal Value
        {
            get => (decimal)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

<<<<<<< master
        protected override void OnInitialized(EventArgs e)
=======
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Delete)
            {
                e.Handled = this.ProcessDeleteKey();

            }
            else if (e.Key == Key.Back)
            {
                e.Handled = this.ProcessBackspaceKey();
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
>>>>>>> local
        {
            base.OnInitialized(e);
            this.Text = $"{0}";
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (this.isUpdating == false)
            {
                this.isUpdating = true;
                if (TryParse(this.NumericType, this.Text, out var v) == true)
                {
                    this.Value = v;
                }
                this.isUpdating = false;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
<<<<<<< master
            base.OnLostFocus(e);
            if (this.Text == string.Empty)
            {
                this.Value = 0;
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (e.Handled == false)
            {
                var text = this.Text.Insert(this.CaretIndex, e.Text);
                if (TryParse(this.NumericType, text, out _) == false)
                    e.Handled = true;
            }
        }

        private static bool TryParse(Type numericType, string text, out decimal result)
        {
            switch (Type.GetTypeCode(numericType))
            {
                case TypeCode.Byte:
                    {
                        if (Byte.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.SByte:
                    {
                        if (SByte.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.UInt16:
                    {
                        if (UInt16.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.UInt32:
                    {
                        if (UInt32.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.UInt64:
                    {
                        if (UInt64.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.Int16:
                    {
                        if (Int16.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.Int32:
                    {
                        if (Int32.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.Int64:
                    {
                        if (Int64.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.Decimal:
                    {
                        if (Decimal.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.Double:
                    {
                        if (Double.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
                case TypeCode.Single:
                    {
                        if (Single.TryParse(text, out var v) == true)
                        {
                            result = (decimal)v;
                            return true;
                        }
                    }
                    break;
            }
            result = 0;
            return false;
        }

        private static bool NumericTypePropertyValidateValueCallback(object value)
        {
            if (value is Type type)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return true;
                    default:
                        return false;
                }
            }
            throw new NotImplementedException();
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericTextBox self)
            {
                self.UpdateText();
=======
            var parser = parserByType[this.NumericType];
            var value = parser(this.Text);
            if (this.Value != value)
            {
                this.TextChanged -= TextBox_TextChanged;
                this.Text = $"{this.Value}";
                this.TextChanged += TextBox_TextChanged;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = this.Text;
            var parser = parserByType[this.NumericType];
            var value = parser(text).Value;
            if (this.Value != value)
            {
                this.Value = value;
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
>>>>>>> local
            }
        }

<<<<<<< master
        private void UpdateText()
        {
            if (this.isUpdating == false)
            {
                this.isUpdating = true;
                this.Text = $"{this.Value}";
                this.isUpdating = false;
=======
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
>>>>>>> local
            }
        }
    }
}
