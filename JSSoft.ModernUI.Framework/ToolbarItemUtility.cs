// Released under the MIT License.
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

using JSSoft.Library.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JSSoft.ModernUI.Framework
{
    public static class ToolBarItemUtility
    {
        public static IEnumerable<IToolBarItem> GetToolBarItems(object parent, IEnumerable<IToolBarItem> toolbarItems)
        {
            if (toolbarItems is null)
                throw new ArgumentNullException(nameof(toolbarItems));
            return toolbarItems.Where(item => Predicate(item, parent)).OrderByAttribute().TopologicalSort();
        }

        public static IEnumerable<IToolBarItem> GetToolBarItems(object parent, IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
                throw new ArgumentNullException(nameof(serviceProvider));
            var items = serviceProvider.GetService(typeof(IEnumerable<IToolBarItem>)) as IEnumerable<IToolBarItem>;
            return GetToolBarItems(parent, items);
        }

        private static bool Predicate<IToolbarItem>(IToolbarItem item, object parent)
        {
            var parentType = parent is Type ? parent as Type : parent.GetType();
            var attrs = item.GetType().GetCustomAttributes(typeof(ParentTypeAttribute), true);

            foreach (var attr in attrs)
            {
                var parentTypeAttr = attr as ParentTypeAttribute;

                if (parentTypeAttr.ParentType.IsAssignableFrom(parentType) == true)
                    return true;

                if (parentTypeAttr.ParentType == parentType)
                    return true;
            }

            return false;
        }
    }
}
