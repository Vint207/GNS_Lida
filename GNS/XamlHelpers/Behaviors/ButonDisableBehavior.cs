using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.XamlHelpers.Behaviors
{
	public class ButonDisableBehavior : Behavior<Button>
	{
		public int Duration { get; set; } = 3000;

		protected override void OnAttachedTo(Button button)
		{
			base.OnAttachedTo(button);
			button.Clicked += OnButtonClicked;
		}

		protected override void OnDetachingFrom(Button button)
		{
			base.OnDetachingFrom(button);
			button.Clicked -= OnButtonClicked;
		}

		private async void OnButtonClicked(object sender, EventArgs e)
		{
			if (sender is Button button)
			{
				button.IsEnabled = false;
				await Task.Delay(Duration);
				button.IsEnabled = true;
			}
		}
	}
}
