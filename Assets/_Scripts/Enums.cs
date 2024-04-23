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
        Dandelion,
        Rose,
        None,
    }

    public enum PotionTypes
    {
        Health,
        Strength,
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

