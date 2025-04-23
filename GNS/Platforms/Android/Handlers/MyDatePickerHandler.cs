using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using CommunityToolkit.Maui.Core.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;



namespace GNS.Platforms.Android.Handlers;

public class MyDatePickerHandler : DatePickerHandler
{
	protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
	{
		var dialog = new DatePickerDialog(Context!, Resource.Style.MyDatePickerDialogTheme, (o, e) =>
		{
			if (VirtualView != null)
			{
				VirtualView.Date = e.Date;
			}
		}, year, month, day);

		return dialog;
	}		
}

