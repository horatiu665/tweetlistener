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
    /// Interaction logic for StreamView.xaml
    /// </summary>
    public partial class StreamView : UserControl
    {
        private ViewModels.StreamViewModel viewModel;

        public TweetListener2.ViewModels.StreamViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.StreamViewModel) {
                        viewModel = (ViewModels.StreamViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that

                        //viewModel = new ViewModels.StreamViewModel();
                        //DataContext = viewModel;
                        return null;
                    }
                }
            }
        }

        public StreamView()
        {
            InitializeComponent();

        }

        private void StartStream_Button(object sender, RoutedEventArgs e)
        {
            ViewModel.IsRunning = true;
        }
    }
}
