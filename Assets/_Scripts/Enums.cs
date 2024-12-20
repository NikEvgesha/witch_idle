using UnityEngine;
    public enum SeedbedState
    {
        Empty,
        Growing,
        Grown,
    }
    public enum PlantTypes
    {
        Mushroom,
        Pumpkin,
        None,
        Bloodthorn,
        Boombloom,
        Coldleaf,
        Dandelion,
        Goodberry,
        Lifelist,
        Rosemary,
        Spellbloom,
}

    public enum PotionTypes
    {
        Health,
        Poison,
        Strength,
        Dexterity,
        Sleep,
        SlowingDown,
        ExuberantGrowth,
        swiftness,
        fire,
        frost,
        Brave,
        Love,
        None,
    }

    public enum ItemTypes
    {
        Plant,
        Potion,
        Water,
}

    public enum PurchasedState
    {
        Purchased,
        Unpurchased,
        Locked,
}

    public enum RecipeState
{
        Available, //�������� ��� �������� �����
        Unstudied, //������������ � ����� ��������, �� �� �������� ��� ������ (����� ������? ��� ��������� ������ ����������?)
        Hidden // ����������, �� ������������ � ����� ��������
}

    public enum PotState
{
        Empty,
        BasisRequire,
        IngredientRequire,
        Cooking,
        Done
}

public enum StorageAction
{
    PutToStorage,
    GetFromStorage,
}
public enum GridUIType
{
    StorageGrid,
    InventoryGrid,
}
public enum NPSStates
{

    WalkingToStore,
    WaitingItemSale,
    WalkingToQueue,
    WaitingInQueue,
    WaitingSeller,
    WalkingHome,
}

public enum ControlType
{
    Keyboard,
    Joystick,
}

