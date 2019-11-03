using System.Diagnostics;
using System;
using Godot;

public class Hooked : IState
{
    public string StateName => "Hooked";

    private Hook Hook;
    private float currentAngle = 0f;

    private const float MAX_ANGLE = 45f, MIN_ANGLE = -45f;
    private int direction = 1;
    public void Enter(ref Player host)
    {
        Hook = ((HookArrow)host.Arrow).Hook;
        currentAngle = 0f;
        host.CollisionBox.Disabled = true;

        if(Hook.GlobalPosition.x > host.GlobalPosition.x)
            direction = 1;
        else
            direction = -1;

        var maxAngle = Mathf.Rad2Deg((Hook.GlobalPosition - host.GlobalPosition).Angle() ) - 90f;
        var minAngle = 180 - maxAngle;

        DebugPrinter.AddDebugItem("maxAngle", maxAngle.ToString());
        DebugPrinter.AddDebugItem("minAngle", minAngle.ToString());

    }

    public void Exit(ref Player host)
    {
        host.CollisionBox.Disabled = false;
        host.Jump();
    }

    public void Update(ref Player host, float delta)
    {
        var playerAngle = Mathf.Rad2Deg((Hook.GlobalPosition - host.GlobalPosition).Angle());
        DebugPrinter.AddDebugItem("Angle", playerAngle.ToString());
        
        if(( - Mathf.Deg2Rad(90)) > Mathf.Deg2Rad((MAX_ANGLE)))
            direction = -direction;
        if(((Hook.GlobalPosition - host.GlobalPosition).Angle()- Mathf.Deg2Rad(90)) < Mathf.Deg2Rad((MIN_ANGLE)))
            direction = -direction;

        if(direction == 1)
            host.GlobalPosition = rotatePoint(host.GlobalPosition, Hook.GlobalPosition, 1f);
        else
            host.GlobalPosition = rotatePointReverse(host.GlobalPosition, Hook.GlobalPosition, 1f);
        
        if(Input.IsActionJustPressed("Jump"))
            host.StateMachine.SetState("Air");

    }

    private Vector2 rotatePoint(Vector2 point, Vector2 center, float angle){

        var angle2 = Mathf.Deg2Rad(angle);// Convert to radians

        var rotatedX = Mathf.Cos(angle2) * (point.x - center.x) - Mathf.Sin(angle2) * (point.y-center.y) + center.x;
        var rotatedY = Mathf.Sin(angle2) * (point.x - center.x) + Mathf.Cos(angle2) * (point.y - center.y) + center.y;



        return new Vector2(rotatedX, rotatedY);

    }

    private Vector2 rotatePointReverse(Vector2 point, Vector2 center, float angle){

        var angle2 = Mathf.Deg2Rad(angle);// Convert to radians


        var rotatedX = Mathf.Cos(angle2) * (point.x - center.x) + Mathf.Sin(angle2) * (point.y-center.y) + center.x;
        var rotatedY = Mathf.Sin(angle2) * -(point.x - center.x) + Mathf.Cos(angle2) * (point.y - center.y) + center.y;



        return new Vector2(rotatedX, rotatedY);

    }
}

