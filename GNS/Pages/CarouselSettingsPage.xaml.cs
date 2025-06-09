using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ErrorOr;
using Services.Intrefaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using static Services.Intrefaces.ICarouselService;

namespace GNS.Pages;

public partial class CarouselSettingsPage : ContentPage, INotifyPropertyChanged
{
	public CarouselSettingsPage(ICarouselService carouselService)
	{
		InitializeComponent();

		_carouselService = carouselService;

		MainThread.BeginInvokeOnMainThread(() =>
		{
			PostCorrectors = [];
			for (int i = 1; i < 21; i++)
			{
				PostCorrectors.Add(new CorrectorItemVm
				{
					Index = i.ToString(),
					Value = string.Empty
				});
			}

			OnPropertyChanged(nameof(PostCorrectors));

			//ImageCollection =
			//[
			//	new("pngwing_1.png"),
			//	new("pngwing_2.png"),
			//	new("pngwing_3.png"),
			//	new("pngwing_5.png"),
			//];
		});


		_ = Task.Run(async () =>
		{
			await Task.Delay(200);

		});
	}


	#region Properties
	private readonly ICarouselService _carouselService;

	private bool _readOnlyFromPost;
	public bool ReadOnlyFromPost
	{
		get => _readOnlyFromPost;
		set
		{
			_readOnlyFromPost = value;
			OnPropertyChanged();
		}
	}

	private bool _useWeightCorrection;
	public bool UseWeightCorrection
	{
		get => _useWeightCorrection;
		set
		{
			_useWeightCorrection = value;
			OnPropertyChanged();
		}
	}

	private bool _useCommonValueOfWeightCorrection;
	public bool UseCommonValueOfWeightCorrection
	{
		get => _useCommonValueOfWeightCorrection;
		set
		{
			_useCommonValueOfWeightCorrection = value;
			OnPropertyChanged();
		}
	}


	private string _weightCorrectionValue;
	public string WeightCorrectionValue
	{
		get => _weightCorrectionValue;
		set
		{
			_weightCorrectionValue = value;
			OnPropertyChanged();
		}
	}


	private ObservableCollection<CorrectorItemVm> _postCorrectors;
	public ObservableCollection<CorrectorItemVm> PostCorrectors
	{
		get => _postCorrectors;
		set
		{
			_postCorrectors = value;
			OnPropertyChanged(nameof(PostCorrectors));
		}
	}

	public class CorrectorItemVm : INotifyPropertyChanged
	{
		private string _index;
		public string Index
		{
			get => _index;
			set
			{
				_index = value;
				OnPropertyChanged();
			}
		}

