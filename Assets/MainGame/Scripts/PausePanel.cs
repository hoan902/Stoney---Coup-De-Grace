using UnityEngine;

public class PausePanel : MonoBehaviour
{
    private void OnEnable()
    {
        SaveModel.paused = true;
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        SaveModel.paused = false;
        Time.timeScale = 1f;
    }
}
