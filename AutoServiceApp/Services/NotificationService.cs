using AutoServiceApp.Models;

namespace AutoServiceApp.Services;

public class NotificationService
{
    private readonly SmsNotifier _smsNotifier = new();
    private readonly EmailSender _emailSender = new();

    public void NotifyAboutStatus(
        RepairOrder order,
        string type,
        List<string> notifications)
    {
        var phone = order.Customer?.Phone ?? "";
        var email = order.Customer?.Email ?? "";
        var text = $"Order {order.OrderNumber}: new status {order.Status}";

        if (type == "sms")
        {
            _smsNotifier.SendSms(phone, text);
        }
        else if (type == "email")
        {
            _emailSender.Send(email, "Order status", text);
        }
        else
        {
            _smsNotifier.SendSms(phone, text);
            _emailSender.Send(email, "Order status", text);
        }

        notifications.Add($"{DateTime.Now:g}: {type} {text}");
    }
}