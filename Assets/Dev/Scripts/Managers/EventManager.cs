using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static Action<Machine> CloseMachine;
    public static Action<float> PlayerCollectedMoney;

    public static Action SetGymMachines;

    public static Action<Machine> MachinePurchased;
    public static Action<Machine> CustomerWorkOutOver;
    public static Action MoneyUpdated;
    public static Action StartGame;
    public static Action<int> LevelUnlocked;
    public static Action<List<int>> SetUnlockedLevels;
    public static Action<Transform> TutorialCameraSet;

    
    public static Action<Transform> PlayerOnUpgrade;
    public static Action PlayerOffUpgrade;
    public static Action<MoneyStack> CollectAllMoney;

    public static Action<Customer> CustomerLeavedPool;


    public static Func<CustomerNeed> GetRandomDemand;

    public static Func<MachineUpgradeData> GetMachineUpgradeData;
    public static Func<GameData> GetGameData;
    public static Func<LevelData> GetLevelMachineData;

}