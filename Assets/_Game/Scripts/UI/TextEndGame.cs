using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TextEndGame : MonoBehaviour
{
    [SerializeField] private GameObject WinPrefab;

    void Start()
    {
        WinPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        TimeStop();
    }
    void TimeStop()
    {
        StartCoroutine(StopTime());
    }
    IEnumerator StopTime()
    {
        if (Enemy.instance.isEnemyDead)
        {
            yield return new WaitForSeconds(1.5f);
            WinPrefab.SetActive(true);
            Player.instance.speed = 0f;
            Player.instance.isJump = false;
            Player.instance.isAttack = false;           
            yield return new WaitForSeconds(2.5f);
            Time.timeScale = 0;
        }       
    }
}
