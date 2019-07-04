using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState
{
    string StateName { get; }


    /// <summary>
    /// Initialize the values for this state.
    /// </summary>
    /// <param name="host">The player reference</param>
    void Enter(ref Player host);

    /// <summary>
    /// This is the behaviour of the state, called every frame.
    /// </summary>
    /// <param name="host">Player</param>
    /// <param name="delta">time between the current frame and last frame.</param>
    void Update(ref Player host, float delta);

    /// <summary>
    /// Exit the state.
    /// </summary>
    /// <param name="host">Player</param>
    void Exit(ref Player host);
}

