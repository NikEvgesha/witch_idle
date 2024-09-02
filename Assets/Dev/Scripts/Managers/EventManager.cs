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


    // новые


    public static Action<int> MoneyChange;
    public static Func<GameData> GetWalletData;
    public static Action PlayerOnPlantArea;
    public static Action PlayerOffPlantArea;
    //public static Action PlantHarvested;
    public static Action UpdateUIInventory;
    public static Action<int> CapacityUpdate;

    public static Action<float> RecipeBookOpened;
    public static Action RecipeBookClosed;
    public static Action<RecipeData> RecipeSelected;

    //public static Action ObjectPurshuased;

    public static Action<StorageAction, int> StorageCellClick;
}
