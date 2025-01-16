using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public static GameObject day;
    public static GameObject night;
    public static GameObject divide;
    public static GameObject player;

    private void Start()
    {
        day = GameObject.Find("Day Background");
        night = GameObject.Find("Night Background");
        divide = GameObject.Find("Background Divide");
        player = GameObject.Find("Player");
    }

    private void SetPosition()
    {
        this.transform.position = player.transform.position;
    }

    private void AddBackgroundAsChildren()
    {
        day.transform.parent = this.transform;
        night.transform.parent = this.transform;
        divide.transform.parent = this.transform;
    }

    private void RemoveBackgroundAsChildren()
    {
        this.transform.DetachChildren();
    }


    private IEnumerator RotateCoroutine(Quaternion startPosition, Quaternion targetPosition, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            transform.rotation = Quaternion.Lerp(startPosition, targetPosition, elapsedPercentage);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    public void Rotate()
    {
        SetPosition();
        AddBackgroundAsChildren();
        // if (transform.rotation.z == 0)
        // {
        //     StartCoroutine(RotateCoroutine(transform.rotation, (transform.rotation.x, transform.rotation.y, 180f, 0f), 2));
        // }
        // else
        // {
        //     StartCoroutine(RotateCoroutine(transform.rotation, (transform.rotation.x, transform.rotation.y, 0f, 0f), 2));
        // }
        RemoveBackgroundAsChildren();
    }

}
