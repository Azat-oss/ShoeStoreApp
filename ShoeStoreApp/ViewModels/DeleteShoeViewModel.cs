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
    public class DeleteShoeViewModel : INotifyPropertyChanged
    {
        private readonly Action _onDeleted;
        private readonly Action _onCancel;
        private Shoe _selectedShoe; // ← теперь это поле, а не readonly

        public ObservableCollection<Shoe> Shoes { get; }

        // ✅ Полноценное свойство с сеттером и INPC
        public Shoe SelectedShoe
        {
            get => _selectedShoe;
            set
            {
                _selectedShoe = value;
                OnPropertyChanged(); // ← обязательно!
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public DeleteShoeViewModel(Action onDeleted, Action onCancel)
        {
            _onDeleted = onDeleted;
            _onCancel = onCancel;
            DeleteCommand = new RelayCommand(Delete);
            CancelCommand = new RelayCommand(Cancel);

            using var context = new ApplicationDbContext();
            var shoes = context.Shoes.ToList();
            Shoes = new ObservableCollection<Shoe>(shoes);
        }

        private void Delete()
        {
            if (SelectedShoe == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить товар?\n\nАртикул: {SelectedShoe.Article}\nНаименование: {SelectedShoe.Name}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using var context = new ApplicationDbContext();
                    var shoeToDelete = context.Shoes.First(s => s.Id == SelectedShoe.Id);
                    context.Shoes.Remove(shoeToDelete);
                    context.SaveChanges();

                    MessageBox.Show("Товар успешно удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _onDeleted?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Cancel()
        {
            _onCancel?.Invoke();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

