using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.XamlHelpers.Behaviors
{
	public class ElementHeigthWhenUnvisibleBehavior : Behavior<VisualElement>
	{
		protected override void OnAttachedTo(VisualElement label)
		{
			base.OnAttachedTo(label);
			label.PropertyChanged += OnSizeChanged;
		}

		protected override void OnDetachingFrom(VisualElement label)
		{
			base.OnDetachingFrom(label);
			label.PropertyChanged -= OnSizeChanged;
		}

		private async void OnSizeChanged(object sender, EventArgs e)
		{
			await Task.Delay(50);
			
			if (sender is VisualElement element)
			{
				if (!element.IsVisible)
					element.MaximumHeightRequest = 0;				
				else
					element.MaximumHeightRequest = 9999;
			}
		}
	}
}
