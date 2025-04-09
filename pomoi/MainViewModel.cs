using Microsoft.EntityFrameworkCore;
using pomoi.Models;
using pomoi.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace pomoi
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        // Свойства клиентов
        private ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set
            {
                _clients = value;
                OnPropertyChanged();
            }
        }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
                (EditClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteClientCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Команды для работы с клиентами
        public ICommand RefreshCommand { get; }
        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }

        // Команда для открытия окна посещений
        public ICommand OpenVisitsWindowCommand { get; }

        public MainViewModel()
        {
            Clients = new ObservableCollection<Client>();

            // Инициализация команд
            RefreshCommand = new RelayCommand(async _ => await LoadClientsAsync());
            AddClientCommand = new RelayCommand(async _ => await ExecuteAddClient(), _ => true);
            EditClientCommand = new RelayCommand(async param => await ExecuteEditClient(param), CanExecuteSelectedClient);
            DeleteClientCommand = new RelayCommand(async param => await ExecuteDeleteClient(param), CanExecuteSelectedClient);
            OpenVisitsWindowCommand = new RelayCommand(_ => ExecuteOpenVisitsWindow());

            // Загрузка данных при старте
            _ = LoadClientsAsync();
        }

        // Загрузка клиентов из базы данных
        private async Task LoadClientsAsync()
        {
            try
            {
                using var context = new BeautySalonContext();
                var clients = await context.Clients
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();

                Clients = new ObservableCollection<Client>(clients);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Добавление клиента
        private async Task ExecuteAddClient()
        {
            var addWindow = new AddEditClientWindow();

            if (addWindow.ShowDialog() == true && addWindow.Client != null)
            {
                try
                {
                    using var context = new BeautySalonContext();
                    context.Clients.Add(addWindow.Client);
                    await context.SaveChangesAsync();
                    await LoadClientsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Редактирование клиента
        private async Task ExecuteEditClient(object parameter)
        {
            if (parameter is Client selectedClient)
            {
                var editWindow = new AddEditClientWindow(selectedClient);

                if (editWindow.ShowDialog() == true && editWindow.Client != null)
                {
                    try
                    {
                        using var context = new BeautySalonContext();
                        var clientToUpdate = await context.Clients.FindAsync(selectedClient.ClientId);
                        if (clientToUpdate != null)
                        {
                            clientToUpdate.FirstName = editWindow.Client.FirstName;
                            clientToUpdate.LastName = editWindow.Client.LastName;
                            clientToUpdate.Phone = editWindow.Client.Phone;
                            clientToUpdate.Email = editWindow.Client.Email;
                            clientToUpdate.BirthDate = editWindow.Client.BirthDate;

                            await context.SaveChangesAsync();
                            await LoadClientsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при редактировании клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Удаление клиента
        private async Task ExecuteDeleteClient(object parameter)
        {
            if (parameter is Client selectedClient)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить клиента {selectedClient.FirstName} {selectedClient.LastName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var context = new BeautySalonContext();
                        var clientToDelete = await context.Clients
                            .Include(c => c.Visits)
                            .ThenInclude(v => v.Services)
                            .FirstOrDefaultAsync(c => c.ClientId == selectedClient.ClientId);

                        if (clientToDelete != null)
                        {
                            foreach (var visit in clientToDelete.Visits.ToList())
                            {
                                foreach (var service in visit.Services.ToList())
                                {
                                    context.Services.Remove(service);
                                }
                                context.Visits.Remove(visit);
                            }

                            context.Clients.Remove(clientToDelete);
                            await context.SaveChangesAsync();
                            await LoadClientsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Открытие окна посещений
        private Task ExecuteOpenVisitsWindow()
        {
            var visitsWindow = new VisitsWindow();
            visitsWindow.ShowDialog();
            return Task.CompletedTask; // Возвращаем завершенный Task
        }

        // Условие для команд, связанных с выбранным клиентом
        private bool CanExecuteSelectedClient(object parameter) =>
            parameter is Client;

        // Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Класс RelayCommand
        public class RelayCommand : ICommand
        {
            private readonly Func<object, Task> _executeAsync;
            private readonly Func<object, bool> _canExecute;

            public RelayCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute = null)
            {
                _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

            public async void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    try
                    {
                        await _executeAsync(parameter);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}