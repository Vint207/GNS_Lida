using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using CommunityToolkit.Maui.Alerts;

namespace GNS.Platforms.Android
{
	public class NfcEventArgs : EventArgs
	{
		public string TagData { get; set; }
	}

	public class NFCScanner 
	{
		static NfcAdapter? _nfcAdapter = null;
		static MainActivity? _activity = (MainActivity)Platform.CurrentActivity;

		static PendingIntent _pendingIntent = null;
		static IntentFilter[] _intentFilters = null;
		static string[][] _techList = null;

		static public Action<NfcEventArgs> OnNfcTagDetected {  get; set; }

		static NFCScanner()
		{
			ConfigureNFC();
			MainActivity.OnNewIntentReceivedHandler += OnNewIntentRecieved;			
			MainActivity.OnResumeHandler += StartAdapter;
			MainActivity.OnPauseHandler += StopAdapter;
			StartAdapter();
		}

		public static void ConfigureNFC()
		{
			_nfcAdapter = NfcAdapter.GetDefaultAdapter(_activity);

			Intent intent = new Intent(_activity, _activity.GetType()).AddFlags(ActivityFlags.SingleTop);

			var _intentFilters = new[]
			{
				new IntentFilter(NfcAdapter.ActionTagDiscovered),
				new IntentFilter(NfcAdapter.ActionNdefDiscovered),
				new IntentFilter(NfcAdapter.ActionTechDiscovered)
			};

			_techList =
			[
				[nameof(NfcA)],
				[nameof(NfcB)],
				[nameof(NfcF)],
				[nameof(NfcV)],
				[nameof(IsoDep)],
				[nameof(NdefFormatable)],
				[nameof(MifareClassic)],
				[nameof(MifareUltralight)],
			];

			try
			{
				_pendingIntent = PendingIntent.GetActivity(_activity, 0, intent, PendingIntentFlags.Mutable);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static async Task OnNewIntentRecieved(Intent intent)
		{
			if (intent.Action == NfcAdapter.ActionTechDiscovered
				|| intent.Action == NfcAdapter.ActionNdefDiscovered
				|| intent.Action == NfcAdapter.ActionTagDiscovered)
			{
				string rfid = await RFID_Simple.GetRfID(intent);
				OnNfcTagDetected?.Invoke(new NfcEventArgs { TagData = rfid });
			}
		}

		public static void StartAdapter()
		{			
			MainThread.BeginInvokeOnMainThread(() =>
			{
				try
				{
					_nfcAdapter?.EnableForegroundDispatch(_activity, _pendingIntent, _intentFilters, null);
				}
				catch (Exception)
				{
					throw;
				}			
			});
		}

		public static void StopAdapter()
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				_nfcAdapter?.DisableForegroundDispatch(_activity);
			});
		}
	}
}
