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
    /// Interaction logic for AllResourcesView.xaml
    /// </summary>
    public partial class AllResourcesView : UserControl
    {
        private AllResourcesViewModel viewModel;

        public AllResourcesViewModel ViewModel
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is AllResourcesViewModel) {
                        viewModel = (AllResourcesViewModel)DataContext;
                        return viewModel;

                    } else {
                        // do not spawn new viewmodel, but rather let systemsmanager handle that
                        
                        return null;
                    }
                }
            }
        }

        public AllResourcesView()
        {
            InitializeComponent();
        }
        private void AddNewStream_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewStream_Click();
        }
        private void AddNewRest_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewRest_Click();
        }
        private void AddNewCredentials_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewCredentials_Click();
        }
        private void AddNewDatabase_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewDatabase_Click();
        }
        private void AddNewLog_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewLog_Click();
        }
        private void AddNewKeywordDatabase_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewKeywordDatabase_Click();
        }
        private void AddNewTweetDatabase_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewTweetDatabase_Click();
        }
        private void AddNewQueryExpansion_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewQueryExpansion_Click();
        }
        private void AddNewPorterStemmer_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewPorterStemmer_Click();
        }
        private void AddNewMailHelper_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewMailHelper_Click();
        }

        private void AddNewTweetListener_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewTweetListener_Click();
        }
    }
}
