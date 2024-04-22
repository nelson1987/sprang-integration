using System.Xml;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

ShowToastNotification("Your notification message here");


void ShowToastNotification(string message)
{
    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
    XmlNodeList textElements = toastXml.GetElementsByTagName("text");
    textElements[0].AppendChild(toastXml.CreateTextNode(message));

    ToastNotification toast = new ToastNotification(toastXml);
    ToastNotificationManager.CreateToastNotifier("YourAppName").Show(toast);
}