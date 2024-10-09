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
        Water,
}

    public enum PurchasedState
    {
        Purchased,
        Unpurchased,
    }

    public enum RecipeState
{
        Available, //Доступен для создания зелья
        Unstudied, //Отображается в книге рецептов, но не доступен для выбора (нужно купить? Или вырастить нужный ингредиент?)
        Hidden // Недоступен, не отображается в книге рецептов
}

    public enum PotState
{
        Empty,
        WaterRequire,
        IngredientRequire,
        Cooking,
        Done
}

public enum StorageAction
{
    PutToStorage,
    GetFromStorage,
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

