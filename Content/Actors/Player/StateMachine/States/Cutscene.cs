using System;

public class Cutscene : IState
{
    public string StateName => "Cutscene";

    public void Enter(ref Player host)
    {
        
    }

    public void Exit(ref Player host)
    {
        throw new NotImplementedException();
    }

    public void Update(ref Player host, float delta)
    {
        throw new NotImplementedException();
    }
}

