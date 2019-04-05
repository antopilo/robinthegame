using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemData
{
    public string ItemName = "ITEM_NAME";
    public string Description = "ITEM_DESCRIPTION";
    public int Amount = 99;
    public string IconPath = "res://icon.png";

    public ItemData(string name, string description, int amount, string icon)
    {
        ItemName = name;
        Description = description;
        Amount = amount;
        IconPath = icon;
    }
}
