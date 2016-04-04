using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bowtie
{
	/// <summary>
	/// Interaction logic for BowTieTabControl.xaml
	/// </summary>
	public partial class BowtieTabControl : TabControl
	{
		public BowtieTabControl()
		{
			InitializeComponent();
			//add a tab
			BowtieTabItem defaultTab = new BowtieTabItem();
			defaultTab.Header = "tab 1";
			Items.Add(defaultTab);
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
			BowtieTabItem tab = new BowtieTabItem();
			tab.Header = "tab " + (this.Items.Count + 1);
			Items.Add(tab);
			this.SelectedIndex = this.Items.IndexOf(tab);
			tab.Focus();
		}

		private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.Items.Remove(this.SelectedItem);
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach(BowtieTabItem item in this.Items)
			{
				if (item != SelectedItem)
					item.Hide();
			}
		}
	}
}
