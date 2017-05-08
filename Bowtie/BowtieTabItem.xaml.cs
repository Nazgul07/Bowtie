using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
		

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetCursorPos(int x, int y);
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
					CreateNoWindow = false,
					WorkingDirectory = Environment.CurrentDirectory,
					LoadUserProfile = true,
				});

				
				while (_process.MainWindowHandle == IntPtr.Zero)
				{
					Thread.Sleep(10);
				}
				_processHandle = _process.MainWindowHandle;
				var helper = new WindowInteropHelper(Window.GetWindow(this.ConsoleHost));
				SetParent(_processHandle, helper.Handle);
				SetWindowLongA(_processHandle, GWL_STYLE, WS_VISIBLE);
				int yCoord = (int)ConsoleHost.TransformToAncestor(Window.GetWindow(this.ConsoleHost)).Transform(new Point(0, 0)).Y;
				MoveWindow(_processHandle, 0, yCoord, (int)ConsoleHost.ActualWidth + 3, (int)ConsoleHost.ActualHeight + 3, true);
				ConsoleHost.Focus();
				System.Timers.Timer timer = new System.Timers.Timer(100);
				timer.Elapsed += (o, ee) => {
					OnResize(null, null);
				};
				timer.Start();
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
		/// <param name="s">todo: describe s parameter on OnSizeChanged</param>
		internal void OnSizeChanged(object s, SizeChangedEventArgs e)
		{
			this.InvalidateVisual();
		}
		/// <summary>
		/// Update display of the executable
		/// </summary>
		/// <param name="e">Not used</param>
		/// <param name="s">todo: describe s parameter on OnResize</param>
		internal void OnResize(object s, SizeChangedEventArgs e)
		{
			Dispatcher.InvokeAsync(() => { 
				var win = Window.GetWindow(this.ConsoleHost);
				if (_processHandle != IntPtr.Zero && win != null && win.IsAncestorOf(ConsoleHost))
				{
					int yCoord = (int)ConsoleHost.TransformToAncestor(win).Transform(new Point(0, 0)).Y;
					MoveWindow(_processHandle, 0, yCoord, (int)ConsoleHost.ActualWidth + 3, (int)ConsoleHost.ActualHeight + 3, true);
				}
			});
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
