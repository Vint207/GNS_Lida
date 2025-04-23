#if ANDROID
using GNS.Platforms.Android;
#endif
using CommunityToolkit.Maui.Alerts;

namespace GNS.Pages.Templates;

public partial class NfcScanView : ContentView
{
	#region Свойства
	public Action<string> OnScanSuccessHandler { get; set; }
	public Action OnBackButtonClickHandler { get; set; }

	public string Nfc { get; set; }

	private bool _nfcCheckPermission;
	private bool _nfcTimerWorkPermission;

	public NfcScanView()
	{
		InitializeComponent();
	}
	#endregion

	#region Методы
	private void BtnBack_Clicked(object sender, EventArgs e)
	{
		OnBackButtonClickHandler?.Invoke();
		_nfcCheckPermission = false;
		_nfcTimerWorkPermission = false;
	}

	public void OnAppearing()
	{
		_nfcCheckPermission = true;
		_nfcTimerWorkPermission = true;
#if ANDROID
		NFCScanner.OnNfcTagDetected += HandleNFCTagDetected;
#endif
		Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
		{
			StartPermissionChecking(_nfcCheckPermission);

			return _nfcTimerWorkPermission;
		});
		App.OnResumeHandler += () => _nfcCheckPermission = true;
		App.OnPauseHandler += () => _nfcCheckPermission = false;
	}

	public void OnDisappearing()
	{
		_nfcTimerWorkPermission = false;
#if ANDROID
		NFCScanner.OnNfcTagDetected -= HandleNFCTagDetected;
#endif
		App.OnResumeHandler -= () => _nfcCheckPermission = true;
		App.OnPauseHandler -= () => _nfcCheckPermission = false;
	}

	private void StartPermissionChecking(bool status)
	{
#if ANDROID
		CheckNFCPermission();
#endif
	}

#if ANDROID
	private void HandleNFCTagDetected(NfcEventArgs e)
	{
		// инверсия nfc-строки 
		string nfc = e.TagData.ToString();
		string invertedNFC = string.Empty;

		//for (int i = nfc.Length - 2; i >= 0; i-=2) 
		//{
		//	invertedNFC += nfc[i..(i+2)];
		//}

		Nfc = nfc;
		OnScanSuccessHandler?.Invoke(nfc);
		//OnScanSuccessHandler = null;
	}
#endif

#if ANDROID
	private bool CheckNFCPermission()
	{
		return new NFCPermission().CheckAndRequest();
	}
#endif
	#endregion
}