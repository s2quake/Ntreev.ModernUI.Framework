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

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ntreev.ModernUI.Framework
{
    public abstract class AppBootstrapperDescriptorBase
    {
        public abstract object GetInstance(Type service, string key);

        public abstract IEnumerable<object> GetInstances(Type service);

        public abstract void BuildUp(object instance);

        public abstract Type ModelType { get; }

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            yield break;
        }

        protected virtual IEnumerable<Tuple<Type, object>> GetParts()
        {
            yield return new Tuple<Type, object>(typeof(IWindowManager), AppWindowManager.Current);
            yield return new Tuple<Type, object>(typeof(IAppConfiguration), AppConfiguration.Current);
            yield return new Tuple<Type, object>(typeof(IServiceProvider), AppBootstrapperBase.Current);
            yield return new Tuple<Type, object>(typeof(IBuildUp), AppBootstrapperBase.Current);
            yield break;
        }

        protected abstract void OnInitialize(IEnumerable<Assembly> assemblies, IEnumerable<Tuple<Type, object>> parts);

        protected abstract void OnDispose();

        internal void Initialize()
        {
            var parts = this.GetParts();
            this.OnInitialize(this.Assemblies, parts);
        }

        internal void Dispose() => this.OnDispose();

        internal Assembly[] Assemblies => this.GetAssemblies().ToArray();
    }
}
