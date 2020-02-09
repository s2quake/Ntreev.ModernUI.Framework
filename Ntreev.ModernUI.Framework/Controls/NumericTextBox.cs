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
    public class NumericTextBox : TextBox
    {
        public static readonly DependencyProperty NumericTypeProperty =
            DependencyProperty.Register(nameof(NumericType), typeof(Type), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(typeof(double)), NumericTypePropertyValidateValueCallback);

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericTextBox),
                new FrameworkPropertyMetadata(ValuePropertyChangedCallback));

        private bool isUpdating;

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

        protected override void OnInitialized(EventArgs e)
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
            }
        }

        private void UpdateText()
        {
            if (this.isUpdating == false)
            {
                this.isUpdating = true;
                this.Text = $"{this.Value}";
                this.isUpdating = false;
            }
        }
    }
}
