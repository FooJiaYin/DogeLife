using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Monument : MonoBehaviour
{
    [SerializeField] TMP_Text NameText;

    public void SetPlayerName(string name)
    {
        NameText.text = name;
    }

}
