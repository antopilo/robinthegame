using Godot;
using System;

public class InventoryNotification : Control
{
    private static VBoxContainer Container;
    private static PackedScene NotificationScene;

    public override void _Ready()
    {
        Container = (VBoxContainer)GetNode("VBoxContainer");
        NotificationScene = (PackedScene)ResourceLoader.Load("res://Content/UI/ItemNotification.tscn");
    }

    public static void SendNotification(ItemData item, int amount)
    {
        var notification = (NotificationScene.Instance()) as ItemNotification;
        notification.ItemName = item.ItemName;
        notification.Amount = amount;
        notification.IconPath = item.IconPath;
        Container.AddChild(notification);
    }
}
