using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class InventoryManager : Node
{
    public static List<ItemData> Items = new List<ItemData>();
    private static List<ItemData> inventory = new List<ItemData>();

    public static List<ItemData> Inventory { get => inventory; set => inventory = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadItems();
    }
    
    // Adding item to inventory.
    public static void AddItem(string name, int amount)
    {
        ItemData item = Items.Find(x => x.ItemName == name);

        if(item is null)
        {
            GD.PrintErr("Item not found: " + name);
            Console.Log("Item not found: " + name);
        }
        if (Inventory.Contains(item))
        {
            Inventory.Find(x=> x.ItemName == name).Amount += amount;
        }
        else
        {
            item.Amount = amount;
            Inventory.Add(item);
        }
        
        InventoryNotification.SendNotification(item, amount);
        Root.Utilities.Inventory.UpdateUI();
    }

    public void LoadItems()
    {
        StreamReader writer = new StreamReader("Content/Data/Items.json");

        string file = writer.ReadToEnd();
        writer.Close();

        List<ItemData> loadedItems = JsonConvert.DeserializeObject<List<ItemData>>(file);
        Items = loadedItems;
    }

    public void LoadInventory()
    {
        StreamReader writer = new StreamReader("Saves/inventory.json");

        string file = writer.ReadToEnd();
        writer.Close();

        List<ItemData> loadedItems = JsonConvert.DeserializeObject<List<ItemData>>(file);
        Items = loadedItems;
    }

    public void SaveInventory()
    {
        string json = JsonConvert.SerializeObject(Items, Formatting.Indented);
        StreamWriter file = new StreamWriter("Saves/inventory.json");
        file.WriteLine(json);
        file.Close();
    }
}
