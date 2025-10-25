using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShoeStoreApp.ViewModels
{
    public class AddShoeViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;
        private readonly Action _onSaved;
        private readonly Action _onCancel;

        public Shoe NewShoe { get; set; } = new();

        // Списки для выпадающих меню
        public ObservableCollection<string> Groups { get; }
        public ObservableCollection<string> Materials { get; }
        public ObservableCollection<string> Colors { get; }
        public ObservableCollection<string> Manufacturers { get; }
        public ObservableCollection<string> Sizes { get; }
        public ObservableCollection<string> Models { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Конструктор с передачей существующих значений
        public AddShoeViewModel(
            Action onSaved,
            Action onCancel,
            List<string> groups,
            List<string> materials,
            List<string> colors,
            List<string> manufacturers,
            List<string> sizes,
            List<string> models)
        {
            _context = new ApplicationDbContext();
            _onSaved = onSaved;
            _onCancel = onCancel;
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            Groups = new ObservableCollection<string>(groups);
            Materials = new ObservableCollection<string>(materials);
            Colors = new ObservableCollection<string>(colors);
            Manufacturers = new ObservableCollection<string>(manufacturers);
            Sizes = new ObservableCollection<string>(sizes);
            Models = new ObservableCollection<string>(models);
        }

        private void Save()
        {
            // 1. Проверка обязательных текстовых полей
            if (string.IsNullOrWhiteSpace(NewShoe.Name) ||
                string.IsNullOrWhiteSpace(NewShoe.Article) ||
                string.IsNullOrWhiteSpace(NewShoe.Group) ||
                string.IsNullOrWhiteSpace(NewShoe.Manufacturer) ||
                string.IsNullOrWhiteSpace(NewShoe.Size) ||
                string.IsNullOrWhiteSpace(NewShoe.Color) ||
                string.IsNullOrWhiteSpace(NewShoe.Material) ||
                string.IsNullOrWhiteSpace(NewShoe.ModelName))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Проверка, что размер — число (целое или дробное)
            if (!decimal.TryParse(NewShoe.Size, out _))
            {
                MessageBox.Show("Размер должен быть числом (например, 36 или 36.5).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Проверка гарантийного срока — целое положительное число
            if (NewShoe.WarrantyMonths <= 0)
            {
                MessageBox.Show("Гарантийный срок должен быть больше 0 месяцев.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 4. Проверка цены — положительное число
            if (NewShoe.Price <= 0)
            {
                MessageBox.Show("Цена должна быть больше нуля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 5. Установка даты производства по умолчанию (если не задана)
            if (NewShoe.ProductionDate == default)
                NewShoe.ProductionDate = DateTime.Today;

            // 6. Проверка уникальности артикула
            using var context = new ApplicationDbContext();
            if (context.Shoes.Any(s => s.Article == NewShoe.Article))
            {
                MessageBox.Show($"Артикул \"{NewShoe.Article}\" уже существует. Введите уникальный артикул.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 7. Сохранение в БД
            try
            {
                context.Shoes.Add(NewShoe);
                context.SaveChanges();
                _onSaved?.Invoke(); // Закроет окно и обновит список в MainViewModel
            }
            catch (Exception ex)
            {
                // Дополнительная защита на случай, если дубликат проскочил (например, при параллельном добавлении)
                var exceptionMessage = ex.InnerException?.Message ?? ex.Message;
                if (exceptionMessage.Contains("UNIQUE") ||
                    exceptionMessage.Contains("duplicate") ||
                    exceptionMessage.Contains("артикул") ||
                    exceptionMessage.Contains("Article") ||
                    exceptionMessage.Contains("нарушение уникального"))
                {
                    MessageBox.Show("Артикул уже существует. Пожалуйста, введите другой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        

        private void Cancel()
        {
            _onCancel(); // Закрываем окно
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
