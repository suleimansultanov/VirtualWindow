namespace NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification
{
    public class PushNotificationDeliveryResult
    {
        public static PushNotificationDeliveryResult Success()
        {
            return new PushNotificationDeliveryResult(DeliveryStatus.Delivered, string.Empty);
        }

        public static PushNotificationDeliveryResult Failure(string error)
        {
            return new PushNotificationDeliveryResult(DeliveryStatus.DeliveryFailed, error);
        }
        
        public DeliveryStatus DeliveryStatus { get; }
        public string Error { get; }

        public PushNotificationDeliveryResult(DeliveryStatus deliveryStatus, string error)
        {
            DeliveryStatus = deliveryStatus;
            Error = error;
        }
    }
}