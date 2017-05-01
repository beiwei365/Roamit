﻿using QuickShare.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace QuickShare.ToastNotifications
{
    internal static partial class Toaster
    {
        static string[] notDirectlyOpenableExtensions = { ".exe" };

        public static void ShowFileReceiveFinishedNotification(int filesCount, string hostName, Guid guid)
        {
            ClearNotification(guid); //Clear progress notification

            DataStorageProviders.HistoryManager.Open();
            var data = DataStorageProviders.HistoryManager.GetItem(guid);
            DataStorageProviders.HistoryManager.Close();

            string toastXml;

            if (filesCount == 1)
            {
                var fileName = (data.Data as ReceivedFileCollection).Files[0].Name;

                if (notDirectlyOpenableExtensions.Contains(System.IO.Path.GetExtension(fileName)))
                    toastXml = Templates.SingleFileReceivedWithNoOpenFileButton.Replace("{title}", "1 file received")
                                                                               .Replace("{subtitle}", $"from {hostName}")
                                                                               .Replace("{guid}", guid.ToString());
                else
                    toastXml = Templates.SingleFileReceived.Replace("{title}", "1 file received")
                                                           .Replace("{subtitle}", $"from {hostName}")
                                                           .Replace("{guid}", guid.ToString());
            }
            else
            {
                toastXml = Templates.MultipleFilesReceived.Replace("{title}", $"{filesCount} files received")
                                                          .Replace("{subtitle}", $"from {hostName}")
                                                          .Replace("{guid}", guid.ToString());
            }

            var doc = new XmlDocument();
            doc.LoadXml(toastXml);

            var toast = new ToastNotification(doc)
            {
                NotificationMirroring = NotificationMirroring.Disabled,
                Tag = guid.ToString()
            };

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}