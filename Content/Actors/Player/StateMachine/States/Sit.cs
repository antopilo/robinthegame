using System;
using Godot;

public class Sit : IState
{
    public string StateName => "Sit";

    public bool StoodUp = false;
    private bool StoodAnimationCompleted = false;

    public void Enter(ref Player host)
    {
        // Play sitting down animation.
        host.Sprite.Play("SitDown");
    }

    public void Exit(ref Player host)
    {
        StoodUp = false;
        StoodAnimationCompleted = false;
    }

    public void Update(ref Player host, float delta)
    {
        // If sitting is cancel.
        if(Input.IsActionJustPressed("Jump"))
            StoodUp = true;

        // Start stand up animation.
        if(StoodUp == true && host.Sprite.Animation != "StandUp")
            host.Sprite.Play("StandUp");

        StoodAnimationCompleted = StoodUp && host.Sprite.Animation == "StandUp" & host.Sprite.Frame >= 4;

        // When stand up animation is done. set idle.
        if(StoodAnimationCompleted)
            host.StateMachine.SetState("Idle");
    }
}
