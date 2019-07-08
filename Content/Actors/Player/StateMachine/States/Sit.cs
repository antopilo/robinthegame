using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Sit : IState
{
    public string StateName { get; } = "Sit";

    public void Enter(ref Player host)
    {
        host.Sprite.Play("Face");
    }

    public void Update(ref global::Player host, float delta)
    {
    }

    public void Exit(ref global::Player host)
    {
    }
}

