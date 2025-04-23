
namespace GNS.Pages.Popup;

public partial class FlyoutPanel : ContentView
{
	public Grid FlyoutMain => FlyoutBox;
	public IList<IView> FlyoutMainContent => FlyoutBoxContent.Children;
	public Action OnFlyoutOpenHandler { get; set; }
	public Action OnFlyoutCloseHandler { get; set; }

	public static readonly BindableProperty IsOpenedProperty =
		BindableProperty.Create(
			propertyName: nameof(IsOpenedProperty),
			returnType: typeof(bool),
			declaringType: typeof(FlyoutPanel),
			defaultValue: false,
			defaultBindingMode: BindingMode.TwoWay);
	public bool IsOpened
	{
		get => (bool)GetValue(IsOpenedProperty);
		set => SetValue(IsOpenedProperty, value);
	}

	public FlyoutPanel()
	{
		InitializeComponent();
	}

	public async void ToggleFlyout()
	{
		if (FlyoutBox.TranslationY > 0)
			await OpenFlyout();
		else
			await CloseFlyout();	
	}

	public async Task OpenFlyout()
	{
		OnFlyoutOpenHandler?.Invoke();
		backgroundFader.IsVisible = true;
		await backgroundFader.FadeTo(0.8, 100);
		await FlyoutBox.TranslateTo(0, 0, 200, Easing.SinInOut);
		IsOpened = true;
	}

	public async Task CloseFlyout()
	{
		if (backgroundFader.Opacity > 0 || FlyoutBox.TranslationY != FlyoutBoxContent.Height)
		{
			OnFlyoutCloseHandler?.Invoke();
			await backgroundFader.FadeTo(0, 100);
			await FlyoutBox.TranslateTo(0, FlyoutBoxContent.Height, 250, Easing.SinInOut);
			backgroundFader.IsVisible = false;
			IsOpened = false;
		}
	}

	private async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
	{
		switch (e.StatusType)
		{
			case GestureStatus.Running:
				double translationY = Math.Max(FlyoutBox.TranslationY + e.TotalY, 0);
				double opacity = Math.Max(backgroundFader.Opacity + e.TotalY, 0);
				FlyoutBox.TranslationY = translationY;
				break;

			case GestureStatus.Completed:
				if (FlyoutBox.TranslationY > FlyoutBox.Height - 250)
					await CloseFlyout();
				else
					await OpenFlyout();
				break;
		}
	}

	private async void BackgroundFader_Clicked(object sender, EventArgs e)
	{		
		//await CloseFlyout();		
	}
}