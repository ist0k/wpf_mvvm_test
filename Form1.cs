using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WinForm.Controls.MainMenu.Models;
using WinForm.Controls.MainMenu.ViewModels;
using WinForm.Controls.MainMenu.Views;

namespace WinForm
{
    public partial class Form1 : Form
    {
        private MainMenuViewModel _viewModel;
        public Form1()
        {
            InitializeComponent();

            InitializeWpfMenu();

            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Используем безопасный вызов ?. на случай, если при инициализации что-то пошло не так
                // Наша вью-модель сама скажет сервису скинуть коды в favorites.json
                _viewModel?.SaveData();
            }
            catch (Exception ex)
            {
                // На всякий случай ловим ошибки, чтобы приложение не упало некрасиво при выходе
                System.Diagnostics.Debug.WriteLine($"Не удалось сохранить избранное: {ex.Message}");
            }
        }

        private void OnWpfCardClicked(Item item)
        {
            MessageBox.Show($"Форма поймала клик по плитке: {item.Code}");
        }

        private void InitializeWpfMenu()
        {
            try
            {
                // 1. Создаем "мост" между WinForms и WPF
                ElementHost host = new ElementHost();

                // Растягиваем контейнер на всю доступную область формы
                host.Dock = DockStyle.Fill;

                // 2. Создаем экземпляр нашего корневого WPF-контрола
                MainMenuUC wpfMenu = new MainMenuUC();

                if (wpfMenu.DataContext is MainMenuViewModel wpfViewModel)
                {
                    // СОХРАНЯЕМ ссылку во внешнее поле класса
                    _viewModel = wpfViewModel;

                    // 3. ПОДПИСЫВАЕМСЯ на события прямо тут!
                    _viewModel.CardClicked += OnWpfCardClicked;

                }

                // 3. Сажаем WPF-контрол внутрь WinForms-хоста
                host.Child = wpfMenu;

                // 4. Добавляем этот хост на саму WinForms форму
                this.Controls.Add(host);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации WPF-меню: {ex.Message}\n{ex.StackTrace}",
                                "Критическая ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}
