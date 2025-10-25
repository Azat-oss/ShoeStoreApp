using ShoeStoreApp.Data;
using ShoeStoreApp.Models;
using ShoeStoreApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace ShoeStoreApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;
        public ObservableCollection<Shoe> Shoes { get; set; }
        public ObservableCollection<string> Groups { get; set; }
        public ObservableCollection<string> Sizes { get; set; }
        public ObservableCollection<string> Models { get; set; }

        private string _selectedGroup;
        private string _selectedSize;
        private string _selectedModel;
        private string _selectedTheme = "Зима"; // по умолчанию
        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
                ApplyTheme(); // применяем тему при изменении
            }
        }
        public ObservableCollection<string> Themes { get; } = new ObservableCollection<string>
    {
        "Зима", "Весна", "Лето", "Осень"
    };

        // Метод для применения цвета
        private void ApplyTheme()
        {
            var color = _selectedTheme switch
            {
                "Зима" => "#FF4A90E2",   // синий
                "Осень" => "#FFF5A623",  // жёлтый/оранжевый
                "Весна" => "#FF7ED321",  // зелёный
                "Лето" => "#FFE74C3C",   // красный
                _ => "#FF4A90E2"
            };

            // Устанавливаем цвет как ресурс на уровне приложения
            Application.Current.Resources["ThemeColor"] = (Color)ColorConverter.ConvertFromString(color);
            Application.Current.Resources["ThemeBrush"] = new SolidColorBrush((Color)Application.Current.Resources["ThemeColor"]);
        }

        public string SelectedGroup
        {
            get => _selectedGroup;
            set { _selectedGroup = value; OnPropertyChanged(); FilterShoes(); }
        }

        public string SelectedSize
        {
            get => _selectedSize;
            set { _selectedSize = value; OnPropertyChanged(); FilterShoes(); }
        }

        public string SelectedModel
        {
            get => _selectedModel;
            set { _selectedModel = value; OnPropertyChanged(); FilterShoes(); }
        }

        public ICommand AddShoeCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand DeleteShoeCommand { get; }

        public ICommand ChangeThemeCommand { get; }
        public ICommand AboutCommand { get; }
        public MainViewModel()
        {
            _context = new ApplicationDbContext();
            LoadData();
            AddShoeCommand = new RelayCommand(OpenAddShoeWindow);
            RefreshCommand = new RelayCommand(LoadData);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
            DeleteShoeCommand = new RelayCommand(OpenDeleteShoeWindow);
            AboutCommand = new RelayCommand(OpenAboutShoeWindow);

            ChangeThemeCommand = new RelayCommand<object>(ChangeTheme);
            // Применяем тему при запуске
            ApplyTheme();
        }

       
        private void ChangeTheme(object param)
        {
             

            string theme = param as string;
            if (theme == null) return;

            var colorHex = theme switch
            {
                "Зима" => "#FF4A90E2",   // Синий
                "Осень" => "#FFF5A623",  // Жёлтый/оранжевый
                "Весна" => "#FF7ED321",  // Зелёный
                "Лето" => "#FFE74C3C",   // Красный
                _ => "#FF4A90E2"          // По умолчанию синий
            };

            // Сообщение о смене темы


            //MessageBox.Show($"Тема изменена на {theme}");//стандартное окно

            var customMsgBox = new CustomMessageBox($"Тема изменена на {theme}");
            customMsgBox.Owner = Application.Current.MainWindow;
            customMsgBox.ShowDialog();




            // Установим новый цвет темы
            Application.Current.Resources["ThemeColor"] = (Color)ColorConverter.ConvertFromString(colorHex);
            Application.Current.Resources["ThemeBrush"] = new SolidColorBrush((Color)Application.Current.Resources["ThemeColor"]);

            // Обновим визуальное представление окон
            foreach (Window window in Application.Current.Windows)
            {
                window.Dispatcher.Invoke(() =>
                {
                    window.InvalidateVisual(); // Принудительная перерисовка окна
                });
            }
        }

        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool>? _canExecute;

            public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute == null || (_canExecute != null && _canExecute((T?)parameter));
            }

            public void Execute(object? parameter)
            {
                _execute((T?)parameter);
            }

            public event EventHandler? CanExecuteChanged;
        }


        private void LoadData()
        {
            var shoes = _context.Shoes.ToList();
            Shoes = new ObservableCollection<Shoe>(shoes);

            Groups = new ObservableCollection<string>(new[] { "Все" }.Concat(shoes.Select(s => s.Group).Distinct().OrderBy(g => g)));
            Sizes = new ObservableCollection<string>(new[] { "Все" }.Concat(shoes.Select(s => s.Size).Distinct().OrderBy(s => s)));
            Models = new ObservableCollection<string>(new[] { "Все" }.Concat(shoes.Select(s => s.ModelName).Distinct().OrderBy(m => m)));
           
            OnPropertyChanged(nameof(Shoes));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(Sizes));
            OnPropertyChanged(nameof(Models));
        }
        private void ResetFilters()
        {
            SelectedGroup = "Все";
            SelectedSize = "Все";
            SelectedModel = "Все";
        }
        private void FilterShoes()
        {
            var query = _context.Shoes.AsQueryable();

            if (!string.IsNullOrEmpty(_selectedGroup) && _selectedGroup != "Все")
                query = query.Where(s => s.Group == _selectedGroup);

            if (!string.IsNullOrEmpty(_selectedSize) && _selectedSize != "Все")
                query = query.Where(s => s.Size == _selectedSize);

            if (!string.IsNullOrEmpty(_selectedModel) && _selectedModel != "Все")
                query = query.Where(s => s.ModelName == _selectedModel);

            Shoes = new ObservableCollection<Shoe>(query.ToList());
            OnPropertyChanged(nameof(Shoes));
        }

        private void OpenAddShoeWindow()
        {
            var groups = _context.Shoes.Select(s => s.Group).Distinct().OrderBy(x => x).ToList();
            var manufacturers = _context.Shoes.Select(s => s.Manufacturer).Distinct().OrderBy(x => x).ToList();
            var sizes = _context.Shoes.Select(s => s.Size).Distinct().OrderBy(x => x).ToList();
            var colors = _context.Shoes.Select(s => s.Color).Distinct().OrderBy(x => x).ToList();
            var materials = _context.Shoes.Select(s => s.Material).Distinct().OrderBy(x => x).ToList();
            var models = _context.Shoes.Select(s => s.ModelName).Distinct().OrderBy(x => x).ToList();

            if (!groups.Any()) groups = new List<string> { "Мужская", "Женская", "Детская" };
            if (!manufacturers.Any()) manufacturers = new List<string> { "Nike", "Adidas", "Ecco" };
            if (!sizes.Any()) sizes = new List<string> { "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45" };
            if (!colors.Any()) colors = new List<string> { "Чёрный", "Белый", "Синий" };
            if (!materials.Any()) materials = new List<string> { "Кожа", "Замша", "Текстиль" };
            if (!models.Any()) models = new List<string> { "Спортивная", "Классическая", "Повседневная" };

            AddShoeWindow? addWindow = null; 

            var viewModel = new AddShoeViewModel(
                onSaved: () =>
                {
                    MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    addWindow?.Close(); 
                },
                onCancel: () =>
                {
                    addWindow?.Close(); 
                },
                groups,
                materials,
                colors,
                manufacturers,
                sizes,
                models
            );

            addWindow = new AddShoeWindow
            {
                DataContext = viewModel
            };

            addWindow.ShowDialog();
        }

        private void OpenDeleteShoeWindow()
        {
            DeleteShoeWindow? deleteWindow = null;

            var viewModel = new DeleteShoeViewModel(
                onDeleted: () =>
                {
                    MessageBox.Show("Список обновлён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData(); // Обновляем список
                    deleteWindow?.Close();
                },
                onCancel: () => deleteWindow?.Close()
            );

            deleteWindow = new DeleteShoeWindow
            {
                DataContext = viewModel
            };

            deleteWindow.ShowDialog();
        }
        private void OpenAboutShoeWindow()
        {
            var aboutWindow = new About();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutWindow.ShowDialog();


        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
