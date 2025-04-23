using CommunityToolkit.Maui.Alerts;
//using ZXing.Mobile;

namespace GNS.Platforms.Android
{
	public class QRScanner
	{
		//MobileBarcodeScanner _scanner;
		//QRScannerOverlay scannerOverlay;
		//public delegate void OnTagDetected(string tag);
		//public OnTagDetected OnTagDetectedHandler;
		//public event EventHandler OnPermissionDeniedHandler;

		public async Task ScanAsync()
		{
			//if(!await CheckPermissions()) return;

			//_scanner = new()
			//{
			//	UseCustomOverlay = true,
			//};

			//scannerOverlay = new QRScannerOverlay();
			//scannerOverlay.ButtonCancelClicked += (sender, e) => StopScan();
			//_scanner.CustomOverlay = scannerOverlay.GetCustomOverlay() as global::Android.Views.View;

			//string scanResult;

			//try
			//{
			//	scanResult = (await _scanner.Scan()).Text;
			//}
			//catch (Exception)
			//{
			//	await Toast.Make("qr-код не отсканирован", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
			//	StopScan();

			//	return;
			//}

			//StopScan();

			//if (string.IsNullOrEmpty(scanResult))
			//{
			//	await Toast.Make("qr-код не отсканирован", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
			//}

			//OnTagDetectedHandler?.Invoke(scanResult);
		}

		public void StopScan()
		{
			//_scanner.Cancel();
		}

		//public async Task<bool> CheckPermissions()
		//{
			//var cameraPermissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

			//if (cameraPermissionStatus is not PermissionStatus.Granted)
			//{
			//	cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();

			//	if (cameraPermissionStatus is not PermissionStatus.Granted || cameraPermissionStatus is PermissionStatus.Denied)
			//	{
			//		OnPermissionDeniedHandler?.Invoke(null, EventArgs.Empty);

			//		return false;
			//	}
			//}

			//return true;
		//}
	}
}
