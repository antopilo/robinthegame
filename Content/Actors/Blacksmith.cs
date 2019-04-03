using Godot;
using System;

public class Blacksmith : Node2D
{
    private int TalkCount = 0;
    private int MaxTalk = 5;
    public bool CanInteract = true;
    
    private string[] Dialogs = new string[]{
        "Hello!",
        "How are you? ",
        "Yes?",
        "What do you want?",
        "..."
        };

    private string Default = "Go away, I got things to do...";

    public void Interact()
    {

        if(TalkCount >= MaxTalk - 1){
            CanInteract = false;
            Root.Player.RemoveFromInteraction(this);
            Root.SceneTransition.MoveOrbInGame(Root.Player.GlobalPosition);
        }
            
        if(TalkCount < Dialogs.Length - 1 )
            Root.Dialog.ShowMessage(Dialogs[TalkCount]);
        else
            Root.Dialog.ShowMessage(Default);

        TalkCount++;
    }
}
