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
    }

    public enum PotionTypes
    {
        Health,
        Strength,
        Brave,
        Poison,
        Love,
        None,
    }

    public enum ItemTypes
    {
        Plant,
        Potion,
}

    public enum PurchasedState
    {
        Purchased,
        Unpurchased,
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
        IngredientRequire,
        Cooking,
        Done
}

public enum StorageAction
{
    PutToStorage,
    GetFromStorage,
}

