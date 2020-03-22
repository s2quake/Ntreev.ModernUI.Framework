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

using Ntreev.Library;
using Ntreev.Library.IO;
using System;
using System.IO;

namespace Ntreev.ModernUI.Framework
{
    sealed class AppConfiguration : ConfigurationBase, IAppConfiguration
    {
        private readonly string filename;
        private IConfigurationSerializer serializer = new ConfigurationSerializer();

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
            //var d = DateTime.Now;
            //this["sbyte"] = default(sbyte);
            //this["byte"] = default(byte);
            //this["short"] = default(short);
            //this["ushort"] = default(ushort);
            //this["int"] = default(int);
            //this["uint"] = default(uint);
            //this["long"] = default(long);
            //this["ulong"] = default(ulong);
            //this["float"] = default(float);
            //this["double"] = default(double);
            //this["decimal"] = default(decimal);
            //this["numberAsString"] = "0";
            //this["dateTime"] = DateTime.Now;
            //Thread.Sleep(100);
            //this["timeSpan"] = DateTime.Now - d;
            //this["wo2w"] = Colors.Red;
            //this["array1"] = new int[] { 1, 2, 3};
            //this["array2"] = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            //this["array3"] = new int[2, 3, 3] 
            //{
            //    { 
            //        { 1, 2, 3 }, 
            //        { 4, 5, 6 },
            //        { 7, 8, 9 }
            //    },
            //    {
            //        { 11, 12, 13 },
            //        { 14, 15, 16 },
            //        { 17, 18, 19 }
            //    }
            //};
            //var i = (int)ConfigurationBase.ConvertFromConfig(this["decimal"], typeof(int));
            //var c = (Color)ConfigurationBase.ConvertFromConfig(this["wo2w"], typeof(Color));

            
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
