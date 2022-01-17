using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Monument : NetworkBehaviour
{
    [SerializeField] TMP_Text NameText;
    [SyncVar(hook = nameof(UpdatePlayerName))]
    string playerName;

    public void SetPlayerName(string name)
    {
        this.playerName = name;
    }

    public void UpdatePlayerName(string oldName, string newName)
    {
        NameText.text = newName;
    }
}
