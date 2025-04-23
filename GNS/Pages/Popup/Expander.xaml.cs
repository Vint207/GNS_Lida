namespace GNS.Pages.Popup;

public partial class Expander : ContentView
{
	public IList<IView> ExpanderContent => expanderContent.Children;
	public StackLayout ExpanderBox => expanderBox;
	public double MinimumHeight { get; set; } = 0;
	public bool IsOpened { get; set; }
	public enum ResizeMode
	{ Open, Close, Resize, None}
	private ResizeMode _currentMode;


	public Expander()
	{
		InitializeComponent();
	}

	public void ResizeExpander(ResizeMode resizeMode)
	{
		Dispatcher.Dispatch(async () =>
		{
			switch (resizeMode)
			{
				case ResizeMode.Open:
					var animation = new Animation(h => expanderBox.HeightRequest = h, expanderBox.HeightRequest, expanderContent.Height);
					animation.Commit(expanderBox, "HeightAnimationMax", length: 300, easing: Easing.SinInOut);
					await Task.Delay(200);
					var fadeAnimation = new Animation(h => expanderContent.Opacity = h, expanderContent.Opacity, 1);
					fadeAnimation.Commit(expanderContent, "FadeAnimationMax", length: 200, easing: Easing.SinInOut);
					IsOpened = true;
					break;

				case ResizeMode.Close:
					fadeAnimation = new Animation(h => expanderContent.Opacity = h, expanderContent.Opacity, 0);
					fadeAnimation.Commit(expanderContent, "FadeAnimationMax", length: 200, easing: Easing.SinInOut);					
					await Task.Delay(100);
					animation = new Animation(h => expanderBox.HeightRequest = h, expanderBox.Height, 0);
					animation.Commit(expanderBox, "HeightAnimationMax", length: 300, easing: Easing.SinInOut);
					IsOpened = false;
					break;

				case ResizeMode.Resize:
					if (expanderBox.HeightRequest > 0)
					{
						animation = new Animation(h => expanderBox.HeightRequest = h, expanderBox.Height, expanderContent.Height);
						animation.Commit(expanderBox, "HeightAnimationMax", length: 300, easing: Easing.SinInOut);				
					}
					break;
			}		
		});
	}
}