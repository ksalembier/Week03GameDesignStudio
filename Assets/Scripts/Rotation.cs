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
        //RotateCoroutine();
        RemoveBackgroundAsChildren();
    }

}
