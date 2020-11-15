using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    bool canRestart = false;
    private Text txtCoins;
    private GameObject restartGame;
    private MouseController mouse;
    void Awake()
    {
        txtCoins = GameObject.FindGameObjectWithTag("CoinsDisplay").GetComponent<Text>();
        restartGame = transform.Find("txtRestart").gameObject;
        mouse = FindObjectOfType<MouseController>();
    }
    void Update()
    {
        if(Input.touchCount > 0 && canRestart)
        {
            SceneManager.LoadScene(0);
        }
    }
    public void ShowRestart()
    {
        restartGame.SetActive(true);
        StartCoroutine(WaitToRestart());
    }
    IEnumerator WaitToRestart()
    {
        yield return new WaitForSeconds(2f);
        canRestart = true;
    }
    public void Redraw(uint coins)
    {
        txtCoins.text = coins.ToString();
    }
}
