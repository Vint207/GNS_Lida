using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.Services
{
    public class PopUpService
    {
        public static async Task<string?> ExecutePopUp(ContentPage page, IEnumerable<string> options, string missClickValue)
        {
			PopUpSelection popUpSelection = new(options)
			{
				ResultAfterNearTouch = missClickValue,
			};
			return (await page.ShowPopupAsync(popUpSelection)).ToString();
		}
	}
}
