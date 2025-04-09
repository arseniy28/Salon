using System.Windows;
using System.Windows.Controls;
using pomoi.Models;

namespace pomoi.Views
{
    public partial class AddEditVisitWindow : Window
    {
        public Visit Visit { get; private set; }

        public AddEditVisitWindow()
        {
            InitializeComponent();
            Title = "Добавить посещение";
        }

        public AddEditVisitWindow(View selectedVisit)
        {
            InitializeComponent();
            Title = "Редактировать посещение";

            // Заполняем поля данными выбранного посещения
            ClientComboBox.SelectedItem = selectedVisit.FirstName + " " + selectedVisit.LastName;
            VisitDatePicker.SelectedDate = selectedVisit.VisitDate;

            Visit = new Visit
            {
                VisitDate = selectedVisit.VisitDate
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (VisitDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату посещения.");
                return;
            }

            Visit = new Visit
            {
                VisitDate = VisitDatePicker.SelectedDate.Value
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}