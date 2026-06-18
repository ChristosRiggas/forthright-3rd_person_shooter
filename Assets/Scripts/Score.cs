using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Score : MonoBehaviour
{
    public TMP_Text scoreText;
    public bool scored = false;

    private int P0Score, P1Score;

    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        scoreText.text = "0  0";
    }

    private void Update()
    {
        scoreText.text = P0Score + "  " + P1Score;
    }

    public void AddScoreToOther(int user)
    {
        if (!scored)
        {
            if (user == 0)
            {
                P1Score++;
                scored = true;
            }
            else
            {
                P0Score++;
                scored = true;
            }
        }
    }

    public void AddTargetScore(int user)
    {
        if (!scored)
        {
            if (user == 0)
            {
                P0Score++;
                scored = true;
            }
            else
            {
                P1Score++;
                scored = true;
            }
        }
    }
}
