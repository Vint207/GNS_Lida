using CommunityToolkit.Maui.Alerts;

namespace GNS.Pages.Templates;

public partial class SerialNumberSearchView : ContentView
{
	public Action OnButtonSearchClickHandler { get; set; }

	private string _serialNumber;
	public string SerialNumber
	{
		get => _serialNumber;
		set
		{
			_serialNumber = value?.ToLowerInvariant();
			OnPropertyChanged();
		}
	}

	public SerialNumberSearchView()
	{
		InitializeComponent();

		SerialNumberInputView.TextEntry.SetBinding(
			Editor.TextProperty, 
			new Binding(nameof(SerialNumber), BindingMode.TwoWay, source: this));
	}

	private async void ButtonSerialNumberSearchFlyout_Clicked(object sender, EventArgs e)
	{
		OnButtonSearchClickHandler?.Invoke();
	}
}