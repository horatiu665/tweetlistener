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
using System.Windows.Shapes;

namespace WPFTwitter
{
	/// <summary>
	/// Interaction logic for HelpHelper.xaml
	/// </summary>
	public partial class HelpHelper : Window
	{
		private static bool exists = false;

		public string Document
		{
			get;
			set;
		}

		public HelpHelper()
		{
			if (!exists) {
				exists = true;
				InitializeComponent();
				this.Show();
			} else {
				this.Close();
				return;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			exists = false;
		}
	}
}
