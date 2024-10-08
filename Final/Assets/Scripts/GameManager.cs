using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public List<Particle2D> asteroids;
    public GameObject player;

    public List<Vector2> startingPositions;
    public List<Vector2> startingVelocities;

    public float secondsBeforeReset = 5.0f;

    public Text winText, loseText;
    // Use this for initialization
    void Start ()
    {
        instance = this;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Asteroid");
        for (int i = 0; i < objects.Length; i++)
        {
            asteroids.Add(objects[i].GetComponent<Particle2D>());
            startingPositions.Add(asteroids[i].position);
            startingVelocities.Add(asteroids[i].velocity);
        }
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("ResetAsteroidsToTop", 0.0f, secondsBeforeReset);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.timeScale == 0.0f) //if paused
        {
            if(Input.GetKey(KeyCode.R))
            {
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reset
            }
        }
	}

    void ResetAsteroidsToTop()
    {
        for (int i = 0; i < asteroids.Count; i++)
        {
            asteroids[i].position = startingPositions[i];
            asteroids[i].position.x = GetRandomX();
            asteroids[i].velocity = startingVelocities[i];
        }
    }

    float GetRandomX()
    {
        float xMin = transform.position.x - 0.5f * transform.localScale.x;
        float xMax = transform.position.x + 0.5f * transform.localScale.x;

        return Random.Range(xMin, xMax);
    }
    public void EndGame()
    {
        Time.timeScale = 0.0f; //pause game
        loseText.gameObject.SetActive(true);
        //Debug.Log("lose");
    }

    public void WinGame()
    {
        Time.timeScale = 0.0f; //pause game
        winText.gameObject.SetActive(true);
        //Debug.Log("win");
    }
}
