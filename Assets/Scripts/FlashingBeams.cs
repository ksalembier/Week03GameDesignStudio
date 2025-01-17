using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingBeams : MonoBehaviour
{
    private bool isCycling;
    public GameObject beam;
    public float onDuration;
    public float offDuration;
    
    void Update()
    {
        if (!isCycling)
        {
            isCycling = true;
            StartCoroutine(CycleFlash());
        }
    }

    private IEnumerator CycleFlash()
    {
        yield return new WaitForSeconds(onDuration);
        beam.SetActive(false);
        yield return new WaitForSeconds(offDuration);
        beam.SetActive(true);
        isCycling = false;
    }

}
