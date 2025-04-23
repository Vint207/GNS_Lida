using CommunityToolkit.Maui.Views;
using GNS.Pages.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.Services
{
	public class PopUpSelection : Popup
	{
		public object ResultAfterNearTouch
		{
			get => ResultWhenUserTapsOutsideOfPopup;
			set { ResultWhenUserTapsOutsideOfPopup = value; }
		}

		public PopUpSelection(IEnumerable<string> options)
		{
			var contentView = new PopUpSelectionView();

			Content = contentView;
			contentView.ListViewOptions.ItemsSource = options;
			contentView.ListViewOptions.ItemSelected += (sender, e) =>
			{
				Close(e.SelectedItem);
			};

			Color = Color.FromRgba(0,0,0,0);
			Size = new(200, Math.Clamp(options.Count() * 50, 50, 400));
		}
	}
}
