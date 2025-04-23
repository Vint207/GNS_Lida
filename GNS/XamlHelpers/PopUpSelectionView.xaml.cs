namespace GNS.Services;

public partial class PopUpSelectionView : ContentView
{
	public ListView ListViewOptions 
	{
		get => listViewOptions;
		set
		{
			listViewOptions = value;
		}
	}

	public PopUpSelectionView()
	{
		InitializeComponent();
	}
}