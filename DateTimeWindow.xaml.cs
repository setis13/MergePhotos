using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MergePhotos
{
    /// <summary>
    /// Interaction logic for DateTimeWindow.xaml
    /// </summary>
    public partial class DateTimeWindow : Window
    {
        public DateTime DateTime { get; set; }
        public DateTimeWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker.SelectedDate = DateTime;

        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate != null) DateTime = (DateTime)DatePicker.SelectedDate;
            DialogResult = true;
        }

    }
}
