using CommunityToolkit.Maui.Core.Extensions;
using ErrorOr;
using GNS.Services;
using Services.Intrefaces;
using System.Collections.ObjectModel;

namespace GNS.Pages.Popup;

public partial class StringOptionsPage : ContentPage
{
	private ObservableCollection<string> _options;
	public ObservableCollection<string> Options
	{
		get => _options;
		set
		{
			_options = value;
			OnPropertyChanged(nameof(Options));
		}
	}

	public event EventHandler<object> ItemSelected;
	public Action OnBackButtonClickHandler { get; set; }
	public Func<Task<ErrorOr<IEnumerable<string>>>> GetElementsCollectionHandler { get; set; }

	public StringOptionsPage()
	{
		InitializeComponent();

		BindingContext = this;
	}

	private void LoadOptionsListAsync()
	{
		Options ??= [];

		Dispatcher.Dispatch(async () =>
		{
			Options?.Clear();
			var optionsResult = await GetElementsCollectionHandler?.Invoke();

			if (optionsResult.IsError)
				return;

			var options = optionsResult.Value?.Select(x => x.ToString());

			if (options is null || !options.Any())
			{
				if (Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync();
				
				return;
			}

			foreach (var option in options)
				Options?.Add(option.ToString()); 				
		});
	}

	private void Options_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		ItemSelected?.Invoke(e, e.CurrentSelection[0]);	
}