using Android.App;
using ArbolesMAUI.DB.ObjectManagers;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Authored by Jared Chan
 **/

namespace ArbolesMAUI.ViewModels
{
    public static class PushNotifUtil
    {
        /// <summary>
        /// Creates a new local push notification based on an asscoiated report/pin that's being added to the map
        /// 
        /// Refer to https://github.com/thudugala/Plugin.LocalNotification for usage
        /// </summary>
        /// <param name="report"></param>
        public static void AddPushNotification(ReportManager report)
        {
            var request = new NotificationRequest
            {
                NotificationId = new Random().Next(),
                Title = "New Tree Uploaded",
                Description = "A new flowering tree location has been added to the map. Click here to view it.",
                CategoryType = NotificationCategoryType.Recommendation,
                ReturningData = report.Location.Latitude.ToString() + "," + report.Location.Longitude.ToString(),
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now,
                    NotifyAutoCancelTime = DateTime.Now.AddHours(2)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }
    }
}
