using System;
using System.Windows;

namespace Bowtie
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			if(Environment.OSVersion.Version.Major <= 6 && Environment.OSVersion.Version.Minor < 2)//windows 7 has this width limitation
			{
				this.MaxWidth = 650;
			}
			else
			{
				this.MaxWidth = SystemParameters.PrimaryScreenWidth;
			}
		}
		
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			foreach(BowtieTabItem tab in ParentTabControl.Items)
			{
				tab.Close();
			}
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var tab = (ParentTabControl.SelectedItem as BowtieTabItem);
			tab.OnSizeChanged(sender, e);
			tab.OnResize(sender, e);
		}
	}
}
