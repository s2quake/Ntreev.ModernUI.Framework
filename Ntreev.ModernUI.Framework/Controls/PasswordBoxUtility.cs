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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class PasswordBoxUtility : DependencyObject
    {
        private const string ValidationTarget = nameof(ValidationTarget);
        private const string IsValid = nameof(IsValid);

        public static readonly DependencyProperty ValidationTargetProperty
            = DependencyProperty.RegisterAttached(ValidationTarget, typeof(PasswordBox), typeof(PasswordBoxUtility),
                new UIPropertyMetadata(ValidationTargetPropertyChangedCallback));

        private static readonly DependencyPropertyKey IsValidPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly(IsValid, typeof(bool), typeof(PasswordBoxUtility), new UIPropertyMetadata(true));

        public static readonly DependencyProperty IsValidProperty = IsValidPropertyKey.DependencyProperty;

        private static readonly Dictionary<PasswordBox, PasswordBox> targetToSource = new Dictionary<PasswordBox, PasswordBox>();

        public static PasswordBox GetValidationTarget(PasswordBox d)
        {
            return (PasswordBox)d.GetValue(ValidationTargetProperty);
        }

        public static void SetValidationTarget(PasswordBox d, PasswordBox value)
        {
            d.SetValue(ValidationTargetProperty, value);
        }

        public static bool GetIsValid(PasswordBox d)
        {
            return (bool)d.GetValue(IsValidProperty);
        }

        private static void SetIsValid(PasswordBox d, bool value)
        {
            d.SetValue(IsValidPropertyKey, value);
        }

        private static void ValidationTargetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;

            if (e.OldValue is PasswordBox oldBox)
            {
                passwordBox.PasswordChanged -= ValidationTarget_PasswordChanged;
                targetToSource.Remove(oldBox);
            }

            if (e.NewValue is PasswordBox newBox)
            {
                newBox.PasswordChanged += ValidationTarget_PasswordChanged;
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                targetToSource[newBox] = passwordBox;
            }
            else
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var validationTarget = GetValidationTarget(passwordBox);

            RefreshIsValidProperty(passwordBox, validationTarget);
        }

        private static void ValidationTarget_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var validationTarget = sender as PasswordBox;
            var passwordBox = targetToSource[validationTarget];
            RefreshIsValidProperty(passwordBox, validationTarget);
        }

        private static void RefreshIsValidProperty(PasswordBox passwordBox, PasswordBox validationTarget)
        {
            if (passwordBox.Password != validationTarget.Password || passwordBox.Password == string.Empty)
            {
                SetIsValid(passwordBox, false);
                SetIsValid(validationTarget, false);
            }
            else
            {
                SetIsValid(passwordBox, true);
                SetIsValid(validationTarget, true);
            }
        }
    }
}
