using CommunityToolkit.Maui.Alerts;
using GNS.Pages;

namespace GNS.Pages.Templates;

public partial class BallonLoadingView : ContentView
{

	public BallonLoadingView()
	{
		InitializeComponent();
		BindingContext = this;
	}


	#region Ñâîéñòâà
	private string _carNumber;
	public string CarNumber
	{
		get => _carNumber;
		set
		{
			_carNumber = value;
			OnPropertyChanged();
		}
	}

	private string _carBrand;
	public string CarBrand
	{
		get => _carBrand;
		set
		{
			_carBrand = value;
			OnPropertyChanged();
		}
	}

	private string _carType;
	public string CarType
	{
		get => _carType;
		set
		{
			_carType = value;
			OnPropertyChanged();
		}
	}

	private string _trailerNumber;
	public string TrailerNumber
	{
		get => _trailerNumber;
		set
		{
			_trailerNumber = value;
			OnPropertyChanged();
		}
	}

	private string _trailerBrand;
	public string TrailerBrand
	{
		get => _trailerBrand;
		set
		{
			_trailerBrand = value;
			OnPropertyChanged();
		}
	}

	private string _trailerType;
	public string TrailerType
	{
		get => _trailerType;
		set
		{
			_trailerType = value;
			OnPropertyChanged();
		}
	}

	private int? _readerType;
	public int? ReaderType
	{
		get => _readerType;
		set
		{
			_readerType = value;
			OnPropertyChanged();
		}
	}

	private string _ttn;
	public string TTN
	{
		get => _ttn;
		set
		{
			_ttn = value;
			OnPropertyChanged();
		}
	}

	private int? _amountOfTtn;
	public int? AmountOfTtn
	{
		get => _amountOfTtn;
		set
		{
			_amountOfTtn = value;
			OnPropertyChanged();
		}
	}

	public Grid StackStopLoading => stack_StopLoading;
	public Grid CarTrailerReadderSelectionBlock => car_trailer_reader_selection_block;
	public Label LabelCarNumber => carNumber;
	public Label LabelCarBrand => carBrand;
	public Label LabelReaderType => readerType;
	public Label LabelScannedBallonsAmount => ballons_amount_label;
	public Editor LabelUnscannedballons5 => unscannedBallonsEntry5;
	public Editor LabelUnscannedballons12 => unscannedBallonsEntry12;
	public Editor LabelUnscannedballons27 => unscannedBallonsEntry27;
	public Editor LabelUnscannedballons50 => unscannedBallonsEntry50;
	public Button ButtonStartLoading => button_StartLoading;
	public Button ButtonStopLoading => button_StopLoading;
	public Button ButtonSelectCar => button_SelectCarInit;
	public Button ButtonSelectReader => button_SelectReaderInit;
	public Grid CarDataFrame => carDataFrame;
	public Grid ReaderDataFrame => readerDataFrame;

	public event EventHandler<EventArgs> OnButtonSelectCarClickHandler;
	public event EventHandler<EventArgs> OnButtonStartLoadingClickHandler;
	public event EventHandler<EventArgs> OnButtonStopLoadingClickHandler;
	public event EventHandler<EventArgs> OnButtonSelectTrailerClickHandler;
	public event EventHandler<EventArgs> OnButtonSelectReaderClickHandler;
	public event EventHandler<EventArgs> OnButtonAddBallonToBatchClickHandler;
	public event EventHandler<EventArgs> OnButtonDeleteBallonFromBatchClickHandler;
	#endregion


	#region Ìåòîäû
	private void Button_SelectCar_Clicked(object sender, EventArgs e)
	{
		OnButtonSelectCarClickHandler?.Invoke(this, new EventArgs());
	}

	private void Button_SelectTrailer_Clicked(object sender, EventArgs e)
	{
		OnButtonSelectTrailerClickHandler?.Invoke(this, new EventArgs());
	}

	private void Button_SelectReader_Clicked(object sender, EventArgs e)
	{
		OnButtonSelectReaderClickHandler?.Invoke(this, new EventArgs());
	}

	private void Button_StartLoading_Clicked(object sender, EventArgs e)
	{
		OnButtonStartLoadingClickHandler?.Invoke(this, new EventArgs());
	}

	private void Button_StopLoading_Clicked(object sender, EventArgs e)
	{
		OnButtonStopLoadingClickHandler?.Invoke(this, new EventArgs());
	}

	private void Button_add_ballon_to_batch_Clicked(object sender, EventArgs e)
	{
		OnButtonAddBallonToBatchClickHandler?.Invoke(this, new EventArgs());
	}

	private void Button_delete_ballon_to_batch_Clicked(object sender, EventArgs e)
	{
		OnButtonDeleteBallonFromBatchClickHandler?.Invoke(this, new EventArgs());
	}
	#endregion
}