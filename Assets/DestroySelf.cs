using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    private float _timer = 0;

    [SerializeField] private float _timeUntilDestruction;
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer>=_timeUntilDestruction)
            Destroy(this.gameObject);
    }
}
