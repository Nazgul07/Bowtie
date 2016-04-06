using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Bowtie
{

	/// <summary>
	/// Interaction logic for BowTieTabItem.xaml
	/// </summary>
	public partial class BowtieTabItem : TabItem
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private const int GWL_STYLE = (-16);
		private const int WS_VISIBLE = 0x10000000;
		[DllImport("user32.dll", SetLastError = true)]
		private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


		[DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
		public static extern int SetWindowLongA([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nIndex, int dwNewLong);


		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

		public static readonly DependencyProperty ExecutableProperty =	DependencyProperty.Register("Executable", typeof(String), typeof(BowtieTabItem));
		
		public string Executable
		{
			get { return (string)GetValue(ExecutableProperty); }
			set { SetValue(ExecutableProperty, value); }
		}
		public Process _process;

		private IntPtr _processHandle = IntPtr.Zero;

		private bool _loaded;

		protected void OnLoaded(object s, RoutedEventArgs e)
		{
			if (!_loaded)
			{
				_loaded = true;
				_process = Process.Start(new ProcessStartInfo
				{
					FileName = Executable,
					UseShellExecute = false,
					WorkingDirectory = Environment.CurrentDirectory,
					LoadUserProfile = true
				});
				while (_process.MainWindowHandle == IntPtr.Zero)
				{
					Thread.Sleep(10);
				}
				_processHandle = _process.MainWindowHandle;
				var helper = new WindowInteropHelper(Window.GetWindow(this.ConsoleHost));
				SetParent(_processHandle, helper.Handle);
				SetWindowLongA(_processHandle, GWL_STYLE, WS_VISIBLE);
				MoveWindow(_processHandle, 0, 32, (int)ConsoleHost.ActualWidth + 3, (int)ConsoleHost.ActualHeight + 3, true);
				ConsoleHost.Focus();
			}
		}
		public BowtieTabItem(string app)
		{
			InitializeComponent();
			this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
			this.Loaded += new RoutedEventHandler(OnLoaded);
			this.SizeChanged += new SizeChangedEventHandler(OnResize);
			Executable = app;
		}

		internal void Hide()
		{
			ShowWindow(_processHandle, 0);
		}

		internal void Show()
		{
			ShowWindow(_processHandle, 1);
		}

		/// <summary>
		/// Force redraw of control when size changes
		/// </summary>
		/// <param name="e">Not used</param>
		internal void OnSizeChanged(object s, SizeChangedEventArgs e)
		{
			this.InvalidateVisual();
		}
		/// <summary>
		/// Update display of the executable
		/// </summary>
		/// <param name="e">Not used</param>
		internal void OnResize(object s, SizeChangedEventArgs e)
		{
			if (_processHandle != IntPtr.Zero)
			{
				MoveWindow(_processHandle, 0, 32, (int)ConsoleHost.ActualWidth + 3, (int)ConsoleHost.ActualHeight + 3, true);
			}
		}
		
		public void Close()
		{
			_process.Kill();
			_process.Dispose();
		}

		internal void TabItem_GotFocus(object sender, RoutedEventArgs e)
		{
			Show();
			this.InvalidateVisual();
			ConsoleHost.UpdateLayout();
			OnResize(sender, null);

			//give back focus to the tab instead of the console so you can use arrow keys to navigate tabs
			this.GotFocus -= TabItem_GotFocus;
			this.Focus();
			this.GotFocus += TabItem_GotFocus;
		}
		
	}
}
