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
        RecipeChoosing,
        Cooking,
        Done
}

