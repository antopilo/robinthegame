using System.IO.IsolatedStorage;
using System;
using System.Collections;
using Godot;

public class MoveTest : CutSceneController
{
    public override string StateName => "MoveTest";

    private InputSequence CurrentSequence;

    public override void Enter(ref Player host)
    {
        base.Enter(ref host);

        Runner.StopAll();

        if(host.CurrentSequence is null)
            host.StateMachine.SetState("Idle");

        CurrentSequence = host.CurrentSequence;

        Coroutine = Runner.Run(Test());
    }

    public override void Update(ref Player host, float delta)
    {
        base.Update(ref host, delta);

        //if(!Runner.IsRunning(Coroutine)){
         //   Console.Log("Done");
         //   host.StateMachine.SetState("Idle");
        //}
    }

    public override void Exit(ref Player host){}

    private IEnumerator Test()
    {
        Console.Log("Tick");
        Press(Keys.LEFT);

        yield return 2f;
        Release(Keys.LEFT);
    }
}

