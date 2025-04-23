using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Services.Intrefaces;
using System.Collections.ObjectModel;
using static Services.Intrefaces.IBatchService;


namespace GNS.Pages.Popup;

public partial class CarCollectionPopup : ContentPage
{
	private ObservableCollection<CarVM> _stationCars;
	public ObservableCollection<CarVM> StationCars
	{
		get => _stationCars;
		set
		{
			_stationCars = value;
			OnPropertyChanged(nameof(StationCars));
		}
	}

	public event EventHandler<object> ItemSelected;

	private IBatchService _batchService;

	public CarCollectionPopup(IBatchService batchService)
	{
		InitializeComponent();

		_batchService = batchService;
		BindingContext = this;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadCarListAsync();
	}

	private async Task LoadCarListAsync()
	{
		StationCars ??= [];

		Dispatcher.Dispatch(async () =>
		{
			StationCars?.Clear();

			var getCarsResult = await _batchService.GetStationCars();

			if (getCarsResult.IsError)
			{
				await Toast.Make($"{getCarsResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				return;
			}

			var cars = getCarsResult.Value;

			if (cars is null || !cars.Any())
			{
				if (Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync();

				return;
			}

			foreach (var car in cars)
			{
				StationCars?.Add(car);
			}
		});
	}

	private async void CarCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ItemSelected?.Invoke(e, e.CurrentSelection[0]);
	}
}