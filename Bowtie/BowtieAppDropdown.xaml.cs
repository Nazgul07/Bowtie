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

namespace Bowtie
{
	/// <summary>
	/// Interaction logic for BowTieTabItem.xaml
	/// </summary>
	public partial class BowtieAppDropdown : ComboBox
	{
		public BowtieAppDropdown()
		{
			InitializeComponent();
			Items.Add(new ComboBoxItem { Tag = "Command Prompt", Content = new Image { Source = MainWindow.CMDImageSource } });
			if(MainWindow.PowerShellInstalled)
				Items.Add(new ComboBoxItem { Tag = "PowerShell", Content = new Image { Source = MainWindow.PSImageSource } });
			
		}
		public string SelectedApplication
		{
			get
			{
				switch (SelectedIndex)
				{
					case 0:
						return "cmd.exe";
					case 1:
						return "powershell.exe";
					default:
						return null;
				}
			}
		}

		public ImageSource SelectedImage
		{
			get
			{
				switch (SelectedIndex)
				{
					case 0:
						return MainWindow.CMDImageSource;
					case 1:
						return MainWindow.PSImageSource;
					default:
						return null;
				}
			}
		}

		private void BowtieAppDropdownControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((SelectedItem as ComboBoxItem).Content as Image).Source = SelectedImage;
		}

		private void BowtieAppDropdownControl_Loaded(object sender, RoutedEventArgs e)
		{
			IsDropDownOpen = true;
			IsDropDownOpen = false;//fixes and odd bitmap zooming bug on starup
			SelectedItem = Items[0];
		}
	}
}
