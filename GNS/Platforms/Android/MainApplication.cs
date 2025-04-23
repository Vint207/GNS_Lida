using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;

namespace GNS
{
	[Application]
	public class MainApplication : MauiApplication
	{
		public MainApplication(nint handle, JniHandleOwnership ownership) : base(handle, ownership)
		{
			Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) =>
			{
				if (view is Editor)
				{
					handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
				}
			});
			Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
			{
				if (view is Entry)
				{
					handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
				}
			});
			//Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping(nameof(DatePicker), (handler, view) =>
			//{
			//	if (view is DatePicker)
			//	{
			//		handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
			//		Microsoft.Maui.Controls.Application.Current.Resources.TryGetValue("Tertiary", out var primaryColor);
			//	}
			//});
		}

		protected override MauiApp CreateMauiApp()
		{
			return MauiProgram.CreateMauiApp();
		}
	}
}
