using System.Collections.Generic;

namespace WinForm.Controls.MainMenu.Models
{
    public class SubCategory
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Item> Items { get; set; }
    }
}
