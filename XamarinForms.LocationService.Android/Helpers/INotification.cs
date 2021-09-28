using Android.App;

namespace XamarinForms.LocationService.Droid.Helpers
{
    public interface INotification
    {
        Notification ReturnNotif(string sTitle = "Trial Service", string sMessage = "Your Message");
        void UpdateNotification(string contentText = "");
    }
}