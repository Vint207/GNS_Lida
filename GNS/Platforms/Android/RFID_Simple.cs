using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using CommunityToolkit.Maui.Alerts;
using System.Text;

namespace GNS.Platforms.Android
{
	public class RFID_Simple
	{
		public async static Task<string> GetSNCRecord(Intent intent)
		{
			try
			{
				var rfidMsg = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);

				if (rfidMsg is null)
				{
					await Toast.Make("nfc-метка не отсканирована", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();

					return string.Empty;
				}

				foreach (var msg in rfidMsg)
				{
					var ndefMessage = (NdefMessage)msg;
					var records = ndefMessage.GetRecords();

					if (records is null) continue;

					foreach (var r in records)
					{
						var payload = r.GetPayload();

						if (payload is null || payload.Length == 0) continue;

						var text = Encoding.UTF8.GetString(payload, 1, payload.Length - 1);

						if (text.StartsWith("en"))
						{
							text = text.Substring(2);
						}

						return text;
					}
				}
			}
			catch (Exception e)
			{
				await Toast.Make("nfc-метка не отсканирована", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
			}

			return string.Empty;
		}

		public async static Task<string> GetRfID(Intent intent)
		{
			string result = string.Empty;

			try
			{
				var myTag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);

				var id = myTag.GetId();

				result = BitConverter.ToString(id);
				result = result.Replace("-", string.Empty);
			}
			catch (Exception e)
			{
				await Toast.Make("nfc-метка не отсканирована", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
			}

			return result;
		}

		public static void WriteData(Intent intent, string txtstring)
		{
			try
			{
				var myTag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);

				var ndef = Ndef.Get(myTag);

				if (ndef is null || !ndef.IsWritable) { return; }

				var message = new NdefMessage([NdefRecord.CreateTextRecord("en", txtstring)]);

				ndef.Connect();
				ndef.WriteNdefMessage(message);
				ndef.Close();
			}
			catch (Exception e)
			{
				//AndroidDebug.Log("error : " + e.Message);
			}
		}
	}
}
