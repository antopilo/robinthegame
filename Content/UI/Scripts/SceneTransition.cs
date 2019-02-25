using Godot;

public class SceneTransition : CanvasLayer
{
    private AnimationPlayer AnimPlayer;
    private Control InteractionSprite;
    private AnimationPlayer InteractionAnim;
    private string CurrentAnim = "Inactive";

    public override void _Ready()
    {
        InteractionAnim = (AnimationPlayer)GetNode("Transition/AnimationPlayer");
        InteractionSprite = (Control)GetNode("InteractionFeedback");
        InteractionAnim = (AnimationPlayer)GetNode("InteractionFeedback/AnimationPlayer");
    }
        
    public override void _Process(float delta){
        
        GetPositionFromPlayer();
    }
    // Call this when you want to trigger the transition animation.
	public void Fade()
	    =>  AnimPlayer.Play("FadeOut");
	
    public void GetPositionFromPlayer()
    {
        var player = Root.Player;   
        var camera = Root.Player.Camera;
        var heightOffset = new Vector2(0, -16);
        var Offset = camera.GetCameraScreenCenter() - (camera.GetViewportRect().Size / 2f);
        var uiPosition = player.GlobalPosition - Offset + heightOffset;
        var scale = Root.GameContainer.StretchShrink;

        InteractionSprite.RectPosition = (uiPosition * new Vector2((float)scale, (float)scale));        

        // Anim
        if(player.CanInteract && CurrentAnim != "Active" )
            InteractionAnim.CurrentAnimation = CurrentAnim = "Active";
        else if(!player.CanInteract && CurrentAnim != "Inactive")
            InteractionAnim.CurrentAnimation = CurrentAnim = "Inactive";
        
        if(Input.IsActionJustPressed("Interact"))
            player.CanInteract = !player.CanInteract;
    }
}



