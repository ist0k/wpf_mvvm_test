using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WinForm.Controls.MainMenu.Infrastructure;
using WinForm.Controls.MainMenu.Models;
using WinForm.Controls.MainMenu.ViewModels.Base;

namespace WinForm.Controls.MainMenu.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly DataService _dataService;
        private readonly SearchService _searchService;

        private string _searchQuery;
        private List<Item> _favoriteItems = new List<Item>();
        private bool _isFavoriteTabSelected;
        private bool _isPopupOpen;
        private Category _selectedCategory;
        private SubCategory _selectedSubCategory;
        private List<SubCategory> _subCategories = new List<SubCategory>();
        private Visibility _leftMenuVisibility = Visibility.Visible;

        public event Action<Item> CardClicked;

        public MainMenuViewModel()
        {
            _dataService = new DataService();
            _searchService = new SearchService();

            CodeClickCommand = new RelayCommand(ExecuteCodeClick);
            NavigateToSubCategoryCommand = new RelayCommand(ExecuteNavigateToSubCategory);
            CardClickCommand = new RelayCommand(ExecuteCardClick);
            ToggleFavoriteCommand = new RelayCommand(ExecuteToggleFavorite);

            InitializeMenu();
        }

        public Visibility LeftMenuVisibility
        {
            get => _leftMenuVisibility;
            set => SetProperty(ref _leftMenuVisibility, value);
        }

        public bool IsFavoriteTabSelected
        {
            get => _isFavoriteTabSelected;
            set
            {
                if (SetProperty(ref _isFavoriteTabSelected, value) && value)
                {
                    _selectedCategory = null;
                    OnPropertyChanged(nameof(SelectedCategory));

                    UpdateSubCategories();
                }
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    FilterSearchResults();
                }
            }
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    _isFavoriteTabSelected = false;
                    OnPropertyChanged(nameof(IsFavoriteTabSelected));

                    UpdateSubCategories();
                }
            }
        }

        public SubCategory SelectedSubCategory
        {
            get => _selectedSubCategory;
            set
            {
                if (SetProperty(ref _selectedSubCategory, value))
                {
                    UpdateItems();
                }
            }
        }

        public List<SubCategory> SubCategories
        {
            get => _subCategories;
            private set => SetProperty(ref _subCategories, value);
        }

        public List<Category> Categories { get; private set; } = new List<Category>();
        public ObservableCollection<Item> CurrentItems { get; } = new ObservableCollection<Item>();
        public ObservableCollection<Item> FilteredItems { get; } = new ObservableCollection<Item>();

        public ICommand CodeClickCommand { get; }
        public ICommand NavigateToSubCategoryCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand CardClickCommand { get; }

        private void ExecuteToggleFavorite(object param)
        {
            if (!(param is Item item)) return;

            item.IsFavorite = !item.IsFavorite;

            if (item.IsFavorite)
            {
                if (!_favoriteItems.Contains(item))
                {
                    _favoriteItems.Add(item);

                    if (IsFavoriteTabSelected)
                        CurrentItems.Add(item);
                }
            }
            else
            {
                if (_favoriteItems.Remove(item))
                {
                    if (IsFavoriteTabSelected)
                        CurrentItems.Remove(item);
                }
            }
        }

        private void ExecuteCardClick(object param)
        {
            if (param is Item clickedItem)
            {
                CardClicked?.Invoke(clickedItem);
            }

            SearchQuery = string.Empty;
            IsPopupOpen = false;
        }

        private void ExecuteCodeClick(object param)
        {
            if (param is Item item)
                MessageBox.Show($"Кликнули по коду: {item.Code}", "Действие 1");

            SearchQuery = string.Empty;
            IsPopupOpen = false;
        }

        private void ExecuteNavigateToSubCategory(object param)
        {
            {
                if (!(param is SubCategory targetSubCategory)) return;

                // 1. Ищем, в какой "большой" категории лежит эта подкатегория
                var targetCategory = Categories.FirstOrDefault(c => c.SubCategories != null && c.SubCategories.Contains(targetSubCategory));

                if (targetCategory != null)
                {
                    // 2. Закрываем поисковый попап и очищаем строку поиска
                    SearchQuery = string.Empty;
                    IsPopupOpen = false;

                    // 3. А теперь магия: сначала выбираем большую категорию
                    SelectedCategory = targetCategory;

                    // 4. Сеттер SelectedCategory автоматом выберет ПЕРВУЮ подкатегорию, 
                    // поэтому мы ПРИНУДИТЕЛЬНО перебиваем её на ту, по которой кликнули в поиске!
                    SelectedSubCategory = targetSubCategory;
                }
            }
        }

        private void FilterSearchResults()
        {
            FilteredItems.Clear();

            var matched = _searchService.Search(Categories, SearchQuery);
            foreach (var item in matched)
            {
                FilteredItems.Add(item);
            }

            IsPopupOpen = FilteredItems.Count > 0;
        }

        private void InitializeMenu()
        {
            var menuConfig = _dataService.LoadMenu();
            if (menuConfig?.Categories == null) return;

            Categories = menuConfig.Categories;

            foreach (var cat in Categories)
            {
                if (cat.SubCategories == null) continue;

                foreach (var subCat in cat.SubCategories)
                {
                    if (subCat.Items == null) continue;

                    foreach (var item in subCat.Items)
                    {
                        item.ParentSubCategoryName = subCat.Name;
                        item.ParentSubCategory = subCat;
                    }
                }
            }

            _favoriteItems = _dataService.ApplyFavorites(Categories);

            SelectedCategory = Categories.FirstOrDefault();
        }

        private void UpdateItems()
        {
            CurrentItems.Clear();
            List<Item> sourceList = IsFavoriteTabSelected ? _favoriteItems : SelectedSubCategory?.Items;

            if (sourceList != null)
            {
                foreach (var item in sourceList)
                {
                    CurrentItems.Add(item);
                }
            }
        }

        private void UpdateSubCategories()
        {
            SubCategories = SelectedCategory?.SubCategories ?? new List<SubCategory>();
            SelectedSubCategory = SubCategories.FirstOrDefault();
            LeftMenuVisibility = SubCategories.Any() ? Visibility.Visible : Visibility.Collapsed;

            if (SelectedCategory == null)
            {
                UpdateItems();
            }
        }

        public void SaveData()
        {
            _dataService.SaveFavorites(_favoriteItems);
        }
    }
}