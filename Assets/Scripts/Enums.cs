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
    }

    public enum PotionTypes
    {
        Health,
        Strength
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

