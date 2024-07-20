using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMobIndicator : MonoBehaviour
{
    public float destroyTime = 1.5f;
    private float time;

    void Update()
    {
        time += Time.deltaTime;

        if (time >= destroyTime)
            Destroy(gameObject);
    }
}
