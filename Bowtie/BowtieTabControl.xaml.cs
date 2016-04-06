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
			AddNewTab((this.Template.FindName("ApplicationCombo", this) as BowtieAppDropdown).SelectedApplication);
		}

		private void AddNewTab(string app = "cmd.exe")
		{
			BowtieTabItem tab = new BowtieTabItem(app);
			tab.Header = "tab " + (_tabCount++ + 1);
			Items.Add(tab);
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
			if(SelectedItem != null)
				(SelectedItem as BowtieTabItem).TabItem_GotFocus(sender, null);
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
