using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// [RequireComponent(typeof(Rigidbody2D))]
public class PlaceController : NetworkBehaviour
{
    public int ownerValue = 1;
    public player owner = null;
    [SyncVar(hook = nameof(updateOwner))]
    public uint ownerId;
    public float ptime = 5;
    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (ownerId != 0) updateOwner(0, ownerId);
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Command(requiresAuthority = false)]
    public void CmdSetOwner(uint ownerId)
    {
        Debug.Log("Cmd set Owner");
        this.ownerId = ownerId;
    }

    public void updateOwner(uint oldOwnerId, uint newOwnerId)
    {
        Debug.Log("updateOwner " + newOwnerId + "," + oldOwnerId);
        // Debug.Log(NetworkServer.spawned[newOwnerId]);
        // foreach (KeyValuePair<uint, NetworkIdentity> p in NetworkServer.spawned)
        // {
        //     Debug.Log("Owner " + p.Key);
        // }
        // if(oldOwnerId != 0) {
        //     player oldOwner = NetworkServer.spawned[oldOwnerId].gameObject.GetComponent<player>();
        //     oldOwner.AddPlaceValue(-ownerValue);
        // }
        // owner = NetworkServer.spawned[newOwnerId].gameObject.GetComponent<player>();
        // owner.AddPlaceValue(ownerValue);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            /*
            if (players[i].GetComponent<NetworkIdentity>().netId == oldOwnerId)
            {
                player oldOwner = players[i].GetComponent<player>();
                oldOwner.AddPlaceValue(-ownerValue);
            }
            */
            if (players[i].GetComponent<NetworkIdentity>().netId == newOwnerId)
            {
                owner = players[i].GetComponent<player>();
                //owner.AddPlaceValue(ownerValue);
            }
        }
        renderer.material.color = owner.placeColor;
    }

    public void setOwner(player newOwner, uint newOwnerId)
    {
        if (owner != newOwner)
        {
            if (owner != null)
            {
                owner.AddPlaceValue(-ownerValue);
            }
            owner = newOwner;
            renderer.material.color = owner.placeColor;
            owner.AddPlaceValue(ownerValue);
            owner.PlayHintAnimation(0, 0, ownerValue, 0);
            ownerId = newOwnerId;
            CmdSetOwner(newOwnerId);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<player>().enteredPlace = this;
            Debug.Log("place set!");
            this.gameObject.transform.localScale = Vector3.one;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<player>().SetEmptyPlace();
            Debug.Log("place unset!");
            this.gameObject.transform.localScale = Vector3.one * 0.7f;

        }
    }
}
