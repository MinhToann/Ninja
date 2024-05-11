using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject btnLoadGame;
    [SerializeField] private GameObject btnPauseGame;
    private void Start()
    {
        btnLoadGame.SetActive(false);
    }
    public void OnClickPauseButton()
    {
        btnLoadGame.SetActive(true);
        btnPauseGame.SetActive(false);
        Time.timeScale = 0;
    }
    public void OnClickResumeButton()
    {
        btnLoadGame.SetActive(false);
        btnPauseGame.SetActive(true);
        Time.timeScale = 1;
    }
    public void OnClickReloadButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
