using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.XamlHelpers.Behaviors
{
	public class LabelHeigthBehavior : Behavior<Label>
	{
		protected override void OnAttachedTo(Label label)
		{
			base.OnAttachedTo(label);
			label.PropertyChanged += OnSizeChanged;
		}

		protected override void OnDetachingFrom(Label label)
		{
			base.OnDetachingFrom(label);
			label.PropertyChanged -= OnSizeChanged;
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			if (sender is Label label)
			{
				if (string.IsNullOrEmpty(label.Text))
					label.MaximumHeightRequest = 0;				
				else
					label.MaximumHeightRequest = 999;
			}
		}
	}
}
