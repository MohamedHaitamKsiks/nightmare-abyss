using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    private bool paused = false;

    // Start is called before the first frame update
    public override void OnInit()
    {
        Resume();
    }

    public static void Pause()
    {
        Instance.paused = true;
        Time.timeScale = 0.0f;

        // free cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void Resume()
    {
        Instance.paused = false;
        Time.timeScale = 1.0f;

        // lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static bool IsPaused()
    {
        return Instance.paused;
    }
}
