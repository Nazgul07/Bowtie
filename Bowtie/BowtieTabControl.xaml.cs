using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bowtie
{
	/// <summary>
	/// Interaction logic for BowTieTabControl.xaml
	/// </summary>
	public partial class BowtieTabControl : TabControl
	{
		private int _tabCount;
		public BowtieTabControl()
		{
			InitializeComponent();
			AddNewTab();
		}

		private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var tabItem = e.Source as TabItem;

			if (tabItem == null)
				return;

			if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
			}
		}


		private void TabItem_Drop(object sender, DragEventArgs e)
		{
			var tabItemTarget = e.Source as BowtieTabItem;

			var tabItemSource = e.Data.GetData(typeof(BowtieTabItem)) as BowtieTabItem;

			if (!tabItemTarget.Equals(tabItemSource))
			{
				var tabControl = tabItemTarget.Parent as TabControl;
				int sourceIndex = tabControl.Items.IndexOf(tabItemSource);
				int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

				tabControl.Items.Remove(tabItemSource);
				tabControl.Items.Insert(targetIndex, tabItemSource);

				tabControl.Items.Remove(tabItemTarget);
				tabControl.Items.Insert(sourceIndex, tabItemTarget);
				tabControl.SelectedIndex = targetIndex;
			}
		}
		
		private void BowtieTabAddButton_Click(object sender, RoutedEventArgs e)
		{
			AddNewTab();
		}

		private void AddNewTab(string app = "cmd.exe")
		{
			BowtieTabItem tab = new BowtieTabItem(app);
			tab.Header = "tab " + (_tabCount++ + 1);
			
			
			Items.Add(tab);
			//(tab.FindName("TabIcon") as System.Windows.Controls.Image).Source = new BitmapImage(new Uri("pack://application:,,,/Resources/cmd.ico"));
			this.SelectedIndex = this.Items.IndexOf(tab);
			tab.Focus();
		}

		private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var grid = VisualTreeHelper.GetParent(sender as System.Windows.Shapes.Path);
			var border = VisualTreeHelper.GetParent(grid);
			grid = VisualTreeHelper.GetParent(border);
			BowtieTabItem tab = VisualTreeHelper.GetParent(grid) as BowtieTabItem;
			tab.Close();
			this.Items.Remove(tab);
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach(BowtieTabItem item in this.Items)
			{
				if (item != SelectedItem)
					item.Hide();
			}
			(SelectedItem as BowtieTabItem).TabItem_GotFocus(sender, null);
		}

		private void BowtieTabAddButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ContextMenu menu = new ContextMenu();
			MenuItem cmdItem = new MenuItem()
			{ 
				Header = "Command Prompt",
				Icon = new System.Windows.Controls.Image
				{
					Source = new BitmapImage(new Uri("pack://application:,,,/Resources/cmd.ico"))
				}
			};
			cmdItem.Click += (object o, RoutedEventArgs args) => AddNewTab();
			menu.Items.Add(cmdItem);

			string regval = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PowerShell\1", "Install", null).ToString();
			if (regval.Equals("1"))
			{
				MenuItem psItem = new MenuItem()
				{
					Header = "PowerShell",
					Icon = new System.Windows.Controls.Image
					{
						Source = new BitmapImage(new Uri("pack://application:,,,/Resources/PS.ico"))
					}
				};
				psItem.Click += (object o, RoutedEventArgs args) => AddNewTab("powershell.exe");
				menu.Items.Add(psItem);
			}
			var button = (sender as BowtieTabAddButton);
			button.ContextMenu = menu;
		}

		public static string GetFullPath(string fileName)
		{
			if (File.Exists(fileName))
				return System.IO.Path.GetFullPath(fileName);

			var values = Environment.GetEnvironmentVariable("PATH");
			foreach (var path in values.Split(';'))
			{
				var fullPath = System.IO.Path.Combine(path, fileName);
				if (File.Exists(fullPath))
					return fullPath;
			}
			return null;
		}
	}
}
