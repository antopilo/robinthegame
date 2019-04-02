using Godot;
using System;

public class SecretTiles : TileMap
{
    private Tween Tween;
    private AudioStreamPlayer AudioPlayer;

    public bool Revealed = false;

    public override void _Ready()
    {
        // Making sure that it is opaque.
        Modulate = new Color(1,1,1,1);

        // References
        AudioPlayer = (AudioStreamPlayer)GetNode("AudioStreamPlayer");
        Tween = (Tween)GetNode("Tween");
    }

    public override void _Process(float delta)
    {
        if(Revealed)
            return;

        if(this.GetCellv( WorldToMap( Root.Player.GlobalPosition - ((Level)GetNode("../../")).LevelPosition)) != -1)
            Reveal();
    }

    public void Reveal()
    {
        Revealed = true;
        AudioPlayer.Play(0);

        Tween.InterpolateProperty(this, "modulate", new Color(1,1,1,1), new Color(1,1,1,0), 1,
            Tween.TransitionType.Linear, Tween.EaseType.In);
        Tween.Start();
    }
}
