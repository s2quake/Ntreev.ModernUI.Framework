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

using JSSoft.Library.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JSSoft.ModernUI.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryDefinitionAttribute : Attribute
    {
        public CategoryDefinitionAttribute(string categoryName)
        {
            this.CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        }

        public string CategoryName { get; }

        public static string[] GetCategoryDefinitions(object obj)
        {
            var categoryList = new List<string>() { string.Empty };
            if (obj != null)
            {
                var attrs = Attribute.GetCustomAttributes(obj.GetType(), typeof(CategoryDefinitionAttribute)).Cast<CategoryDefinitionAttribute>();
                foreach (var item in attrs)
                {
                    categoryList.Add(item.CategoryName);
                }
            }
            return categoryList.ToArray();
        }

        public static IEnumerable<T> Order<T>(IEnumerable<T> items, Func<T, string> selector, string[] categories)
        {
            var comparer = new Comparer(categories);
            return items.OrderBy(item => selector(item), comparer);
        }

        #region classes

        class Comparer : IComparer<string>
        {
            private readonly string[] categories;

            public Comparer(string[] categories)
            {
                this.categories = categories;
            }
            public int Compare(string x, string y)
            {
                var x1 = this.categories.IndexOf(x);
                var y1 = this.categories.IndexOf(y);
                return x1.CompareTo(y1);
            }
        }

        #endregion
    }
}
