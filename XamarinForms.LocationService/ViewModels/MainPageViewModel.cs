using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Utils;

namespace XamarinForms.LocationService.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region vars
        private int latitude;
        private int longitude;
        public string userMessage;
        public bool startEnabled;
        public bool stopEnabled;
        #endregion vars

        #region properties
        public int Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }
        public int Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }
        public string UserMessage
        {
            get => userMessage;
            set => SetProperty(ref userMessage, value);
        }
        public bool StartEnabled
        {
            get => startEnabled;
            set => SetProperty(ref startEnabled, value);
        }
        public bool StopEnabled
        {
            get => stopEnabled;
            set => SetProperty(ref stopEnabled, value);
        }
        #endregion properties

        #region commands
        public Command StartCommand { get; }
        public Command EndCommand { get; }
        #endregion commands

        readonly ILocationConsent locationConsent;

        public MainPageViewModel()
        {
            locationConsent = DependencyService.Get<ILocationConsent>();
            StartCommand = new Command(() => OnStartClick());
            EndCommand = new Command(() => OnStopClick());
            HandleReceivedMessages();
            //locationConsent.GetLocationConsent();
            StartEnabled = true;
            StopEnabled = false;
            ValidateStatus();
        }

        public void OnStartClick()
        {
            Start();
        }

        public void OnStopClick()
        {
            var message = new StopServiceMessage();
            MessagingCenter.Send(message, "ServiceStopped");
            UserMessage = "Trial Service has been stopped!";
            SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "0");
            StartEnabled = true;
            StopEnabled = false;
        }

        void ValidateStatus() 
        {
            var status = SecureStorage.GetAsync(Constants.SERVICE_STATUS_KEY).Result;
            if (status != null && status.Equals("1")) 
            {
                Start();
            }
        }

        void Start() 
        {
            var message = new StartServiceMessage();
            MessagingCenter.Send(message, "ServiceStarted");
            UserMessage = "Trial Service has been started!";
            SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "1");
            StartEnabled = false;
            StopEnabled = true;
        }
        System.DateTime prevDateTime;
        void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<LocationMessage>(this, "Location", message => {
                Device.BeginInvokeOnMainThread(() => {
                    Latitude = message.Latitude;
                    Longitude = message.Longitude;
                    System.DateTime dateTime = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, (int)message.Latitude, (int)message.Longitude, 0);
                    if((prevDateTime-dateTime).TotalMinutes > 1)
                    {
                        UserMessage += (prevDateTime - dateTime).TotalMinutes.ToString() + " ";
                    }
                    prevDateTime = dateTime;
                    
                });
            });
            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message => {
                Device.BeginInvokeOnMainThread(() => {
                    UserMessage = "Trial Service has been stopped!";
                });
            });
            MessagingCenter.Subscribe<LocationErrorMessage>(this, "LocationError", message => {
                Device.BeginInvokeOnMainThread(() => {
                    UserMessage = "There was an error updating timestamp!";
                });
            });
        }
    }
}
