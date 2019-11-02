using System;

using System.Collections.Generic;

public struct DebugItem
{
    public string name;
    public string value;
    
}

public static class DebugPrinter
{
    private static Dictionary<string, string> Items = new Dictionary<string, string>();

    public static void AddDebugItem(string name, string value)
    {
        if(Items.ContainsKey(name))
        {
            Items[name] = value;
        }
        else
        {
            Items.Add(name, value);
        }
    }

    public static string GetDebugContent()
    {
        string debugContent = "\n";
        foreach(var item in Items)
        {
            debugContent += item.Key + ": " + item.Value + "\n"; 
        }

        return debugContent;
    }
}
