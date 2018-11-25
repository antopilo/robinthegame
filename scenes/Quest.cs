using Godot;
using System;

public enum QuestType { Retreive, GoTo, Collect, TalkTo, Multiple }
public enum QuestState { Inactive, Active, Completed }

public class Quest : Node
{
    #region Properties
    [Export] public string QuestName = "Unknown Quest";
    [Export(PropertyHint.Range,"0,50")] public int Experience;
    [Export] public QuestType Type = QuestType.GoTo;

    [Export] public NodePath Destination;
    [Export] public NodePath Object;
    [Export] public NodePath Receiver;
    #endregion

    public QuestState State = QuestState.Active;

    private GameController GameController;
    private QuestManager QuestManager;

    public override void _Ready()
    {
        QuestManager = GetParent() as QuestManager;
        GameController = GetParent().GetParent() as GameController;
    }

    public void CheckStatus()
    {
        if (QuestManager.CurrentQuest == this && CheckCompleted() == true)
        {
            State = QuestState.Completed;
            QuestManager.TrackedQuest.Remove(this);
            QuestManager.CompletedQuest.Add(this);
            QuestManager.NextQuest();
        }

        else if (QuestManager.CurrentQuest == this && !CheckCompleted())
            this.State = QuestState.Active;
        else if (QuestManager.CurrentQuest != this && !CheckCompleted())
            this.State = QuestState.Inactive;
        else if (QuestManager.CurrentQuest != this && CheckCompleted())
            this.State = QuestState.Completed;
    }

    public bool CheckCompleted()
    {
        if (QuestManager.CurrentQuest.State == QuestState.Completed)
            return true;

        switch (Type)
        {
            case QuestType.Retreive:
                if (GameController.PlayerHas(GetNode(Object) as Node2D))
                    return true;
                else
                    return false;
                break;
            case QuestType.GoTo:
                GD.Print(GetNode(Destination).Name);
                if (GameController.CurrentRoom == GetNode(Destination))
                    return true;
                else
                    return false;
                break;
            case QuestType.Collect:
                if (GameController.PlayerHas(GetNode(Object) as Node2D))
                    return true;
                else
                    return false;
                break;
        }

        return false;
    }
}
