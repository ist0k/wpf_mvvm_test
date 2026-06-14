using System.Collections.Generic;
using System.Linq;
using WinForm.Controls.MainMenu.Models;

namespace WinForm.Controls.MainMenu.Infrastructure
{
    public class SearchService
    {
        public IEnumerable<Item> Search(IEnumerable<Category> categories, string query)
        {
            if (categories == null || string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Item>();

            string lowerQuery = query.ToLower();

            return categories
                .Where(cat => cat.Items != null)
                .SelectMany(cat => cat.Items)
                .Concat(categories
                    .Where(cat => cat.SubCategories != null)
                    .SelectMany(cat => cat.SubCategories)
                    .Where(subCat => subCat.Items != null)
                    .SelectMany(subCat => subCat.Items))
                .Where(item => (item.Code != null && item.Code.ToLower().Contains(lowerQuery)) ||
                               (item.Name != null && item.Name.ToLower().Contains(lowerQuery)))
                .ToList();
        }
    }
}
