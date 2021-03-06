﻿// Released under the MIT License.
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

using JSSoft.Library;
using JSSoft.Library.IO;
using System;
using System.IO;

namespace JSSoft.ModernUI.Framework
{
    sealed class AppConfiguration : ConfigurationBase, IAppConfiguration
    {
        private readonly string filename;
        private readonly IConfigurationSerializer serializer = new ConfigurationSerializer();

        internal AppConfiguration()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var productName = AppInfo.ProductName;
            this.filename = Path.Combine(path, productName, "app.config");
            try
            {
                if (File.Exists(this.filename) == true)
                {
                    using var stream = File.OpenRead(filename);
                    this.Read(stream, this.serializer);
                }
            }
            catch
            {

            }
        }

        public void Write()
        {
            try
            {
                FileUtility.Prepare(filename);
                using var stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                this.Write(stream, this.serializer);
            }
            catch
            {

            }
        }

        public override string Name => "AppConfigs";

        public static AppConfiguration Current { get; } = new AppConfiguration();
    }
}
