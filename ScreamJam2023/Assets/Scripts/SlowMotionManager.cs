using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : Singleton<SlowMotionManager>
{
    private float timer = 0.0f;


    // Update is called once per frame
    void Update()
    {
        if (PauseManager.IsPaused()) return;

        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            Time.timeScale = 1.0f;
        }
    }

    // set slow motion
    public static void SlowMotion(float duration, float scale)
    {
        Instance.timer = duration * scale;
        Time.timeScale = scale;
    }
}
