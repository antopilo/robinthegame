using System.IO.IsolatedStorage;
using System;
using System.Collections;
using Godot;

public class MoveTest : CutSceneController
{
    public override string StateName => "MoveTest";

    private InputSequence CurrentSequence;
    private float DeltaTime = 0f;
    private float NextAction = 0f;

    public override void Enter(ref Player host)
    {
        base.Enter(ref host);

        //Runner.StopAll();

        InputDirection = new Vector2();

        if(host.CurrentSequence is null)
            host.StateMachine.SetState("Idle");

        CurrentSequence = host.CurrentSequence;

        //Coroutine = Runner.Run(Test());
    }

    public override void Update(ref Player host, float delta)
    {
        DeltaTime += delta;

        base.Update(ref host, delta);

        if(CurrentSequence.SequenceEnded())
        {
            Console.Log("Sequence Ended");
            host.StateMachine.SetState("Idle");
        }

        

        if(NextAction <= DeltaTime)
        {
            var currentItem = CurrentSequence.GetNextItem();
            PerformAction(currentItem);
        }
            

        

    }


    private void PerformAction(InputItem item)
    {
        if(item.Action == Action.Wait)
        {
            Console.Log("Action: " + "Wait " + item.Duration );
            NextAction = DeltaTime + item.Duration;
        }
        else if(item.Action == Action.Press)
        {
            Console.Log("Press: " + item.Key.ToString());
            Press(item.Key);
        }
        else if(item.Action == Action.Release)
        {
            Console.Log("Release: " + item.Key.ToString());
            Release(item.Key);
        }
        else if(item.Action == Action.Tap)
        {
            Jump();
        }

        
    }


    public override void Exit(ref Player host){}

    
}

