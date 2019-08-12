using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    private float _timer = 0;

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer>=1f)
            Destroy(this.gameObject);
    }
}
