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
    /// Interaction logic for MailHelperView.xaml
    /// </summary>


    public partial class MailHelperView : UserControl
    {
        private ViewModels.MailHelperViewModel viewModel;

        public TweetListener2.ViewModels.MailHelperViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.MailHelperViewModel) {
                        viewModel = (ViewModels.MailHelperViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that

                        //viewModel = new ViewModels.MailHelperViewModel();
                        //DataContext = viewModel;
                        return null;
                    }
                }
            }
        }

        public MailHelperView()
        {
            InitializeComponent();

        }
    }


}
