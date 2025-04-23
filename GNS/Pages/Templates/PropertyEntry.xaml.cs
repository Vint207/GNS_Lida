

namespace GNS.Pages.Templates;

public partial class PropertyEntry : ContentView
{
	#region Properties
	public Label Label => label;
	public Editor TextEntry => textEntry;
	public DatePicker DateEntry => dateEntry;
	public ImageButton ListButton => listButton;
	public Editor ListText => listText;
	public Keyboard TextEntryKeabord
	{
		get => textEntry.Keyboard;
		set
		{
			textEntry.Keyboard = value;
		}
	}
	public string PropertyTitle
	{
		get => label.Text;
		set
		{
			label.Text = value;
			OnPropertyChanged();
		}
	}

	public static readonly BindableProperty DateProperty =
		BindableProperty.Create(
			propertyName: nameof(Date),
			returnType: typeof(DateTime),
			declaringType: typeof(PropertyEntry),
			defaultValue: null,
			defaultBindingMode: BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var control = (PropertyEntry)bindable;
				control.dateEntry.Date = (DateTime)newvalue;
			});
	public DateTime Date
	{
		get => (DateTime)GetValue(DateProperty);
		set => SetValue(DateProperty, value);
	}

	public static readonly BindableProperty EditorTextProperty =
		BindableProperty.Create(
			propertyName: nameof(EditorText),
			returnType: typeof(string),
			declaringType: typeof(PropertyEntry),
			defaultValue: null,
			defaultBindingMode: BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var control = (PropertyEntry)bindable;
				control.textEntry.Text = (string)newvalue;
			});
	public string EditorText
	{
		get => (string)GetValue(EditorTextProperty);
		set => SetValue(EditorTextProperty, value);
	}

	public static readonly BindableProperty ListEditorTextProperty =
		BindableProperty.Create(
			propertyName: nameof(ListEditorText),
			returnType: typeof(string),
			declaringType: typeof(PropertyEntry),
			defaultValue: null,
			defaultBindingMode: BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var control = (PropertyEntry)bindable;
				control.listText.Text = (string)newvalue;
			});

	public string ListEditorText
	{
		get => (string)GetValue(ListEditorTextProperty);
		set => SetValue(ListEditorTextProperty, value);
	}
	public bool IsDateField
	{
		get => dateEntry.IsVisible;
		set
		{
			dateEntry.IsVisible = value;
		}
	}
	public bool IsTextField
	{
		get => textEntry.IsVisible;
		set
		{
			textEntry.IsVisible = value;
		}
	}
	public bool IsListField
	{
		get => listText.IsVisible;
		set
		{
			listText.IsVisible = value;
			listButton.IsVisible = value;
		}
	}

	public event EventHandler<EventArgs> OnEntryClickHandler; 
	#endregion

	public PropertyEntry()
	{
		InitializeComponent();
	}


	private async void ListButton_Clicked(object sender, EventArgs e)
	{
		OnEntryClickHandler?.Invoke(this, e);
		await Task.Delay(3000);
	}


	private void Frame_SizeChanged(object sender, EventArgs e)
	{
		if (sender is Border frame) 
		{
			double cleanFrameWidth = frame.Width - frame.Padding.Left - frame.Padding.Right - 8;
			label.WidthRequest = cleanFrameWidth / 2;
			grid_property_column_1.Width = label.WidthRequest;
			grid_property_column_2.Width = cleanFrameWidth - grid_property_column_1.Width.Value;
			textEntry.WidthRequest = grid_property_column_2.Width.Value;
			listButton.WidthRequest = grid_property_column_2.Width.Value;
		}
	}


	private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
	{
		EditorText = e.NewTextValue;
	}


	private void OnListEditorTextChanged(object sender, TextChangedEventArgs e)
	{
		ListEditorText = e.NewTextValue;
	}


	private void DateEntry_DateSelected(object sender, DateChangedEventArgs e)
	{
		Date = e.NewDate;
	}


	private void DateEntry_Focused(object sender, FocusEventArgs e)
	{

	}
}