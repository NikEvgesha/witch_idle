using UnityEngine;public enum MachineState
{
    Open,
    Close,
}

public enum WorkoutMachineState
{
    Available,
    Busy,
    CustomerComing,
}

public enum WorkoutMachineTypes
{
    Bicycle,
    BenchPress,
    TreadMill,
    Dumbell,
    DeadLift,
    InclinePress,
    Pool
}

public enum CustomerStates
{
    
    WalkingToMachine,
    WorkingOut,
    LeavingTheGym,
    WaitingForDemand,
    WalkingToLine,
    WaitingForMachineOpen,
}

public enum CustomerNeed
{
    Nothing,
    Towel,
    Water,
    Snack,
}
