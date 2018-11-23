using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

class QuestManager : Node2D
{
    [Export] public Quest CurrentQuest;

    public List<Quest> TrackedQuest = new List<Quest>();
    public List<Quest> CompletedQuest = new List<Quest>();
    public List<Quest> UntrackedQuest = new List<Quest>();

    
}

