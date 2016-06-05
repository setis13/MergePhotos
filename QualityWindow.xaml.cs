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
    /// Interaction logic for QualityWindow.xaml
    /// </summary>
    public partial class QualityWindow : Window
    {
        public QualityWindow()
        {
            InitializeComponent();
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBlock.Text = ((Slider) sender).Value.ToString();
        }
    }
}
