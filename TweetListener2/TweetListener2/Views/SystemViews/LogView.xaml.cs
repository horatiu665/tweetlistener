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
        private ViewModels.LogViewModel viewModel;

        public TweetListener2.ViewModels.LogViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.LogViewModel) {
                        viewModel = (ViewModels.LogViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that

                        //viewModel = new ViewModels.LogViewModel();
                        //DataContext = viewModel;
                        return null;
                    }
                }
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
