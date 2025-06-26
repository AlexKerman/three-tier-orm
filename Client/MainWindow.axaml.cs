using System.Linq;
using Avalonia.Controls;
using Client.Data;

namespace Client
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var list = Tables.Costs
				.Include(c => c.Promo)
				.Where(c => c.ProdId == 0 && c.UnitCost > 10.0m)
				.OrderBy(c => c.ChannelId)
				.Include(c => c.Prod)
				.Take(10)
				.ToList();
		}
	}
}