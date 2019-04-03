using Godot;

public class SceneTransition : CanvasLayer
{
    public bool SceneChangeReady = false;
    public AnimationPlayer InteractionAnim;

    private AnimationPlayer AnimPlayer;
    private Control InteractionSprite;
    private string CurrentAnim = "Inactive";
    private bool AnimationPlayed = false;
    private string LastAnimationPlayer = "";
    private Vector2 LastOrbPosition = new Vector2();
    private PackedScene textScene;
    private Tween Tween;
    

    private SpeechTextDissapearing LastText = null;

    public override void _Ready()
    {
        AnimPlayer = (AnimationPlayer)GetNode("Transition/AnimationPlayer");
        InteractionSprite = (Control)GetNode("InteractionFeedback");
        InteractionAnim = (AnimationPlayer)GetNode("InteractionFeedback/AnimationPlayer");
        textScene = ResourceLoader.Load("res://Content/Actors/SpeechText.tscn") as PackedScene;
        Tween = (Tween)GetNode("Tween");

        AnimPlayer.Play("Out");
    }
        
    public override void _Process(float delta){
        
        GetPositionFromPlayer();
    }
    // Call this when you want to trigger the transition animation.
	public void Fade()
    {   
	    AnimPlayer.Play("FadeOut");
        GetTree().Paused = true;
    }

    public void FadeIn()
    {
        AnimPlayer.Play("FadeIn");
    }
	
    // Move the interaction orb to the closest interactable object.
    public void GetPositionFromPlayer()
    {
        
        var player = Root.Player;   
        var camera = Root.Player.Camera;
        var heightOffset = new Vector2(0, -16);
        var Offset = camera.GetCameraScreenCenter() - (camera.GetViewportRect().Size / 2f);
        Vector2 uiPosition; 
        var scale = Root.GameContainer.StretchShrink;

        if(player.InteractableObject.Count > 0 && player.InteractableObject[0] != null){
            var offset = new Vector2();
            if(player.InteractableObject[0].Get("InteractionOffset") as Vector2? != null)
                offset = (player.InteractableObject[0].Get("InteractionOffset") as Vector2?).GetValueOrDefault();
            uiPosition = player.InteractableObject[0].GlobalPosition + offset - Offset + heightOffset;

            if((uiPosition * new Vector2((float)scale, (float)scale)) != LastOrbPosition){
                MoveOrb((uiPosition * new Vector2((float)scale, (float)scale)));
                LastOrbPosition = (uiPosition * new Vector2((float)scale, (float)scale));
            }
        }
        else{
            uiPosition = player.GlobalPosition - Offset + heightOffset;
            if((uiPosition * new Vector2((float)scale, (float)scale)) != LastOrbPosition){
                MoveOrb((uiPosition * new Vector2((float)scale, (float)scale)));
                LastOrbPosition = (uiPosition * new Vector2((float)scale, (float)scale));
            }
        }
       
        
        // Anim
        if(player.CanInteract && CurrentAnim != "Active"){
            InteractionAnim.CurrentAnimation = CurrentAnim = "Active";
        }
        else if(!player.CanInteract && CurrentAnim != "Inactive")
            InteractionAnim.CurrentAnimation = CurrentAnim = "Inactive";
    }

    public void MoveOrb(Vector2 pPosition)
    {
        Tween.StopAll();
        Tween.InterpolateProperty(InteractionSprite, "rect_position", InteractionSprite.RectPosition, pPosition,
            0.5f, Tween.TransitionType.Expo, Tween.EaseType.Out);
        Tween.Start();
    }

    public void MoveOrbInGame(Vector2 pPosition)
    {
        var scale = Root.GameContainer.StretchShrink;
        var heightOffset = new Vector2(0, -8);
        var camera = Root.Player.Camera;
        var Offset = camera.GetCameraScreenCenter() - (camera.GetViewportRect().Size / 2f);
        Tween.StopAll();
        Tween.InterpolateProperty(InteractionSprite, "rect_position", InteractionSprite.RectPosition, 
            (pPosition - Offset) * new Vector2((float)scale, (float)scale) - heightOffset,
            0.5f, Tween.TransitionType.Expo, Tween.EaseType.Out);
        Tween.Start();
        AnimPlayer.CurrentAnimation = "Inactive";
    }

    // Make an an npc say something over their head!
    public void NpcSay(Node2D Npc, string pMessage)
    {
        if(LastText != null){
            LastText.QueueFree();
        }

        var player = Npc;   
        var camera = Root.Player.Camera;
        var heightOffset = new Vector2(0, -16);
        var Offset = camera.GetCameraScreenCenter() - (camera.GetViewportRect().Size / 2f);
        var uiPosition = player.GlobalPosition - Offset + heightOffset;
        var scale = Root.GameContainer.StretchShrink;
        var position = (uiPosition * new Vector2((float)scale, (float)scale));
        var newtext = textScene.Instance() as SpeechTextDissapearing;

        newtext.text = pMessage;
        newtext.Name = "SpeechText";
        newtext.RectPosition = position - new Vector2(0, 32);
        newtext.RectScale /= 1.33f;

        AddChild(newtext);
        LastText = newtext;
    }

    // When the transition stops, start the game.
    private void _on_AnimationPlayer_animation_finished(string anim_name)
    {
        // If the animation is triggered while changing scene.
        if(anim_name == "FadeIn")
        {
            GD.Print("Switcher Ready!");
            SceneChangeReady = true;
            AnimPlayer.Play("Out");
            return;
        }
        if(anim_name == "Out"){
            return;
        }

        GetTree().Paused = false;
        Root.Player.Alive = true;
        Root.Player.CanControl = true;
    }
}

