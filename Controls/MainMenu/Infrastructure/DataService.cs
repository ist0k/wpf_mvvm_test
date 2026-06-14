using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WinForm.Controls.MainMenu.Models;

namespace WinForm.Controls.MainMenu.Infrastructure
{
    public class DataService
    {
        private const string MenuFileName = "menu.json";
        private const string FavoritesFileName = "favorites.json";

        public MenuConfiguration LoadMenu()
        {
            if (!File.Exists(MenuFileName)) return null;
            try
            {
                string jsonString = File.ReadAllText(MenuFileName);
                return JsonSerializer.Deserialize<MenuConfiguration>(jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки меню: {ex.Message}");
                return null;
            }
        }

        public List<Item> ApplyFavorites(List<Category> categories)
        {
            var favItems = new List<Item>();

            if (categories == null) return favItems;

            // 1. Читаем сохраненные коды из файла
            if (!File.Exists(FavoritesFileName)) return favItems;

            List<string> savedCodes;
            try
            {
                string jsonString = File.ReadAllText(FavoritesFileName);
                savedCodes = JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
            }
            catch
            {
                return favItems;
            }

            if (savedCodes.Count == 0) return favItems;

            // 2. Бежим по ЧИСТОМУ дереву меню (тут больше нет вкладки Избранного)
            foreach (var cat in categories)
            {
                if (cat.Items != null)
                    BindReferences(cat.Items, savedCodes, favItems);

                if (cat.SubCategories != null)
                {
                    foreach (var subCat in cat.SubCategories)
                    {
                        if (subCat.Items != null)
                            BindReferences(subCat.Items, savedCodes, favItems);
                    }
                }
            }

            return favItems;
        }

        private void BindReferences(List<Item> sourceItems, List<string> savedCodes, List<Item> favItems)
        {
            foreach (var item in sourceItems)
            {
                if (savedCodes.Contains(item.Code))
                {
                    item.IsFavorite = true;
                    // Складываем ссылки в наш результирующий список
                    favItems.Add(item);
                }
            }
        }
        public void SaveFavorites(List<Item> favoriteItems)
        {
            try
            {
                var codesToSave = favoriteItems?
                    .Where(i => !string.IsNullOrEmpty(i.Code))
                    .Select(i => i.Code)
                    .ToList() ?? new List<string>();

                string jsonString = JsonSerializer.Serialize(codesToSave);
                File.WriteAllText(FavoritesFileName, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения избранного: {ex.Message}");
            }
        }
    }
}
