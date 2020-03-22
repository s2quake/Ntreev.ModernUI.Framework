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
        private Assembly[] assemblies;
        private Tuple<Type, object>[] parts;

        public abstract Type ModelType { get; }

        protected abstract object GetInstance(Type service, string key);

        protected abstract IEnumerable<object> GetInstances(Type service);

        protected abstract void OnBuildUp(object instance);

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

        internal object Instance(Type service, string key) => this.GetInstance(service, key);

        internal IEnumerable<object> Instances(Type service) => this.GetInstances(service);

        internal void Initialize()
        {
            this.OnInitialize(this.assemblies, this.parts);
        }

        internal void Dispose() => this.OnDispose();

        internal void BuildUp(object instance) => this.OnBuildUp(instance);

        internal IEnumerable<Assembly> SelectAssemblies()
        {
            this.assemblies = this.GetAssemblies().ToArray();
            this.parts = this.GetParts().ToArray();
            return this.assemblies;
        }
    }
}
