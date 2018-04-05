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

using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework.Controls
{
    /// <summary>
    /// ExceptionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExceptionWindow : ModernWindow
    {
        public static readonly DependencyProperty MailAddresProperty =
            DependencyProperty.Register(nameof(MailAddress), typeof(string), typeof(ExceptionWindow),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register(nameof(UserName), typeof(string), typeof(ExceptionWindow),
                new PropertyMetadata(string.Empty));

        private readonly Exception exception;
        private bool isSendMail;
        private SecureString secureString = new SecureString();

        public ExceptionWindow(Exception e)
        {
            InitializeComponent();
            this.exception = e;

            this.IsVisibleChanged += ExceptionWindow_IsVisibleChanged;
        }

        public string MailAddress
        {
            get { return (string)this.GetValue(MailAddresProperty); }
            set { this.SetValue(MailAddresProperty, value); }
        }

        public string UserName
        {
            get { return (string)this.GetValue(UserNameProperty); }
            set { this.SetValue(UserNameProperty, value); }
        }

        public string Password
        {
            get;
            set;
        }

        public SecureString SecurePassword
        {
            get { return this.secureString; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.isSendMail == false)
                e.Cancel = true;

            base.OnClosing(e);
        }

        private void ExceptionWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible == true)
            {
                this.SendMail();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SendMail()
        {
            try
            {
                var to = new MailAddress(this.MailAddress);
                var from = new MailAddress("error@ntreev.com", Environment.UserName);
                var client = new SmtpClient("smtp.gmail.com", 25);

                using (var message = new MailMessage(from, to))
                {
                    message.Subject = "[Exception] " + this.AssemblyTitle + " - " + Environment.UserName;
                    message.Body = this.exception.ToString();
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(this.UserName, this.Password);
                    await Task.Run(() => client.Send(message));
                }

                this.isSendMail = true;

                this.CloseButton.Visibility = System.Windows.Visibility.Visible;
                this.ProgressBar.Visibility = System.Windows.Visibility.Hidden;
                this.Message.Content = "전송이 완료되었습니다. 프로그램을 다시 시작해주세요.";
            }
            catch
            {

            }
            finally
            {
                this.CloseButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title.Length > 0)
                        return titleAttribute.Title;
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }
    }
}
