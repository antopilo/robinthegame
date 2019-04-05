using Godot;
using System;
using System.Collections.Generic;

public class QuestManager : Node2D
{
    [Export]public NodePath CurrentQuestPath;
    public Quest CurrentQuest;

    public List<Quest> TrackedQuest = new List<Quest>();
    public List<Quest> CompletedQuest = new List<Quest>();
    public List<Quest> UntrackedQuest = new List<Quest>();
    public List<Quest> AllQuest = new List<Quest>();

    public override void _Ready()
    {
        CurrentQuest = GetNode(CurrentQuestPath) as Quest;

        TrackedQuest.Add(GetNode("GetKey") as Quest);
        TrackedQuest.Add(GetNode("OpenDoor") as Quest);

        AllQuest.Add(GetNode("Next") as Quest);
        AllQuest.Add(GetNode("GetKey") as Quest);
        AllQuest.Add(GetNode("OpenDoor") as Quest);
    }
    public override void _Process(float delta)
    {
        if(CurrentQuest != null)
            CurrentQuest.CheckStatus();

        if(CurrentQuest.State == QuestState.Completed && TrackedQuest.Count > 0)
        {
            CurrentQuest = TrackedQuest[0];
        }

    }

    public void NextQuest()
    {
        if (TrackedQuest.Count > 0)
            CurrentQuest = TrackedQuest[0];
    }

    public void NewQuest(string pQuestName, int pExperience, QuestType pType)
    {

    }
}
