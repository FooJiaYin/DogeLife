using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerHintBox : MonoBehaviour
{
    [SerializeField] GameObject EatHint;
    [SerializeField] GameObject HealthHint;
    [SerializeField] GameObject PlaceHint;
    [SerializeField] GameObject SocialHint;

    [SerializeField] TMP_Text EatValue;
    [SerializeField] TMP_Text HealthValue;
    [SerializeField] TMP_Text PlaceValue;
    [SerializeField] TMP_Text SocialValue;

    public void UpdateValue(int eat, int health, int place, int social)
    {
        UpdateEatValue(eat);
        UpdateHealthValue(health);
        UpdatePlaceValue(place);
        UpdateSocialValue(social);
    }

    void UpdateEatValue(int eat)
    {
        if (eat == 0)
        {
            EatHint.SetActive(false);
        }
        else if (eat > 0)
        {
            EatValue.text = "+" + eat;
            EatHint.SetActive(true);
        }
        else
        {
            EatValue.text = eat.ToString();
            EatHint.SetActive(true);
        }
    }

    void UpdateHealthValue(int health)
    {
        if (health == 0)
        {
            HealthHint.SetActive(false);
        }
        else if (health > 0)
        {
            HealthValue.text = "+" + health;
            HealthHint.SetActive(true);
        }
        else
        {
            HealthValue.text = health.ToString();
            HealthHint.SetActive(true);
        }
    }

    void UpdatePlaceValue(int place)
    {
        if (place == 0)
        {
            PlaceHint.SetActive(false);
        }
        else if (place > 0)
        {
            PlaceValue.text = "+" + place;
            PlaceHint.SetActive(true);
        }
        else
        {
            PlaceValue.text = place.ToString();
            PlaceHint.SetActive(true);
        }
    }

    void UpdateSocialValue(int social)
    {
        if (social == 0)
        {
            SocialHint.SetActive(false);
        }
        else if (social > 0)
        {
            SocialValue.text = "+" + social;
            SocialHint.SetActive(true);
        }
        else
        {
            SocialValue.text = social.ToString();
            SocialHint.SetActive(true);
        }
    }

}
