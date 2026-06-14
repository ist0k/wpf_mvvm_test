using System.Text.Json.Serialization;
using WinForm.Controls.MainMenu.ViewModels.Base;

namespace WinForm.Controls.MainMenu.Models
{
    public class Item: BaseViewModel
    {
        private bool _isFavorite;
        public string Code { get; set; }
        public string Name { get; set; }

        public bool IsFavorite
        {
            get => _isFavorite;
            set => SetProperty(ref _isFavorite, value);
        }

        public string ParentSubCategoryName { get; set; }

        [JsonIgnore]
        public SubCategory ParentSubCategory { get; set; }
    }
}
