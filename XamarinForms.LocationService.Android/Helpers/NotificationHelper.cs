using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using XamarinForms.LocationService.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]
namespace XamarinForms.LocationService.Droid.Helpers
{
    internal class NotificationHelper : INotification
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private static string foregroundChannelId = "9001";
        private static Context context = global::Android.App.Application.Context;

        public void UpdateNotification(string contentText = "")
        {
            var mNotificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                mNotificationManager.CreateNotificationChannel(_CreateNotificationChannel());
            }

            var notification = ReturnNotif("Trial Service", contentText);

            mNotificationManager.Notify(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }
        private NotificationChannel _CreateNotificationChannel()
        {
            NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High);
            notificationChannel.Importance = NotificationImportance.High;
            notificationChannel.EnableLights(true);
            notificationChannel.EnableVibration(true);
            notificationChannel.SetShowBadge(true);
            notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300 });
            return notificationChannel;
        }


        public Notification ReturnNotif(string sTitle = "Trial Service", string sMessage = "Your Message")
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle(sTitle)
                .SetContentText(sMessage)
                .SetSmallIcon(Resource.Drawable.location)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {


                var notifManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notifManager != null)
                {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notifManager.CreateNotificationChannel(_CreateNotificationChannel());
                }
            }

            return notifBuilder.Build();
        }
    }
}