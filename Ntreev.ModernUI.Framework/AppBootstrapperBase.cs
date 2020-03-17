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
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework
{
    public abstract class AppBootstrapperBase : BootstrapperBase, IServiceProvider
    {
        private readonly Type modelType;
        private CompositionContainer container;

        static AppBootstrapperBase()
        {
            ConventionManager.AddElementConvention<ThicknessControl>(ThicknessControl.ValueProperty, nameof(ThicknessControl.Value), nameof(ThicknessControl.ValueChanged));
            ConventionManager.AddElementConvention<ColorPicker>(ColorPicker.ValueProperty, nameof(ColorPicker.Value), nameof(ColorPicker.ValueChanged));
        }

        protected AppBootstrapperBase(Type modelType)
        {
            if (Current != null)
                throw new InvalidOperationException("AppBootstrapper does not allow multi instance");
            Current = this;
            this.modelType = modelType;
            if (this.AutoInitialize == true)
                this.Initialize();
        }

        public static AppBootstrapperBase Current { get; private set; }

        public static IAppConfiguration Configs => AppConfiguration.Current;

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;

            if (typeof(IEnumerable).IsAssignableFrom(serviceType) && serviceType.GenericTypeArguments.Length == 1)
            {
                var itemType = serviceType.GenericTypeArguments.First();
                var contractName = AttributedModelServices.GetContractName(itemType);
                var items = this.container.GetExportedValues<object>(contractName);
                var listGenericType = typeof(List<>);
                var list = listGenericType.MakeGenericType(itemType);
                var ci = list.GetConstructor(new Type[] { typeof(int) });
                var instance = ci.Invoke(new object[] { items.Count(), }) as IList;
                foreach (var item in items)
                {
                    instance.Add(item);
                }
                return instance;
            }
            else
            {
                var contractName = AttributedModelServices.GetContractName(serviceType);
                return this.container.GetExportedValue<object>(contractName);
            }
        }

        public static void SatisfyImportsOnce(object instance)
        {
            Current.BuildUp(instance);
        }

        protected override void Configure()
        {
            var catalog = new AggregateCatalog();

            foreach (var item in AssemblySource.Instance)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(item));
            }

            var container = new CompositionContainer(catalog);
            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(AppWindowManager.Current);
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<IAppConfiguration>(AppConfiguration.Current);
            batch.AddExportedValue<IServiceProvider>(this);
            batch.AddExportedValue<ICompositionService>(container);

            foreach (var item in this.GetParts())
            {
                var contractName = AttributedModelServices.GetContractName(item.Item1);
                var typeIdentity = AttributedModelServices.GetTypeIdentity(item.Item1);
                batch.AddExport(new Export(contractName, new Dictionary<string, object>
                {
                    {
                        "ExportTypeIdentity",
                        typeIdentity
                    }
                }, () => item.Item2));
            }
            container.Compose(batch);
            this.container = container;

            var defaultLocateForView = ViewModelLocator.LocateForView;
            ViewModelLocator.LocateForView = (view) =>
            {
                object viewModel = defaultLocateForView(view);
                if (viewModel != null)
                    return viewModel;

                if (TypeDescriptor.GetAttributes(view)[typeof(ModelAttribute)] is ModelAttribute attr)
                {
                    if (attr.ModelType.IsInterface == true)
                        return this.GetInstance(attr.ModelType, null);
                    return TypeDescriptor.CreateInstance(null, attr.ModelType, null, null);
                }

                return viewModel;
            };

            var defaultLocateTypeForModelType = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (vmType, location, context) =>
            {
                var attribute = vmType.GetCustomAttributes(typeof(ViewAttribute), true).OfType<ViewAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    return Type.GetType(attribute.ViewTypeName);
                }

                var viewType = defaultLocateTypeForModelType(vmType, location, context);

                if (viewType == null)
                {
                    Type baseType = vmType.BaseType;
                    if (baseType.Name.EndsWith("ViewModel") == true)
                    {
                        return ViewLocator.LocateTypeForModelType(baseType, location, context);
                    }
                }

                return viewType;
            };

            var defaultLocateForModel = ViewLocator.LocateForModel;
            ViewLocator.LocateForModel = (model, displayLocation, context) =>
            {
                //IViewAware viewAware = model as IViewAware;
                //if (viewAware != null)
                //{
                //    UIElement uIElement = viewAware.GetView(context) as UIElement;
                //    if (uIElement != null)
                //    {
                //        Window window = uIElement as Window;
                //        if (window == null || (!window.IsLoaded && !(new System.Windows.Interop.WindowInteropHelper(window).Handle == IntPtr.Zero)))
                //        {
                //            //ViewLocator.Log.Info("Using cached view for {0}.", new object[]
                //            //{
                //            //    model
                //            //});
                //            return uIElement;
                //        }
                //    }
                //}
                var view = defaultLocateForModel(model, displayLocation, context);

                if (view != null && view.GetType().GetCustomAttributes<ExportAttribute>(true).Any() == false)
                    this.container.SatisfyImportsOnce(view);

                return view;
            };
        }

        protected override object GetInstance(Type service, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;
            var exports = this.container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var assemblyList = new List<Assembly>(base.SelectAssemblies())
            {
                Assembly.GetExecutingAssembly()
            };

            var assembliesByName = new Dictionary<string, Assembly>();
            foreach (var item in assemblyList)
            {
                assembliesByName.Add(item.FullName, item);
            }

            if (Execute.InDesignMode == false)
            {
                var query = from directory in EnumerableUtility.Friends(AppDomain.CurrentDomain.BaseDirectory, this.SelectPath())
                            let catalog = new DirectoryCatalog(directory)
                            from file in catalog.LoadedFiles
                            select file;

                foreach (var item in query.Distinct())
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(item);
                        if (assembliesByName.ContainsKey(assembly.FullName) == false)
                        {
                            assembliesByName.Add(assembly.FullName, assembly);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return assembliesByName.Values.ToArray();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.container.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
        }

        protected override void BuildUp(object instance)
        {
            this.container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));

            if (this.IgnoreDisplay == false)
                this.DisplayRootViewForAsync(this.modelType);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            AppConfiguration.Current.Write();
            base.OnExit(sender, e);
            this.container.Dispose();
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
        }

        protected virtual IEnumerable<string> SelectPath()
        {
            yield break;
        }

        protected virtual IEnumerable<Tuple<System.Type, object>> GetParts()
        {
            yield break;
        }

        protected virtual bool IgnoreDisplay
        {
            get { return false; }
        }

        protected virtual bool AutoInitialize
        {
            get { return true; }
        }
    }
}
