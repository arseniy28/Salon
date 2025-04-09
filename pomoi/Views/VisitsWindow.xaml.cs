using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using pomoi.Models;

namespace pomoi.Views
{
    public partial class VisitsWindow : Window
    {
        public ObservableCollection<View> Visits { get; set; }

        public VisitsWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadVisits();
        }

        private void LoadVisits()
        {
            using var context = new BeautySalonContext();
            var visits = context.View.ToList();
            Visits = new ObservableCollection<View>(visits);
        }
    }
}