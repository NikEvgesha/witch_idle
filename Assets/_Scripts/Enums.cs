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
        Available, //Доступен для создания зелья
        Unstudied, //Отображается в книге рецептов, но не доступен для выбора (нужно купить? Или вырастить нужный ингредиент?)
        Hidden // Недоступен, не отображается в книге рецептов
}

    public enum PotState
{
        Empty,
        IngredientRequire,
        Cooking,
        Done
}

