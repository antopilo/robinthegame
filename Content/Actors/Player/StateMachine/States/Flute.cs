using System;
using Godot;

public class Flute : IState
{
    public string StateName => "Flute";

    public void Enter(ref Player host)
    {
        host.Sprite.Play("Flute");
        host.Weapon.CanShoot = true;
        host.Weapon.ShootArrow();
    }

    public void Exit(ref Player host)
    {
        host.Weapon.CanShoot = false;
    }

    public void Update(ref Player host, float delta)
    {
        if(host.Weapon.CurrentArrow.Frozen)
            host.StateMachine.SetState("Idle");

    }
}

