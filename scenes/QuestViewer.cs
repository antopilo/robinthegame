using Godot;
using System;

public class QuestViewer : Control
{
    private QuestManager questManager;
    private VBoxContainer vBox;
    
    public override void _Ready()
    {
        questManager = GetNode("../../game/Viewport/GameManager/QuestManager") as QuestManager;
        vBox = GetNode("MarginContainer/VBox") as VBoxContainer;
    }

    public override void _Process(float delta)
    {
        DisplayTrackedQuests();
    }

    public void DisplayTrackedQuests()
    {
        foreach (var quest in questManager.AllQuest)
        {
            if (!vBox.HasNode(quest.QuestName))
            {
                RichTextLabel newLabel = new RichTextLabel();
                newLabel.Name = quest.QuestName;
                newLabel.BbcodeEnabled = true;
                newLabel.RectMinSize = new Vector2(0,64);
                vBox.AddChild(newLabel);
                (vBox.GetNode(quest.QuestName) as RichTextLabel).BbcodeText = "[b]" + quest.QuestName + ": " + quest.State + "[/b]";
                //newLabel.PushFont(ResourceLoader.Load("res://Assets/Fonts/QuestFont.tres") as Font);
                newLabel.AddFontOverride("normal_font", ResourceLoader.Load("res://Assets/Fonts/QuestFont.tres") as Font);
            }
            else
            {
                if (quest.State == QuestState.Active)
                    (vBox.GetNode(quest.QuestName) as RichTextLabel).BbcodeText = "[font=res://Assets/Fonts/QuestFont.tres]" + 
                        quest.QuestName + ": " + quest.State + "[/font]";


                else if(quest.State == QuestState.Completed)
                    (vBox.GetNode(quest.QuestName) as RichTextLabel).BbcodeText = "[font=res://Assets/Fonts/QuestFont.tres][s][color=green]" + quest.QuestName + ": " + quest.State + "[/color][/s][/font]";
                else
                    (vBox.GetNode(quest.QuestName) as RichTextLabel).BbcodeText = "[b]" + quest.QuestName + ": " + quest.State + "[/b]";
            }
        }
    }
}
