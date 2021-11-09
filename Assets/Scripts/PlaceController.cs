using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
public class PlaceController : MonoBehaviour
{
    public float ownerValue = 10;
    public player owner;
    public float ptime = 5;
    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setOwner(player newOwner)
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
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<player>().enteredPlace = this;
            Debug.Log("place set!");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<player>().SetEmptyPlace();
            Debug.Log("place unset!");
        }
    }
}
