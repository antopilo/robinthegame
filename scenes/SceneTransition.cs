using Godot;

public class SceneTransition : CanvasLayer
{
    private AnimationPlayer AnimPlayer;

    
    public override void _Ready()
        =>  AnimPlayer = (AnimationPlayer)GetNode("Transition/AnimationPlayer");
    
    // Call this when you want to trigger the transition animation.
	public void Fade()
	    =>	AnimPlayer.Play("FadeOut");
	
}



