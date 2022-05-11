using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    public Text nameT;
    public Text studentNumberT;

    public TMP_Text correct;
    public TMP_Text wrong;
    public TMP_Text grade;

    public Button retryBtn;
    public Button quitBtn;

    void Awake()
    {
        if (PlayerPrefs.HasKey("Name"))
        {
            nameT.text = PlayerPrefs.GetString("Name");
        }

        if (PlayerPrefs.HasKey("StudentNumber"))
        {
            studentNumberT.text = PlayerPrefs.GetString("StudentNumber");
        }

        retryBtn.onClick.AddListener(RetryGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        correct.text = (StaticData.score / 5).ToString();
        wrong.text = ((100 - StaticData.score) / 5).ToString();
        int tGrade = StaticData.score;
        grade.text = tGrade.ToString() + "%";
    }

    void RetryGame()
    {
        FadeIntoNextScene.instance.LoadBeginScene();
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
