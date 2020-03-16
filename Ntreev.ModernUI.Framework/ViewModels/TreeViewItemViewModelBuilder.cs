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

using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using Ntreev.Library.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Ntreev.ModernUI.Framework.ViewModels
{
    public class TreeViewItemViewModelBuilder
    {
        public TreeViewItemViewModelBuilder()
        {

        }

        public IDictionary<string, TreeViewItemViewModel> Create(string[] items)
        {
            return this.Create(items, false);
        }

        public IDictionary<string, TreeViewItemViewModel> Create(string[] items, bool categoryOnly)
        {
            var viewModels = new Dictionary<string, TreeViewItemViewModel>();

            foreach (var item in MakeItemList(items))
            {
                if (item == PathUtility.Separator)
                {
                    var viewModel = this.CreateCategory(item);
                    viewModels.Add(item, viewModel);
                }
                else if (NameValidator.VerifyCategoryPath(item) == true)
                {
                    _ = new CategoryName(item);
                    var viewModel = this.CreateCategory(item);
                    var parentPath = this.GetParentPath(item);
                    viewModel.Parent = viewModels[parentPath];
                    viewModels.Add(item, viewModel);
                }
                else if (categoryOnly == false)
                {
                    _ = new ItemName(item);
                    var viewModel = this.CreateItem(item);
                    var parentPath = this.GetParentPath(item);
                    viewModel.Parent = viewModels[parentPath];
                    viewModels.Add(item, viewModel);
                }
            }
            return viewModels;
        }

        public static string[] MakeItemList(string[] items)
        {
            return MakeItemList(items, false);
        }

        public static string[] MakeItemList(string[] items, bool categoryOnly)
        {
            var query = from item in items
                        from parent in QueryParents(item)
                        select parent;

            var result = query.Concat(items)
                              .Distinct()
                              .Where(item => categoryOnly == true ? NameValidator.VerifyCategoryPath(item) : true)
                              .OrderBy(item => item)
                              .ToArray();

            if (result.Any() == true)
                return result;
            return new string[] { PathUtility.Separator, };

            IEnumerable<string> QueryParents(string path)
            {
                return EnumerableUtility.Ancestors(path, item =>
                {
                    if (item == PathUtility.Separator)
                        return null;
                    if (NameValidator.VerifyItemPath(item) == true)
                        return new ItemName(item).CategoryPath;
                    return new CategoryName(item).ParentPath;
                });
            }
        }

        protected virtual TreeViewItemViewModel CreateCategory(string path)
        {
            return new CategoryTreeViewItemViewModel(path);
        }

        protected virtual TreeViewItemViewModel CreateItem(string path)
        {
            return new ItemTreeViewItemViewModel(path);
        }

        protected virtual string GetParentPath(string path)
        {
            if (NameValidator.VerifyItemPath(path) == true)
                return new ItemName(path).CategoryPath;
            return new CategoryName(path).ParentPath;
        }
    }
}
