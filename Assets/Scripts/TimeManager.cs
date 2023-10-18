using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    float slowFactor = 0.05f;
    float slowLength = 1f;


    // Update is called once per frame
    void Update()
    {
        Time.timeScale += (1f / slowLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }


    public void SlowMo()
    {
        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
