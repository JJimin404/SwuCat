using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    public GameObject pause;
    public GameObject howTo1;
    public GameObject howTo2;
    public GameObject howTo3;

    //메인화면으로
    public void ToMain()
    {
        SceneManager.LoadScene("MainScene");
    }
    //메뉴화면으로
    public void ToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    //게임 1로
    public void startGame1()
    {
        SceneManager.LoadScene("Game1");
    }
    //게임 2로
    public void startGame2()
    {
        SceneManager.LoadScene("Game2");
    }
    //게임 3으로
    public void startGame3()
    {
        SceneManager.LoadScene("Game3");
    }
    //종료하기
    public void GameExit()
    {
        Application.Quit();
    }
    //게임1 설명창 띄우기
    public void popUp1()
    {
        howTo1.SetActive(true);
    }
    //게임1 설명창 닫기
    public void exit1()
    {
        howTo1.SetActive(false);
    }
    //게임2 설명창 띄우기
    public void popUp2()
    {
        howTo2.SetActive(true);
    }
    //게임2 설명창 닫기
    public void exit2()
    {
        howTo2.SetActive(false);
    }
    //게임3 설명창 띄우기
    public void popUp3()
    {
        howTo3.SetActive(true);
    }
    //게임3 설명창 닫기
    public void exit3()
    {
        howTo3.SetActive(false);
    }
    //일시정지
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
    }
    //계속하기
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
    }

}
