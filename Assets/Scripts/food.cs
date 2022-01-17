using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum FoodType
{
    None = -1, Brocconi = 0, Carrot = 1, Chicken = 2, Chocolate = 3,
    Fries = 4, Grape = 5, Rubbish = 6, Tomato = 7, NumOfFood = 8
}

public static class FoodTypeExtensions
{
    public static int FoodFullValue(this FoodType me)
    {
        switch (me)
        {
            case FoodType.None:
                return 0;
            case FoodType.Brocconi:
                return 1;
            case FoodType.Carrot:
                return 2;
            case FoodType.Chicken:
                return 4;
            case FoodType.Chocolate:
                return 1;
            case FoodType.Fries:
                return 1;
            case FoodType.Grape:
                return 2;
            case FoodType.Rubbish:
                return 1;
            case FoodType.Tomato:
                return 3;
            default:
                return 0;
        }
    }

    public static int FoodHealthValue(this FoodType me)
    {
        switch (me)
        {
            case FoodType.None:
                return 0;
            case FoodType.Brocconi:
                return 1;
            case FoodType.Carrot:
                return 3;
            case FoodType.Chicken:
                return 4;
            case FoodType.Chocolate:
                return -4;
            case FoodType.Fries:
                return -1;
            case FoodType.Grape:
                return -1;
            case FoodType.Rubbish:
                return -2;
            case FoodType.Tomato:
                return 2;
            default:
                return 0;
        }
    }
}
public class Food : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = nameof(UpdateFoodType))]
    FoodType m_foodType = FoodType.None;
    [SerializeField] Sprite[] m_foodSprites;
    [SerializeField] SpriteRenderer m_foodSpriteRenderer;
    public float foodSpawnInterval = 2;

    public void SetFoodType(int foodTypeIndex)
    {
        m_foodType = (FoodType)foodTypeIndex;
        if (m_foodType != FoodType.None) m_foodSpriteRenderer.sprite = m_foodSprites[foodTypeIndex];
    }

    void UpdateFoodType(FoodType oldFoodType, FoodType newFoodType)
    {
        if (newFoodType != FoodType.None) m_foodSpriteRenderer.sprite = m_foodSprites[(int)newFoodType];
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player plyr = other.gameObject.GetComponent<player>();
            plyr.AddFoodValue(m_foodType.FoodFullValue());
            plyr.AddHealthValue(m_foodType.FoodHealthValue());
            plyr.PlayHintAnimation(m_foodType.FoodFullValue(), m_foodType.FoodHealthValue(), 0, 0);
        }
        Destroy(gameObject);
    }
}
