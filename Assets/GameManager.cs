using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public HttpManager web;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
        web = GameObject.FindWithTag("httpManager").GetComponent<HttpManager>();
        web.Record();
    }

    public void Salir()
    {
        HttpManager.Salir();
        SceneManager.LoadScene("Inicio");
    }
    public void Replay()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Scores()
    {
        web = GameObject.FindWithTag("httpManager").GetComponent<HttpManager>();
        web.ClickGetScores();
        

    }
}
