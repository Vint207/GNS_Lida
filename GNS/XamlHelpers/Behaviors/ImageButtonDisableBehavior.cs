using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.XamlHelpers.Behaviors
{
	public class ImageButtonDisableBehavior : Behavior<ImageButton>
	{
		public int Duration { get; set; } = 3000;

		protected override void OnAttachedTo(ImageButton button)
		{
			base.OnAttachedTo(button);
			button.Clicked += OnButtonClicked;
		}

		protected override void OnDetachingFrom(ImageButton button)
		{
			base.OnDetachingFrom(button);
			button.Clicked -= OnButtonClicked;
		}

		private async void OnButtonClicked(object sender, EventArgs e)
		{
			if (sender is ImageButton button)
			{
				button.IsEnabled = false;
				await Task.Delay(Duration);
				button.IsEnabled = true;
			}
		}
	}
}
