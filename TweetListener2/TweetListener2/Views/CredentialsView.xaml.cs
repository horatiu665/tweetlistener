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
    /// Interaction logic for CredentialsView.xaml
    /// </summary>
    public partial class CredentialsView : UserControl
    {
        ViewModels.CredentialsViewModel ViewModel { get; set; }

        public CredentialsView()
        {
            InitializeComponent();
        }

        private void setCredentialsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCredentials(new List<string>() {
                // "Access_Token"
				Access_Token.Text,
				// "Access_Token_Secret"
				Access_Token_Secret.Text,
				// "Consumer_Key"
				Consumer_Key.Text,
				// "Consumer_Secret"
				Consumer_Secret.Text
            });
        }
    }
}
