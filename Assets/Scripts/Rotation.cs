using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    float reference;
    public float rotationDuration = 2.0f;

    private static GameObject day;
    private static GameObject night;
    private static GameObject divide;
    private static GameObject player;

    private float targetRotation;

    private void Start()
    {
        day = GameObject.Find("DAY");
        night = GameObject.Find("NIGHT");
        divide = GameObject.Find("DIVIDE");
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        float rotate = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation, ref reference, 0.5f);
        transform.rotation = Quaternion.Euler(0, 0, rotate);
    }

    private void SetPosition()
    {
        this.transform.position = new Vector2(player.transform.position.x, 0f);
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

    private IEnumerator RotateCoroutine(float newZValue)
    {
        SetPosition();
        AddBackgroundAsChildren();
        targetRotation = newZValue;
        yield return new WaitForSeconds(2.5f);
        transform.rotation = Quaternion.Euler(0, 0, newZValue);
        RemoveBackgroundAsChildren();
    }

    public void Rotate(float newZValue)
    {
        StartCoroutine(RotateCoroutine(newZValue));
    }

    // private IEnumerator RotateCoroutine(Quaternion startPosition, Quaternion targetPosition, float duration)
    // {
    //     float elapsedTime = 0;
    //     float elapsedPercentage = 0;

    //     while (elapsedPercentage < 1)
    //     {
    //         elapsedPercentage = elapsedTime / duration;
    //         transform.rotation = Quaternion.Lerp(startPosition, targetPosition, elapsedPercentage);
    //         yield return null;
    //         elapsedTime += Time.deltaTime;
    //     }
    // }

}
