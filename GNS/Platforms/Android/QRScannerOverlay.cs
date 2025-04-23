using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Platform;

namespace GNS.Platforms.Android
{
	public class QRScannerOverlay
	{
		public event EventHandler ButtonCancelClicked;

		public object GetCustomOverlay()
		{
			var context = global::Android.App.Application.Context;
			var customOverlay = new LinearLayout(context)
			{
				Orientation = global::Android.Widget.Orientation.Vertical,
				LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
																 ViewGroup.LayoutParams.MatchParent)
			};

			var cancelButton = new global::Android.Widget.Button(context)
			{
				Text = "Назад",				
				TextSize = 14
			};

			LinearLayout.LayoutParams layoutParameters = new(ViewGroup.LayoutParams.MatchParent,
															 ViewGroup.LayoutParams.WrapContent);
			layoutParameters.SetMargins(85, 85, 85, 85);
			cancelButton.LayoutParameters = layoutParameters;

			var background = new GradientDrawable();
			background.SetShape(ShapeType.Rectangle);

			if (Application.Current.Resources.TryGetValue("PrimaryDark", out var primaryColor))
			{
				if (primaryColor is Color color)
				{
					background.SetColor(global::Android.Graphics.Color.ParseColor(color.ToHex())); 
				} 
			}
			background.SetCornerRadius(25f);
			cancelButton.SetBackground(background);
			cancelButton.TransformationMethod = null;

			cancelButton.Click += (sender, e) =>
			{
				ButtonCancelClicked?.Invoke(sender, e);
			};

			var spaceView = new global::Android.Views.View(context)
			{
				LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0, 1)
			};

			customOverlay.AddView(spaceView);
			customOverlay.AddView(cancelButton);

			return customOverlay;
		}
	}
}
