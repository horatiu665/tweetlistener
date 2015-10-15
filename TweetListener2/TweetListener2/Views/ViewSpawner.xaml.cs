using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TweetListener2.ViewModels;

namespace TweetListener2.Views
{
    /// <summary>
    /// Interaction logic for ViewSpawner.xaml
    /// </summary>
    public partial class ViewSpawner : UserControl
    {
        public ViewSpawner()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.instance.RemovePanel((ViewModelBase)DataContext);
        }
    }
}
