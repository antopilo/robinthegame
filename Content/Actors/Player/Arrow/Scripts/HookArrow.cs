using System;
using Godot;

public class HookArrow : Arrow
{
    public Hook Hook;

    private Vector2 playerPos, hookPosition;

    public override void Ability()
    {
        if(Hook is null)
            this.ReturnToPlayer();
        else
            DoHook();
        

    }

    public void DoHook()
    {
        this.Speed = 0f;

        Player.StateMachine.SetState("Hooked");

        Console.Log("Hookresult: " + CheckCanHook());
    }

    public bool CheckCanHook()
    {
        var spaceState = GetWorld2d().DirectSpaceState;

        playerPos = this.Player.GlobalPosition;
        hookPosition = Hook.GlobalPosition;
        var result = spaceState.IntersectRay(playerPos, hookPosition + new Vector2(4, 4), new Godot.Collections.Array{ Player, this });

        DebugPrinter.AddDebugItem("HookRC", result.ToString());

       

        if(result.Keys.Count > 0 && !(result["collider"] is Player) && !(result["collider"] is Arrow))
            return false;
            
        return true;
    }
    public override void _Draw()
    {
        if(playerPos != null && hookPosition != null)
             DrawLine(new Vector2(), playerPos , new Color(1,0,0),1f);

    }
}

