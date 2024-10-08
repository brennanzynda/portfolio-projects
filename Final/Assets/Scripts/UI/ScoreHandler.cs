using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    private int score;
    public int pointsPerFrameInTrigger;
    public Text tmp;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
    }

    public void AddScore()
    {
        score += pointsPerFrameInTrigger;
    }

    private void UpdateScore()
    {
        tmp.text = score.ToString();
    }
}
