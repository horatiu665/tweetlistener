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
    /// Interaction logic for TweetDatabaseView.xaml
    /// </summary>

    public partial class TweetDatabaseView : UserControl
    {
        private ViewModels.TweetDatabaseViewModel viewModel;

        public TweetListener2.ViewModels.TweetDatabaseViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.TweetDatabaseViewModel) {
                        viewModel = (ViewModels.TweetDatabaseViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that

                        //viewModel = new ViewModels.TweetDatabaseViewModel();
                        //DataContext = viewModel;
                        return null;
                    }
                }
            }
        }

        public TweetDatabaseView()
        {
            InitializeComponent();

        }
    }

}
