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

    //����ȭ������
    public void ToMain()
    {
        SceneManager.LoadScene("MainScene");
    }
    //�޴�ȭ������
    public void ToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    //���� 1��
    public void startGame1()
    {
        SceneManager.LoadScene("Game1");
    }
    //���� 2��
    public void startGame2()
    {
        SceneManager.LoadScene("Game2");
    }
    //���� 3����
    public void startGame3()
    {
        SceneManager.LoadScene("Game3");
    }
    //�����ϱ�
    public void GameExit()
    {
        Application.Quit();
    }
    //����1 ����â ����
    public void popUp1()
    {
        howTo1.SetActive(true);
    }
    //����1 ����â �ݱ�
    public void exit1()
    {
        howTo1.SetActive(false);
    }
    //����2 ����â ����
    public void popUp2()
    {
        howTo2.SetActive(true);
    }
    //����2 ����â �ݱ�
    public void exit2()
    {
        howTo2.SetActive(false);
    }
    //����3 ����â ����
    public void popUp3()
    {
        howTo3.SetActive(true);
    }
    //����3 ����â �ݱ�
    public void exit3()
    {
        howTo3.SetActive(false);
    }
    //�Ͻ�����
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
    }
    //����ϱ�
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
    }

}
