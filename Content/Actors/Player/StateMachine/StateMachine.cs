using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class StateMachine
{
    // Current state being updated.
    public IState CurrentState { get; private set; }

    // All the currently available states in this machine.
    private Dictionary<string, IState> m_states = new Dictionary<string, IState>();

    private Player m_player;

    public StateMachine(Player player)
    {
        m_player = player;
    }

    
    public void Update(float delta)
    {
        CurrentState.Update(ref m_player, delta);
        GD.Print(CurrentState.StateName);
    }

    public void AddState(IState state)
    {
        // If the statemachine already has this state.
        if (m_states.ContainsKey(state.StateName))
            return;

        // Add the state to the dictionary.
        m_states.Add(state.StateName, state);
    }

    public void SetState(string stateName)
    {
        // If the statemachine already has this state.
        if (!m_states.ContainsKey(stateName))
            return;

        // Leave current state.
        if (CurrentState != null)
            CurrentState.Exit(ref m_player);

        // Sets the current state.
        CurrentState = m_states[stateName];

        // Enter the new state.
        CurrentState.Enter(ref m_player);
    }
    
}

