using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FocusManager : MonoBehaviour
{
    public int keyFocusOn = 0;
    public int scrollFocusOn = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("StartupScene");
    }
    public void GetKeyFocusOn(int focusId)
    {
        keyFocusOn = focusId;
    }
    public void GetScrollFocusOn(int focusId)
    {
        scrollFocusOn = focusId;
    }
    public bool IsKeyFocusOn(int focusId)
    {
        return keyFocusOn == focusId;
    }
    public bool IsScrollFocusOn(int focusId)
    {
        return scrollFocusOn == focusId;
    }
}
