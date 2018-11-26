using Godot;
using System;

public class QuestViewer : Control
{
    private QuestManager questManager;
    private VBoxContainer ActiveQuestSection;
    

    public override void _Ready()
    {
        questManager = GetNode("../../game/Viewport/GameManager/QuestManager") as QuestManager;
        ActiveQuestSection = GetNode("MarginContainer/HBoxContainer/LeftPanel/Active/VBox/Scroll/VBox") as VBoxContainer;
        //vBox = GetNode("MarginContainer/VBox") as VBoxContainer;
        AddQuest(questManager.AllQuest[0]);
    }

    public override void _Process(float delta)
    {
        //DisplayTrackedQuests();
    }

    public void DisplayTrackedQuests()
    {
        foreach (var quest in questManager.AllQuest)
        {
            if (!ActiveQuestSection.HasNode(quest.Name))
            {
                AddQuest(quest);
            }
        }
    }

    public void AddQuest(Quest pQuest)
    {
        Control newControl = new Control();
        newControl.SetHSizeFlags(2);
        newControl.RectMinSize = new Vector2(0, 100);

        Panel background = new Panel();
        background.AddStyleboxOverride("Panel", ResourceLoader.Load("res://Assets/ui/Themes/Quest.tres") as StyleBox);
        background.SizeFlagsHorizontal = 2;
        background.SizeFlagsVertical = 2;
        newControl.AddChild(background);

        Label title = new Label();
        title.AddFontOverride("normal", ResourceLoader.Load("res://Assets/Fonts/QuestFont.tres") as Font);
        title.SizeFlagsHorizontal = 2;
        title.SizeFlagsVertical = 2;
        title.Text = pQuest.QuestName;
        newControl.AddChild(title);

        newControl.Name = pQuest.QuestName;

        ActiveQuestSection.AddChild(newControl);

    }

    public void RemoveQuest(Quest pQuest)
    {

    }
}
