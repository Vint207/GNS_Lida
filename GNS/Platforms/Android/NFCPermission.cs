using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android;
using Android.OS;
using AndroidX.Activity.Result;
using static Android.Preferences.PreferenceManager;
using CommunityToolkit.Maui.Alerts;
using GNS.Platforms.Android;

namespace GNS
{
	public class NFCPermission : Permissions.BasePlatformPermission
	{		
		public bool CheckAndRequest()
		{
			var activity = Platform.CurrentActivity;

			if (activity.PackageManager.HasSystemFeature(PackageManager.FeatureNfc))
			{
				NfcManager nfcManager = activity.GetSystemService(Context.NfcService) as NfcManager;

				if (nfcManager.DefaultAdapter is not null && nfcManager.DefaultAdapter.IsEnabled)
					return true;				
			}

			Intent intent = new(global::Android.Provider.Settings.ActionNfcSettings);
			intent.AddFlags(ActivityFlags.NewTask);
			activity.StartActivityForResult(intent, 1001);
			return false;
		}
	}
}
