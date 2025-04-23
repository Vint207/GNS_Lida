using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using static Android.Preferences.PreferenceManager;

namespace GNS
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity : MauiAppCompatActivity
	{
		public static Func<Intent,Task> OnNewIntentReceivedHandler;
		public static Action OnPauseHandler;
		public static Action OnResumeHandler;
		public static EventHandler<ActivityResultEventArgs> OnActivityResultHandler;

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);			
		}

		protected override void OnNewIntent(Intent? intent)
		{
			base.OnNewIntent(intent);
			OnNewIntentReceivedHandler?.Invoke(intent);
		}

		protected override void OnResume()
		{
			base.OnResume();
			OnResumeHandler?.Invoke();
		}

		protected override void OnPause()
		{
			base.OnPause();
			OnPauseHandler?.Invoke();
		}

		protected override void OnStop()
		{
			base.OnStop();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void OnRestart()
		{
			base.OnRestart();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			OnActivityResultHandler?.Invoke(null, new ActivityResultEventArgs(true, requestCode, resultCode, data));
		}

		//public static void ClearMainActivityEvents()
		//{
		//	OnResumeHandler = null;
		//	OnPauseHandler = null;
		//	OnActivityResultHandler = null;
		//	OnNewIntentReceivedHandler = null;
		//}
	}
}
