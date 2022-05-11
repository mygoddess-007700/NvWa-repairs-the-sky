using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public int number = 1;
    public int score = 0;
    
    [Header("Reference")]
    public TextAsset Answer1;
    public TextAsset Answer2;
    public Vector2 questionPos;

    public TMP_Text numT;
    public TMP_Text scoreT;
    public TMP_Text answer1T;
    public TMP_Text answer2T;
    
    public Image rightI1;
    public Image errorI1;
    public Image rightI2;
    public Image errorI2;

    public AudioSource rightA;
    public AudioSource errorA;

    public Stone [] stone;
    public Hole [] hole;

    [Header("问题")]
    public GameObject [] questions;
    [Header("选项答案")]
    public string [][] answer;
    [Header("题目数量")]
    public int maxNum = 10;
    [Header("题目分数")]
    public int shootRightScore = 5;
    [Header("当前题目已回答的空")]
    public int answerHole = -1;
    [Header("当前题目已回答的题目数")]
    public int answerCount = 0;

    private GameObject questionT;
    
    void Awake()
    {
        instance = this;

        answer = new string[2][];
        answer[0] = Answer1.text.Split(' ');
        answer[1] = Answer2.text.Split(' ');
        maxNum = answer[0].Length;
    }

    void Start()
    {
        Color tColor = rightI1.color;
        tColor.a = 0;
        rightI1.color = tColor;
        errorI1.color = tColor;

        tColor = rightI2.color;
        tColor.a = 0;
        rightI2.color = tColor;
        errorI2.color = tColor;

        tColor = answer1T.color;
        tColor.a = 0;
        answer1T.color = tColor;

        tColor = answer2T.color;
        tColor.a = 0;
        answer2T.color = tColor;
        
        questionT = Instantiate<GameObject>(questions[0], questionPos, Quaternion.identity);
        questionT.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        questionT.transform.localScale = Vector3.one;
        answerHole = -1;
        answerCount = 0;
    }

    void Update()
    {
        numT.text = "number: " + number.ToString();
        scoreT.text = "score: " + score.ToString();    
    }

    public void GetAnswer(int holeNum, int stoneNum)
    {
        answerCount++;
        answerHole = holeNum;
        string ans = "";
        switch (stoneNum)
        {
            case 0:
                ans = "adj";
                break;
            case 1:
                ans = "adv";
                break;
            case 2:
                ans = "noun";
                break;
            case 3:
                ans = "verb";
                break;
        }
        if (answer[holeNum][number-1] == ans)
        {
            RightAnswer(stoneNum, answerHole, answerCount);
        }
        else
        {
            ErrorAnswer(stoneNum, answerHole, answerCount);
        }
    }

    public void RightAnswer(int stoneNum, int answerHole, int answerCount)
    {
        switch (answerHole)
        {
            case 0:
                StartCoroutine(FadeIn(rightI1));
                break;
            case 1:
                StartCoroutine(FadeIn(rightI2));
                break;
        }
        if (answerCount == 1)
        {
            rightA.Play();
            score += shootRightScore;
            StaticData.score = score;
            scoreT.text = "score: " + score.ToString();
            StoneStay(stoneNum, answerCount);
        }
        else if (answerCount == 2)
        {
            rightA.Play();
            score += shootRightScore;
            StaticData.score = score;
            scoreT.text = "score: " + score.ToString();
            StoneStay(stoneNum, answerCount);
            StartCoroutine(FadeAnswer());
            StartCoroutine(NextQuestion(number));
            StartCoroutine(StoneUnselected());
        }
    }

    public void ErrorAnswer(int stoneNum, int answerHole, int answerCount)
    {
        switch (answerHole)
        {
            case 0:
                StartCoroutine(FadeIn(errorI1));
                break;
            case 1:
                StartCoroutine(FadeIn(errorI2));
                break;
        }
        if (answerCount == 1)
        {
            errorA.Play();
            StoneStay(stoneNum, answerCount);
        }
        else if (answerCount == 2)
        {
            errorA.Play();
            StoneStay(stoneNum, answerCount);
            StartCoroutine(FadeAnswer());
            StartCoroutine(NextQuestion(number));
            StartCoroutine(StoneUnselected());
        }
    }

    public void StoneStay(int stoneNum, int answerCount)
    {
        hole[answerHole].ReleaseStone();
        StartCoroutine(ShowAnswer());
    }

    public IEnumerator NextQuestion(int num)
    {
        yield return new WaitForSeconds(1.5f);

        if (num >= maxNum)
        {
            FadeIntoNextScene.instance.LoadNextScene();
        }
        yield return new WaitForSeconds(1f);

        answerHole = -1;
        answerCount = 0;
        number++;

        Destroy(questionT);
        questionT = Instantiate<GameObject>(questions[num], questionPos, Quaternion.identity);
        questionT.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        questionT.transform.localScale = Vector3.one;

        GameObject go = questionT;
        Vector2 t1 = go.transform.Find("w1").transform.position;
        Vector2 t2 = go.transform.Find("w2").transform.position;
        hole[0].transform.position = t1 + (Vector2)Vector2.down;
        hole[1].transform.position = t2 + (Vector2)Vector2.down;

        for (int i = 0; i < stone.Length; i++)
        {
            stone[i].unselectable = false;
        }
    }

    public IEnumerator FadeIn(Image i)
    {
        Color tColor = i.color;
        float fadeOutDuration = 1f;
        float fadeOutDone = Time.time + fadeOutDuration;
        while (Time.time < fadeOutDone)
        {
            tColor.a = 1 - (fadeOutDone - Time.time) / fadeOutDone;
            i.color = tColor;
            yield return null;
        }
        tColor.a = 1;
        i.color = tColor;

        StartCoroutine(FadeOut(i));
    }

    public IEnumerator FadeOut(Image i)
    {
        Color tColor = i.color;
        float fadeOutDuration = 1f;
        float fadeOutDone = Time.time + fadeOutDuration;
        while (Time.time < fadeOutDone)
        {
            tColor.a = (fadeOutDone - Time.time) / fadeOutDone;
            i.color = tColor;
            yield return null;
        }
        tColor.a = 0;
        i.color = tColor;
    }

    public IEnumerator StoneUnselected()
    {
        for (int i = 0; i < 4; i++)
        {
            stone[i].unselectable = true;
        }

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < 4; i++)
        {
            stone[i].unselectable = false;
        }

    }

    public IEnumerator ShowAnswer()
    {
        yield return new WaitForSeconds(0.5f);

        if (answerHole == 0)
        {
            answer1T.text = answer[0][number-1];
            answer1T.transform.position = hole[0].transform.position;

            float fadeDuration = 1f;
            float fadeDone = Time.time + fadeDuration;
            Color tColor = answer1T.color;
            while (Time.time < fadeDone)
            {
                tColor.a = (1 - (fadeDone - Time.time)) / fadeDuration;
                answer1T.color = tColor;
                yield return null;
            }
            tColor.a = 1;
            yield return new WaitForSeconds(1f);
        }
        else if (answerHole == 1)
        {
            answer2T.text = answer[1][number-1];
            answer2T.transform.position = hole[1].transform.position;

            float fadeDuration = 1f;
            float fadeDone = Time.time + fadeDuration;
            Color tColor = answer1T.color;
            while (Time.time < fadeDone)
            {
                tColor.a = (1 - (fadeDone - Time.time)) / fadeDuration;
                answer2T.color = tColor;
                yield return null;
            }
            tColor.a = 1;
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator FadeAnswer()
    {
        yield return new WaitForSeconds(1.5f);

        float fadeDuration = 0.5f;
        float fadeDone = Time.time + fadeDuration;
        Color tColor = answer1T.color;
        while (Time.time < fadeDone)
        {
            tColor.a = (fadeDone - Time.time) / fadeDuration;
            answer1T.color = tColor;
            answer2T.color = tColor;
            yield return null;
        }
        tColor.a = 0;
    }
}