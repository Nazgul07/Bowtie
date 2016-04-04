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
		}

		//private void Border_LeftMouseDown(object sender, MouseButtonEventArgs e)
		//{
		//	if (e.ClickCount == 2)
		//	{
		//		Maximize_Click(sender, e);
		//	}
		//	else if (e.ChangedButton == MouseButton.Left)
		//	{ 
		//		this.DragMove();
		//	}
		//}

		//private void Close_Click(object sender, RoutedEventArgs e)
		//{
		//	SystemCommands.CloseWindow(this);
		//}

		//private void Maximize_Click(object sender, RoutedEventArgs e)
		//{
		//	if (this.WindowState == WindowState.Maximized)
		//	{
		//		SystemCommands.RestoreWindow(this);
		//	}
		//	else
		//	{
		//		SystemCommands.MaximizeWindow(this);
		//	}
		//}

		//private void Minimize_Click(object sender, RoutedEventArgs e)
		//{
		//	SystemCommands.MinimizeWindow(this);
		//}

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
