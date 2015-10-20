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
    /// Interaction logic for QueryExpansionView.xaml
    /// </summary>


    public partial class QueryExpansionView : UserControl
    {
        private ViewModels.QueryExpansionViewModel viewModel;

        public TweetListener2.ViewModels.QueryExpansionViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.QueryExpansionViewModel) {
                        viewModel = (ViewModels.QueryExpansionViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that

                        //viewModel = new ViewModels.QueryExpansionViewModel();
                        //DataContext = viewModel;
                        return null;
                    }
                }
            }
        }

        public QueryExpansionView()
        {
            InitializeComponent();

        }
    }


}
