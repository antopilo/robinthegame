using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PurpleArrow : Arrow
{
    public override void Ability()
    {
        var player = this.Player;
        var nextPosition = player.GlobalPosition;
        var oldPosition = this.GlobalPosition;

        // Teleporting.
        ReturnToPlayer();

        player.GlobalPosition = oldPosition;
    }
}

