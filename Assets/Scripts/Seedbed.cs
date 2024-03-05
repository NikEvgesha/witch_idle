
using UnityEngine;

public class Seedbed : Machine
{

    private SeedbedState _state;

    private void Update()
    {
        switch (_state)
        {
            case SeedbedState.Empty:
                break;
            case SeedbedState.Growing:
                break;
            case SeedbedState.Grown:
                break;
        }
    }

}
