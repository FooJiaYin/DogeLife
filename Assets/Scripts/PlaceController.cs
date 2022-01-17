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
    public float maxSize = 1.0f;
    public float minSize = 0.7f;
    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        updateOwner(0, ownerId);
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
        Debug.Log("updateOwner " + gameObject.name + " " + oldOwnerId + "->" + newOwnerId);
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
        ownerId = newOwnerId;
        if (newOwnerId == 0)
        {
            if (owner != null) owner.AddPlaceValue(-ownerValue);
            owner = null;
            renderer.material.color = Color.white;
        }
        else
        {

            if (owner != null)
            {
                owner.AddPlaceValue(-ownerValue);
            }
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<NetworkIdentity>().netId == newOwnerId)
                {
                    owner = players[i].GetComponent<player>();
                    owner.AddPlaceValue(ownerValue);
                }
            }
            if (owner != null) renderer.material.color = owner.placeColor;
            else renderer.material.color = Color.white;
        }
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
            if (newOwner != null)
            {
                renderer.material.color = owner.placeColor;
                owner.AddPlaceValue(ownerValue);
                owner.PlayHintAnimation(0, 0, ownerValue, 0);
            }
            else
            {
                renderer.material.color = Color.white;
            }
            CmdSetOwner(newOwnerId);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<player>().enteredPlace = this;
            Debug.Log("place set!");
            this.gameObject.transform.localScale = Vector3.one * maxSize;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<player>().SetEmptyPlace();
            Debug.Log("place unset!");
            this.gameObject.transform.localScale = Vector3.one * minSize;

        }
    }
}
