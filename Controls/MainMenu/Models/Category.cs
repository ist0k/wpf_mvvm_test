using System.Collections.Generic;

namespace WinForm.Controls.MainMenu.Models
{
    public class Category
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<SubCategory> SubCategories { get; set; }
        public List<Item> Items { get; set; }
    }
}
