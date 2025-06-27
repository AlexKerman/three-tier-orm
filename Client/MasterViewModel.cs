using Avalonia.Threading;
using Client.Common;
using Client.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client;

public class MasterViewModel: NotifyPropertyChangedBase
{
	private string fetchedTime = "0";

	public string FetchedTime
	{
		get => fetchedTime;
		set => SetField(ref fetchedTime, value);
	}

	public ObservableCollection<Product> Products { get; set; }

	public MasterViewModel()
	{
		Products = new ObservableCollection<Product>();
	}

	public void LoadProducts()
	{
		var sw = Stopwatch.StartNew();
		foreach (var product in Tables.Products)
		{
			var elapsed = sw.ElapsedMilliseconds + "ms";
			Dispatcher.UIThread.Post(() =>
			{
				FetchedTime = elapsed;
				Products.Add(product);
			});
		}
		sw.Stop();
	}
}