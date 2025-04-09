using System;
using System.Windows;
using pomoi.Models;

namespace pomoi.Views
{
    public partial class AddEditClientWindow : Window
    {
        // Свойство для хранения редактируемого или нового клиента
        public Client Client { get; private set; }

        private readonly bool _isEditMode;

        // Конструктор для добавления нового клиента
        public AddEditClientWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            Title = "Добавить клиента";
        }

        // Конструктор для редактирования существующего клиента
        public AddEditClientWindow(Client client)
        {
            InitializeComponent();
            _isEditMode = true;
            Title = "Редактировать клиента";

            // Заполняем поля данными клиента
            FirstNameBox.Text = client.FirstName;
            LastNameBox.Text = client.LastName;
            PhoneBox.Text = client.Phone;
            EmailBox.Text = client.Email;
            BirthDatePicker.SelectedDate = client.BirthDate?.ToDateTime(TimeOnly.MinValue);

            Client = client; // Сохраняем ссылку на клиента
        }

        // Обработчик кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            // Создаем объект клиента
            Client = new Client
            {
                ClientId = _isEditMode ? Client.ClientId : 0, // Для нового клиента Id = 0
                FirstName = FirstNameBox.Text,
                LastName = LastNameBox.Text,
                Phone = PhoneBox.Text,
                Email = EmailBox.Text,
                BirthDate = DateOnly.FromDateTime(BirthDatePicker.SelectedDate ?? DateTime.Now)
            };

            DialogResult = true; // Устанавливаем результат диалога
            Close(); // Закрываем окно
        }

        // Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Отменяем операцию
            Close(); // Закрываем окно
        }
    }
}