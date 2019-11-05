using System.Linq;
using System.Collections.Generic;
using System;
using Godot;

public enum Action 
{
    Press, Release, Tap, Wait
}

public struct InputItem
{
    public Keys Key;
    public Action Action;
    public float Duration;
}

public class InputSequence
{
    public List<InputItem> Sequence;

    public InputSequence(float startDelay)
    {
        Sequence = new List<InputItem>();
        
    }

    // Add a wait action.
    public void AddItem(float duration)
    {
        var item = new InputItem()
        {
            Action = Action.Wait,
            Duration = duration
        };
        Sequence.Add(item);
    }

    // Add a key action.
    public void AddItem(Action action, Keys key)
    {
        var item = new InputItem()
        {
            Action = action,
            Key = key
        };
        Sequence.Add(item);
    }

    public InputItem GetLastItem()
    {
        return Sequence.Last();
    }
}

public class InputRecorder : Node
{
    InputSequence CurrentSequence;

    private bool Recording = false;
    private float CurrentDelay = 0f;

    public InputSequence GetSequence()
    {
        return CurrentSequence;
    }

    public void StartRecord()
    {
        CurrentSequence = new InputSequence(1f);
        Recording = true;
    }

    public void StopRecord()
    {
        Recording = false;
    }

    public override void _Process(float delta)
    {
        if(!Recording)
            return;

        if(CurrentSequence.Sequence.Count == 0)
        {
            if(Input.IsActionJustPressed("Left"))
                CurrentSequence.AddItem(Action.Press, Keys.LEFT);
            if(Input.IsActionJustPressed("Right"))
                CurrentSequence.AddItem(Action.Press, Keys.RIGHT);

            if(Input.IsActionJustReleased("Left") )
                CurrentSequence.AddItem(Action.Release, Keys.LEFT);
            if(Input.IsActionJustReleased("Right") )
                CurrentSequence.AddItem(Action.Release, Keys.RIGHT);

            if(Input.IsActionJustPressed("Jump"))
                CurrentSequence.AddItem(Action.Press, Keys.JUMP);
            
        }
        else
        {
            InputItem lastItem = CurrentSequence.GetLastItem();

            // Press
            if(Input.IsActionJustPressed("Left") && (lastItem.Key != Keys.LEFT || lastItem.Action != Action.Press))
            {
                CurrentSequence.AddItem(CurrentDelay);
                CurrentSequence.AddItem(Action.Press, Keys.LEFT);
                Console.Log("Press left");
                CurrentDelay = 0f;
            }
            if(Input.IsActionJustPressed("Right") && (lastItem.Key != Keys.RIGHT || lastItem.Action != Action.Press))
            {
                CurrentSequence.AddItem(CurrentDelay);
                CurrentSequence.AddItem(Action.Press, Keys.RIGHT);
                Console.Log("Press right");
                CurrentDelay = 0f;
            }
            
            
            // Release
            if(Input.IsActionJustReleased("Left") && (lastItem.Key != Keys.LEFT || lastItem.Action != Action.Release))
            {
                CurrentSequence.AddItem(CurrentDelay);
                CurrentSequence.AddItem(Action.Release, Keys.LEFT);
                Console.Log("Release Left");
                CurrentDelay = 0f;
            }
            if(Input.IsActionJustReleased("Right")  && (lastItem.Key != Keys.RIGHT || lastItem.Action != Action.Release))
            {
                CurrentSequence.AddItem(CurrentDelay);
                CurrentSequence.AddItem(Action.Release, Keys.RIGHT);
                Console.Log("Release Right");
                CurrentDelay = 0f;
            }
            

            // Press
            if(Input.IsActionJustPressed("Jump") && (lastItem.Key != Keys.JUMP || lastItem.Action != Action.Tap))
            {
                CurrentSequence.AddItem(CurrentDelay);
                CurrentSequence.AddItem(Action.Tap, Keys.JUMP);
                Console.Log("Tap Jump");
                CurrentDelay = 0f;
            }
            
            CurrentDelay += delta;
        }
    }
}

