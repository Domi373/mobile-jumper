using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareIndicatorController : MonoBehaviour
{
    public float destroyTime = 2f;

    private float time = 0f;

    private void Start()
    {
        time = 0f;
    }

    private void Update()
    {
        time+= Time.deltaTime;

        if (time >= destroyTime)
            Destroy(gameObject);
    }
}
