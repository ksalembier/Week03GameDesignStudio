using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public Rotation rotation;
    private void isHoldingDayKey;
    private void isHoldingNightKey;

    private void Start()
    {
        isHoldingDayKey = false;
        isHoldingNightKey = false;
    }

    private void MakeIncorporeal()
    {
        this.GetComponent<Rigidbody2D>().simulated = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void MakeCorporeal()
    {
        this.GetComponent<Rigidbody2D>().simulated = true;
        this.GetComponent<BoxCollider2D>().enabled = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Day Obstacle")
        {
            MakeIncorporeal();
            rotation.Rotate();
            MakeCorporeal();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

    }

    private void OnTriggerExit2D(Collider2D collider)
    {

    }
}
