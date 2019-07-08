using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class Inventory : Control
{
    private GridContainer ItemContainer;
    private PackedScene ItemScene;

    private int SelectedItem = 0;

    // Info
    private Label ItemTitleLabel;
    private RichTextLabel ItemDescriptionLabel;

    public override void _Ready()
    {
        // refs
        ItemContainer = (GridContainer)GetNode("MarginContainer/HBoxContainer/Left/VBoxContainer/ScrollContainer/GridContainer");
        ItemScene = (PackedScene)ResourceLoader.Load("res://Content/UI/Item.tscn");

        ItemTitleLabel = (Label)GetNode("MarginContainer/HBoxContainer/Right/VBoxContainer/Label");
        ItemDescriptionLabel = (RichTextLabel)GetNode("MarginContainer/HBoxContainer/Right/VBoxContainer/MarginContainer/VBoxContainer/Panel/MarginContainer/Content");

        // Init.
        UpdateUI();

        
        UpdateInfo();

        
        //SaveInventory();
    }


    public override void _Process(float delta)
    {
        UpdateInfo();
    }

    public void UpdateUI()
    {
        
        foreach (Node child in ItemContainer.GetChildren())
        {
            child.QueueFree();
            ItemTitleLabel.Text = "...";
            ItemDescriptionLabel.Text = "...    ";
        }
        int idx = 0;
        foreach (var data in InventoryManager.Inventory)
        {
            // Create the item and add it to the container.
            InventoryItem item = (InventoryItem)ItemScene.Instance();
            item.Name = item.ItemName = data.ItemName;
            item.Description = data.Description;
            item.Amount = data.Amount;
            item.IconPath = data.IconPath;

            if (idx == 0)
            {
                if(item.IsInsideTree())
                    item.GrabFocus();

                ItemTitleLabel.Text = item.ItemName;
                ItemDescriptionLabel.Text = item.Description;
            }
            
            ItemContainer.AddChild(item);
            idx++;
        }
    }

    private void UpdateInfo()
    {
        foreach (Control child in ItemContainer.GetChildren())
        {
            if (child.HasFocus())
            {
                ItemTitleLabel.Text = ((InventoryItem)child).ItemName;
                ItemDescriptionLabel.Text = ((InventoryItem)child).Description;
            }
        }
    }
}
