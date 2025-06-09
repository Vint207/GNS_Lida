using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Services.Intrefaces;
using System.Collections.ObjectModel;
using static Services.Intrefaces.IBallonService;

namespace GNS.Pages.Popup;

public partial class BallonsListPage : ContentPage
{
	private ObservableCollection<BallonVM> _ballonslList;
	public ObservableCollection<BallonVM> BallonslList
	{
		get => _ballonslList;
		set
		{
			_ballonslList = value;
			OnPropertyChanged(nameof(BallonslList));
		}
	}
	public event EventHandler<object> ItemSelected;
	private IBallonService _ballonService;
	private string _serialNumber;

	public BallonsListPage(string serialNumber, IBallonService ballonService)
	{
		InitializeComponent();

		BindingContext = this;
		_ballonService = ballonService;
		_serialNumber = serialNumber;
		LoadBallonsListAsync();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
	}

	private void LoadBallonsListAsync()
	{
		BallonslList ??= [];
		Dispatcher.Dispatch(async () =>
		{
			BallonslList?.Clear();
			var getBallonResult = await _ballonService.GetBallonBySerialNumber(_serialNumber);
			if (getBallonResult.IsError)
			{
				await Toast.Make($"{getBallonResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				if(Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync();

				return;
			}

			var ballons = getBallonResult.Value;

			if (ballons is null || !ballons.Any())
			{
				if (Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync();

				return;
			}

			foreach (var ballon in ballons)
				BallonslList?.Add(ballon);

			await Task.Delay(50);
		});
	}

	private void Batch_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ItemSelected?.Invoke(e, e.CurrentSelection[0]);
	}
}