using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelController : MonoBehaviour
{
    GameObject Name;
    GameObject Score;
    GameObject Lives;

    void Start()
    {
        Name = transform.Find("Name").gameObject;
        Score = transform.Find("Score").gameObject; 
        Lives = transform.Find("Lives").gameObject; 
    }

    public void SetLives(int lives)
    {
        Lives.GetComponent<TextMeshProUGUI>().text = lives.ToString();
    }

    public void SetScore(int score)
    {
        Score.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }
    
    public void SetName(string name)
    {
        Name.GetComponent<TextMeshProUGUI>().text = name;
    }

    public void ClearAll()
    {
        Name.GetComponent<TextMeshProUGUI>().text = "";
        Score.GetComponent<TextMeshProUGUI>().text = "0";
        Lives.GetComponent<TextMeshProUGUI>().text = "0";
    }
}
