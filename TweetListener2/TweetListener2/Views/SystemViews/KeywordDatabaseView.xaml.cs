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
    /// Interaction logic for KeywordDatabaseView.xaml
    /// </summary>

    public partial class KeywordDatabaseView : UserControl
    {
        private ViewModels.KeywordDatabaseViewModel viewModel;

        public TweetListener2.ViewModels.KeywordDatabaseViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.KeywordDatabaseViewModel) {
                        viewModel = (ViewModels.KeywordDatabaseViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that

                        //viewModel = new ViewModels.KeywordDatabaseViewModel();
                        //DataContext = viewModel;
                        return null;
                    }
                }
            }
        }

        public KeywordDatabaseView()
        {
            InitializeComponent();

        }
    }

}
