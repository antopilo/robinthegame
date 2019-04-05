using Godot;
using Newtonsoft.Json;
using System;

public class InventoryItem : MarginContainer
{
    public string ItemName = "ITEM_NAME";
    public string Description = "ITEM_DESCRIPTION";
    public int Amount = 99;
    public string IconPath = "res://icon.png";

    private TextureRect TextureRect;
    private Label CountLabel;
    private ColorRect FocusRect;

    public override void _Ready()
    {
        TextureRect = (TextureRect)GetNode("Icon/Image");
        CountLabel = (Label)GetNode("CountContainer/Count");
        FocusRect = (ColorRect)GetNode("Focus");

        TextureRect.Texture = (Texture)ResourceLoader.Load(IconPath);
        CountLabel.Text = "x" + Amount;
    }

    public void UpdateAmount(int amount)
    {
        Amount = amount;
        CountLabel.Text = "x" + Amount;
    }

    public override void _Process(float delta)
    {
        FocusRect.Visible = this.HasFocus();
    }
}
