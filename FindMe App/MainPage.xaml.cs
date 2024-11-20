namespace FindMe_App;

public partial class MainPage : ContentPage
{
    private string _baseUrl = "https://bing.com/maps/default.aspx?cp=";
    public string UserName { get; set; }
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async Task ShareLocation()
    {
        UserName = UsernameEntry.Text;

        var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
        var location = await Geolocation.GetLocationAsync(locationRequest);

        await Share.RequestAsync(new ShareTextRequest
        {
            Subject = "Find Me!",
            Title = "Find Me!",
            Text = $" {UserName} is sharing their location with you.",
            Uri = $"{_baseUrl}{location.Latitude},{location.Longitude}"
        });
    }

    private async void OnFindMeClicked(object sender, EventArgs e)
    {
        var permissions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (permissions == PermissionStatus.Granted)
        {
            await ShareLocation();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Permissions Error",
                "You have not granted permission to access your location",
                "OK");
            
            var requested = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            
            if (requested == PermissionStatus.Granted)
            {
                await ShareLocation();
            }
            else
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                {
                    await App.Current.MainPage.DisplayAlert("Location Required",
                        "Location is required to share your location. Please enable location in settings.",
                        "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Location Required",
                        "Location is required to share your location. We'll ask again next time.",
                        "OK");
                }
            }
        }
    }
}