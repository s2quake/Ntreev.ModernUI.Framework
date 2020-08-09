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
using System.Diagnostics;
using System.Reflection;

namespace Ntreev.ModernUI.Framework
{
    public class AppInfo
    {
        private static string productName;
        public static string ProductName
        {
            get
            {
                if (productName == null)
                {
                    string text = string.Empty;
                    Assembly assembly = Assembly.GetEntryAssembly();
                    if (assembly == null)
                    {
                        assembly = Assembly.GetCallingAssembly();
                    }
                    AssemblyProductAttribute[] array = (AssemblyProductAttribute[])assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
                    if (array != null && array.Length > 0)
                    {
                        text = array[0].Product;
                    }
                    if ((text == null || text.Length == 0) && assembly.EntryPoint != null)
                    {
                        text = assembly.EntryPoint.DeclaringType.Namespace;
                        if (text != null)
                        {
                            int num = text.LastIndexOf('.');
                            if (num >= 0 && num < text.Length - 1)
                            {
                                text = text.Substring(num + 1);
                            }
                        }
                        if (text == null || text.Length == 0)
                        {
                            text = assembly.EntryPoint.DeclaringType.FullName;
                        }
                    }
                    productName = text;
                }
                return productName;
            }
        }

        private static string compnayName;
        public static string CompanyName
        {
            get
            {
                if (compnayName == null)
                {
                    string text = string.Empty;
                    Assembly assembly = Assembly.GetEntryAssembly();
                    if (assembly == null)
                    {
                        assembly = Assembly.GetCallingAssembly();
                    }
                    AssemblyCompanyAttribute[] array = (AssemblyCompanyAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                    if (array != null && array.Length > 0)
                    {
                        text = array[0].Company;
                    }
                    if ((text == null || text.Length == 0) && assembly.EntryPoint != null)
                    {
                        text = assembly.EntryPoint.DeclaringType.Namespace;
                        if (text != null)
                        {
                            int num = text.LastIndexOf('.');
                            if (num >= 0 && num < text.Length - 1)
                            {
                                text = text.Substring(num + 1);
                            }
                        }
                        if (text == null || text.Length == 0)
                        {
                            text = assembly.EntryPoint.DeclaringType.FullName;
                        }
                    }
                    compnayName = text;
                }
                return compnayName;
            }
        }

        public static string ProductVersion
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        public static string StartupPath => System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static string GetUserAppDataPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            var at = typeof(AssemblyCompanyAttribute);
            var r = assembly.GetCustomAttributes(at, false);
            var ct = (AssemblyCompanyAttribute)(r[0]);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += @"\" + ct.Company;
            path += @"\" + assembly.GetName().Name.ToString();

            return path;
        }
    }
}
