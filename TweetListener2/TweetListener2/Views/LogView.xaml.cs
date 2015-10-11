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

namespace TweetListener2.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        private TweetListener2.ViewModels.LogViewModel ViewModel
        {
            get
            {
                return (ViewModels.LogViewModel)DataContext;
            }
        }

        public LogView()
        {
            InitializeComponent();

        }

        private void TestMessage_Click(object sender, EventArgs e)
        {
            ViewModel.TestMessage();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Clear();
        }
    }
}
