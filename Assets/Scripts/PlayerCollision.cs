using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public GameObject rotation;

    private GameObject dayStartMarker;
    private GameObject nightStartMarker;

    public GameObject dayKeySymbol;
    public GameObject nightKeySymbol;

    private bool isDay;
    private bool isNight;

    private bool isHoldingDayKey;
    private bool isHoldingNightKey;

    private void Start()
    {
        dayStartMarker = GameObject.Find("Day Start Marker");
        nightStartMarker = GameObject.Find("Night Start Marker");
        
        isHoldingDayKey = false;
        isHoldingNightKey = false;
    }

    private void MakeIncorporeal()
    {
        this.GetComponent<Rigidbody2D>().simulated = false;
        this.GetComponent<Collider2D>().enabled = false;
    }

    private void MakeCorporeal()
    {
        this.GetComponent<Rigidbody2D>().simulated = true;
        this.GetComponent<Collider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // obstacle collisions
        if (collision.gameObject.tag == "Day Portal") // day portal in the night world
        {
            StartCoroutine(RotateCoroutine(0f, false));
        }
        if (collision.gameObject.tag == "Night Portal") // night portal in a day world
        {
            StartCoroutine(RotateCoroutine(180f, true));
        }

        // key and door collisions
        if (collision.gameObject.tag == "Day Key") // collected in night world, used in day world
        {
            collision.gameObject.SetActive(false);
            dayKeySymbol.SetActive(true);
            isHoldingDayKey =  true;
        }
        if (collision.gameObject.tag == "Night Key") // collected in day world, used in night world
        {
            collision.gameObject.SetActive(false);
            nightKeySymbol.SetActive(true);
            isHoldingNightKey =  true;
        }
        if (collision.gameObject.tag == "Day Door" && isHoldingDayKey)
        {
            collision.gameObject.SetActive(false); // or maybe just remove collider
            dayKeySymbol.SetActive(false);
            isHoldingDayKey = false;
        }
        if (collision.gameObject.tag == "Night Door" && isHoldingNightKey)
        {
            collision.gameObject.SetActive(false); // or maybe just remove collider
            nightKeySymbol.SetActive(false);
            isHoldingNightKey = false;
        }
    }

    private IEnumerator RotateCoroutine(float newZValue, bool isDay)
    {
        MakeIncorporeal();
        rotation.GetComponent<Rotation>().Rotate(newZValue);
        yield return new WaitForSeconds(2.5f);
        float newStartX;
        if (isDay) { newStartX = nightStartMarker.transform.position.x; }
        else { newStartX = dayStartMarker.transform.position.x; }
        //StartCoroutine(MoveToStartCoroutine(transform.position, new Vector2(newStartX, transform.position.y), 1.0f));
        StartCoroutine(MoveToStartCoroutine(transform.position, new Vector2(newStartX, 3.0f), 1.0f));
        yield return new WaitForSeconds(2.0f);
        MakeCorporeal();
    }

    private IEnumerator MoveToStartCoroutine(Vector2 startPosition, Vector2 targetPosition, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedPercentage);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

}
