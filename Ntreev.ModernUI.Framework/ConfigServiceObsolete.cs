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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Reflection;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Ntreev.ModernUI.Framework
{
    [Obsolete]
    class ConfigServiceObsolete : Dictionary<string, object>
    {
        private const string tagConfigs = "Configs";
        private const string tagConfig = "Config";
        private const string attrName = "name";
        private const string attrAssembly = "assembly";
        private const string attrTypename = "typename";
        private const string attrVersion = "version";

        private static readonly int version = 1;

        private bool upgraded;
        private string path;
        private string backupPath;

        public ConfigServiceObsolete()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string productName = AppInfo.ProductName;
            this.path = Path.Combine(path, productName, "config.xml");
            this.backupPath = Path.Combine(path, productName, "config.xml.bak");
        }

        public void Update()
        {
            if (File.Exists(this.path) == false)
                return;

            try
            {
                string contents = File.ReadAllText(this.path);

                using (StringReader sr = new StringReader(contents))
                using (XmlReader reader = XmlReader.Create(sr))
                {
                    this.Read(reader);
                }
            }
            catch
            {

            }
        }

        public void Commit()
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
            };

            using (StringWriter sw = new InternalStringWriter())
            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                this.Write(writer);
                
                this.PrepareDirectory();

                writer.Close();
                string contents = sw.ToString();
                if (this.upgraded == true)
                {
                    File.Copy(this.path, this.backupPath, true);
                    this.upgraded = false;
                }
                File.WriteAllText(this.path, contents, Encoding.UTF8);
            }
        }

        public string ConfigPath
        {
            get { return this.path; }
            set
            {
                this.path = value;
            }
        }

        private static Type GetType(string typeName, string assemblyName)
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.FullName == assemblyName)
                {
                    return item.GetType(typeName);
                }
            }
            return null;
        }

        private void Write(XmlWriter writer)
        {
            writer.WriteStartElement(tagConfigs);
            writer.WriteAttributeString(attrVersion, version.ToString());

            foreach (var item in this)
            {
                object value = item.Value;

                if (value == null)
                    continue;

                writer.WriteStartElement(tagConfig);
                writer.WriteAttributeString(attrName, item.Key);
                writer.WriteAttributeString(attrTypename, string.Format("{0}", value.GetType().FullName));
                writer.WriteAttributeString(attrAssembly, string.Format("{0}", value.GetType().Assembly.FullName));
                this.WriteConfig(writer, value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void Read(XmlReader reader)
        {
            reader.MoveToContent();

            reader.ReadStartElement(tagConfigs);

            while (reader.MoveToContent() == XmlNodeType.Element && reader.Name == tagConfig)
            {
                string key = reader.GetAttribute(attrName);
                string typeName = reader.GetAttribute(attrTypename);
                string assemblyName = reader.GetAttribute(attrAssembly);

                Type type = GetType(typeName, assemblyName);

                this.ReadConfig(key, type, reader.ReadInnerXml());
            }

            if (reader.NodeType == XmlNodeType.EndElement)
                reader.ReadEndElement();
        }

        private void WriteConfig(XmlWriter parentWriter, object value)
        {
            if (value is IXmlSerializable == true)
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Indent = true,
                };
                XmlWriter writer = XmlWriter.Create(parentWriter, settings);
                IXmlSerializable serializer = value as IXmlSerializable;
                serializer.WriteXml(writer);
            }
            else
            {
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Indent = true,
                };
                XmlWriter writer = XmlWriter.Create(parentWriter, settings);
                var serializer = XmlSerializer.FromTypes(new Type[] { value.GetType(), })[0];
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);
                serializer.Serialize(writer, value, ns);
            }
        }

        private void ReadConfig(string key, Type type, string configXml)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                IgnoreProcessingInstructions = false,
            };
            using (StringReader sr = new StringReader(configXml))
            using (XmlReader reader = XmlReader.Create(sr, settings))
            {
                reader.MoveToContent();
                if (typeof(IXmlSerializable).IsAssignableFrom(type) == true)
                {
                    IXmlSerializable serializer = null;

                    if (this.ContainsKey(key) == true)
                    {
                        serializer = this[key] as IXmlSerializable;
                        serializer.ReadXml(reader);
                    }
                    else
                    {
                        serializer = Activator.CreateInstance(type) as IXmlSerializable;
                        serializer.ReadXml(reader);
                        this.Add(key, serializer);
                    }
                }
                else
                {
                    if (this.ContainsKey(key) == true)
                    {
                        var serializer = XmlSerializer.FromTypes(new Type[] { type, })[0];
                        this[key] = serializer.Deserialize(reader);
                    }
                    else
                    {
                        var serializer = XmlSerializer.FromTypes(new Type[] { type, })[0];
                        this.Add(key, serializer.Deserialize(reader));
                    }
                }
            }
        }

        private void PrepareDirectory()
        {
            string directory = Path.GetDirectoryName(this.path);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
        }

        class InternalStringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}
