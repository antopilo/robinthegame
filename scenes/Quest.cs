using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public enum QuestType { Retreive, GoTo, Collect }
public enum QuestState { Inactive, Active, Completed}

public class Quest
{
    #region Properties
    public string Name = "Unkown Quest";
    public int Experience;
    public QuestType Type = QuestType.Collect;
    [Export] public Level Destination;
    [Export] public Node2D Object;
    [Export] public Node2D Receiver;

    public QuestState State { get; private set; } 
    #endregion

    private GameController GameController;
    private QuestManager QuestManager;

    #region Errors
    private const string ERR_NAME_NULL = "ERR_NO_NAME_SET";
    #endregion

    #region Constructors
    public Quest(string pName, Level pDestination)
    {
        if (pName == null)
            Name = ERR_NAME_NULL;
        else
            Name = pName;
        Type = QuestType.GoTo;

        if (pDestination == null)
            throw new ArgumentNullException("Destination can't be null");
        else
            Destination = pDestination;
    }

    public Quest(string pName, Node2D pObject)
    {
        this.Name = pName;
        this.Type = QuestType.Collect;
        this.Object = pObject;
    }

    public Quest(string pName, Node2D pObject, Node2D pRetreive)
    {
        this.Name = pName;
        this.Receiver = pRetreive;
        this.Type = QuestType.Retreive;
        this.Object = pObject;
    } 
    #endregion

    public void CheckStatus()
    {
        if (QuestManager.CurrentQuest == this && CheckCompleted())
        {
            State = QuestState.Completed;
            QuestManager.TrackedQuest.Remove(this);
            QuestManager.CompletedQuest.Add(this);
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
        switch (Type)
        {
            case QuestType.Retreive:
                if (GameController.PlayerHas(this.Object))
                    return true;
                break;
            case QuestType.GoTo:
                if (GameController.CurrentRoom == Destination)
                    return true;
                break;
            case QuestType.Collect:
                if (GameController.PlayerHas(this.Object))
                    return true;
                break;
        }

        return false;
    }
}

