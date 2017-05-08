using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bowtie
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static ImageSource CMDImageSource; 
		public static ImageSource PSImageSource; 
		public static bool PowerShellInstalled;

		public MainWindow()
		{
			InitializeComponent();
			DependencyObject dep = new DependencyObject();
			if (!DesignerProperties.GetIsInDesignMode(dep))
			{
				CMDImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/cmd.ico"));
				PSImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/PS.ico"));
				PowerShellInstalled = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PowerShell\1", "Install", null).ToString().Equals("1");
			}
			if (Environment.OSVersion.Version.Major <= 6 && Environment.OSVersion.Version.Minor < 2)//windows 7 has this width limitation
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
