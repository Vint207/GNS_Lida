using GNS.Pages;
using Services;
using Services.Intrefaces;

namespace GNS
{
    public partial class MainPage : ContentPage
	{

		public MainPage(ILoginService loginService, User user)
		{
			InitializeComponent();
		}		
	}
}
