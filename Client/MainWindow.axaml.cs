using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Client.Data;

namespace Client
{
	public partial class MainWindow : Window
	{
		private MasterViewModel vm;

		public MainWindow()
		{
			InitializeComponent();
			vm = new MasterViewModel();
			DataContext = vm;
		}

		private void Button_OnClick(object? sender, RoutedEventArgs e)
		{
			Task.Run(() =>
			{
				vm.LoadProducts();
			});
		}
	}
}