		private string _value;
		public string Value
		{
			get => _value;
			set
			{
				_value = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}


	//private ObservableCollection<GalleryModelVm> _imageCollection;
	//public ObservableCollection<GalleryModelVm> ImageCollection
	//{
	//	get => _imageCollection;
	//	set
	//	{
	//		_imageCollection = value;
	//		OnPropertyChanged();
	//	}
	//}
	//public class GalleryModelVm : INotifyPropertyChanged
	//{
	//	public GalleryModelVm(string imageString)
	//	{
	//		ImageName = imageString;
	//	}

	//	private string _imageName;
	//	public string ImageName
	//	{
	//		get => _imageName; 
	//		set { _imageName = value; }
	//	}

	//	public event PropertyChangedEventHandler? PropertyChanged;
	//	protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
	//	{
	//		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	//	}
	//}
	#endregion


	#region On_Appearing
	protected override async void OnAppearing()
	{
		try
		{
			base.OnAppearing();

			var getCarouselParametersResult = await _carouselService.GetCarouselParameters();

			if (getCarouselParametersResult.IsError)
			{
				await Toast.Make($"{getCarouselParametersResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				return;
			}

			var carouselParameters = getCarouselParametersResult.Value;

			ReadOnlyFromPost = carouselParameters.ReadOnly;
			UseWeightCorrection = carouselParameters.UseWeightManagement;
			UseCommonValueOfWeightCorrection = carouselParameters.UseCommonCorrection;
			WeightCorrectionValue = carouselParameters.WeightCorrectionValue.ToString();
			PostCorrectors[0].Value = carouselParameters.Post1Correction.ToString();
			PostCorrectors[1].Value = carouselParameters.Post2Correction.ToString();
			PostCorrectors[2].Value = carouselParameters.Post3Correction.ToString();
			PostCorrectors[3].Value = carouselParameters.Post4Correction.ToString();
			PostCorrectors[4].Value = carouselParameters.Post5Correction.ToString();
			PostCorrectors[5].Value = carouselParameters.Post6Correction.ToString();
			PostCorrectors[6].Value = carouselParameters.Post7Correction.ToString();
			PostCorrectors[7].Value = carouselParameters.Post8Correction.ToString();
			PostCorrectors[8].Value = carouselParameters.Post9Correction.ToString();
			PostCorrectors[9].Value = carouselParameters.Post10Correction.ToString();
			PostCorrectors[10].Value = carouselParameters.Post11Correction.ToString();
			PostCorrectors[11].Value = carouselParameters.Post12Correction.ToString();
			PostCorrectors[12].Value = carouselParameters.Post13Correction.ToString();
			PostCorrectors[13].Value = carouselParameters.Post14Correction.ToString();
			PostCorrectors[14].Value = carouselParameters.Post15Correction.ToString();
			PostCorrectors[15].Value = carouselParameters.Post16Correction.ToString();
			PostCorrectors[16].Value = carouselParameters.Post17Correction.ToString();
			PostCorrectors[17].Value = carouselParameters.Post18Correction.ToString();
			PostCorrectors[18].Value = carouselParameters.Post19Correction.ToString();
			PostCorrectors[19].Value = carouselParameters.Post20Correction.ToString();
		}
		catch (Exception ex)
		{
			await Toast.Make($"{ex.Message}", ToastDuration.Long, 16).Show();
		}
	}
	#endregion


	private async void ButtonSaveCarouselSettings_Clicked(object sender, EventArgs e)
	{
		try
		{
			double[] parsedPostCorrectors = new double[20];
			int i = 0;
			foreach (var postCorrector in PostCorrectors)
			{
				if (!double.TryParse(postCorrector.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedPostCorrectors[i++]))
				{
					if (!double.TryParse(postCorrector.Value, NumberStyles.Any, new CultureInfo("ru-RU"), out parsedPostCorrectors[i]))
					{
						await Toast.Make($"Нерорректный формат коррекции веса {i}", ToastDuration.Long, 16).Show();
						return;
					}					
				}
			}

			if (!double.TryParse(WeightCorrectionValue, out double parsedWeightCorrectionValue))
			{
				await Toast.Make($"Нерорректный формат общего значение коррекции веса", ToastDuration.Long, 16).Show();
				return;
			}

			var carouselParameters = new SetCarouselCorrectionRequest(
				1,
				ReadOnlyFromPost,
				UseWeightCorrection,
				UseCommonValueOfWeightCorrection,
				parsedWeightCorrectionValue,
				parsedPostCorrectors[0],
				parsedPostCorrectors[1],
				parsedPostCorrectors[2],
				parsedPostCorrectors[3],
				parsedPostCorrectors[4],
				parsedPostCorrectors[5],
				parsedPostCorrectors[6],
				parsedPostCorrectors[7],
				parsedPostCorrectors[8],
				parsedPostCorrectors[9],
				parsedPostCorrectors[10],
				parsedPostCorrectors[11],
				parsedPostCorrectors[12],
				parsedPostCorrectors[13],
				parsedPostCorrectors[14],
				parsedPostCorrectors[15],
				parsedPostCorrectors[16],
				parsedPostCorrectors[17],
				parsedPostCorrectors[18],
				parsedPostCorrectors[19]);

			var setCarouselParametersResult = await _carouselService.SetCarouselParameters(carouselParameters);

			if (setCarouselParametersResult.IsError)
			{
				await Toast.Make($"{setCarouselParametersResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				return;
			}
		}
		catch (Exception ex)
		{
			await Toast.Make($"{ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
		}
	}
}