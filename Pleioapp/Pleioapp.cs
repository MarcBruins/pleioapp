using System;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace Pleioapp
{
	public class App : Application
	{
		public MainPage RootPage;
		public Site mainSite;
		public Site currentSite;
		public Group currentGroup;
		public AuthToken authToken;

		public WebService webService;
		public IPushService pushService;
		public ISSOService ssoService;
		public LoginService loginService;

		private bool isRefreshingToken = false;

		public App ()
		{
			mainSite = new Site ();
			mainSite.name = "Pleio hoofdniveau";
			mainSite.url = Constants.Url;
			currentSite = mainSite;

			webService = new WebService ();
			loginService = new LoginService ();

			pushService = DependencyService.Get<IPushService> ();
			ssoService = DependencyService.Get<ISSOService> ();

			RootPage = new MainPage ();
			MainPage = RootPage;

			LoadAccessToken ();
			pushService.SetBadgeNumber (0);

			MessagingCenter.Subscribe<Xamarin.Forms.Application> (App.Current, "refresh_access_token", async(sender) => {
				await RefreshToken();
			});
				
			MessagingCenter.Subscribe<Xamarin.Forms.Application> (App.Current, "login", async(sender) => {
				ShowLogin();
			});

			MessagingCenter.Subscribe<Xamarin.Forms.Application> (App.Current, "login_succesful", async(sender) => {
				RefreshPushToken();
			});
		}
			
		private void LoadAccessToken()
		{
			var token = DependencyService.Get<ITokenStore> ().getToken();

			if (token != null) {
				authToken = token;
				RefreshPushToken ();
				MessagingCenter.Send<Xamarin.Forms.Application> (App.Current, "refresh_menu");
			} else {
				ShowLogin ();
			}
		}

		private async void RefreshPushToken()
		{
            await Task.Run(() => {
                if (pushService.GetToken() == null)
                {
                    pushService.RequestToken();
                }
                else {
                    //android will register through an intentservice
                    if (Device.OS == TargetPlatform.Android)
                        return;

                    //iOS and other platforms registration
                    pushService.RegisterToken();
                }
            });
		}

		private async void ShowLogin()
		{
			await RootPage.Navigation.PushModalAsync (new LoginPage ());
		}

		public async Task<bool> RefreshToken()
		{
			if (isRefreshingToken) {
				return true;
			} else {
				isRefreshingToken = true;
			}

			var store = DependencyService.Get<ITokenStore> ();
			var new_token = await loginService.RefreshToken (store.getToken());
			isRefreshingToken = false;

			if (new_token != null) {
				authToken = new_token;
				currentSite = mainSite;
				webService = new WebService ();

				store.saveToken (new_token);

				MessagingCenter.Send<Xamarin.Forms.Application> (App.Current, "login_succesful");
				MessagingCenter.Send<Xamarin.Forms.Application> (App.Current, "refresh_menu");
				return true;
			} else {
				ShowLogin ();
				return false;
			}
		}
			
		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			MessagingCenter.Send<Xamarin.Forms.Application> (App.Current, "refresh_menu");
			pushService.SetBadgeNumber (0);
		}
	}
}

