using Godot;
using Coroutines;
using System.Collections;

public class TestCutscene : IState
{
    public string StateName => "Test";

    public void Enter(ref Player host)
    {

    }

    public void Exit(ref Player host)
    {
        
    }

    public void Update(ref Player host, float delta)
    {
        

        

    }


    private IEnumerator Test()
    {
        Root.Dialog.ShowMessage("Hello");
        yield return 3f; // wait 3 seconds
        Root.Dialog.ShowMessage("Hello2");
        yield return 3f; // wait 3 seconds
        Root.Dialog.ShowMessage("Hello3");
    }
}
