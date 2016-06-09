using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [Header("Checkpoint Info")]
    public GameObject checkpointPrefab;
    public GameObject CheckpointParent;

    [Header("User Interface")]
    public GameObject MainMenuScreen;
    public GameObject GameScreen;
    public GameObject GameOver;
    public Image LightImage;

    [Header("Player")]
    public GameObject PlayerVehicle;

    private Text currentCheckpointText;
    private int checkPointCount = 0;
    private float startTime;

    private Queue<string> trafficLight;
    private bool _gameStarted = false;
    private bool _cameraFollowCar = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }

    void Update()
    {
        if(_gameStarted)
        {
            currentCheckpointText.text = String.Format("Checkpoint {0}: {1}", checkPointCount, Math.Round(Time.time - startTime, 1));
        }
    }

    /// <summary>
    /// Handles the logic for starting the game
    /// </summary>
    public void StartGame()
    {
        
        PlayerVehicle.transform.position = new Vector3(203, 0, 128f);
        PlayerVehicle.transform.rotation = Quaternion.identity;

        MainMenuScreen.SetActive(false);
        GameScreen.SetActive(true);

        trafficLight = new Queue<string>(new string[] { "trafficLight_Red", "trafficLight_Yellow", "trafficLight_Green" });
        StartCoroutine(StartGameVisuals());
    }

    /// <summary>
    /// Handles the logic for ending the game
    /// </summary>
    public void EndGame()
    {
        _gameStarted = false;
        MainMenuScreen.SetActive(false);
        GameScreen.SetActive(true);
        currentCheckpointText.text = String.Format("Checkpoint {0}: {1}", checkPointCount, Math.Round(Time.time - startTime, 1));
    }

    /// <summary>
    /// Creates a new checkpoint on the UI for tracking the time
    /// </summary>
    public void CreateNewCheckpoint_UI()
    {
        checkPointCount++;
        GameObject checkpoint = GameObject.Instantiate(checkpointPrefab);
        currentCheckpointText = checkpoint.GetComponent<Text>();
        checkpoint.transform.SetParent(CheckpointParent.transform);
    }

    /// <summary>
    /// Handles the visuals triggered at the beginning of the game
    /// </summary>
    /// <returns></returns>
    IEnumerator StartGameVisuals()
    {

        Camera.main.GetComponent<Animator>().SetTrigger("move");
        yield return new WaitForSeconds(2.3f);

        _cameraFollowCar = true;
        GameScreen.GetComponentInChildren<Animator>().SetTrigger("toggleVisibility");
        yield return new WaitForSeconds(2f);

        while (!trafficLight.Peek().Contains("Green"))
        {
            LightImage.sprite = Resources.Load<Sprite>(trafficLight.Peek());
            trafficLight.Dequeue();

            yield return new WaitForSeconds(2f);
        }

        LightImage.sprite = Resources.Load<Sprite>(trafficLight.Peek());
        startTime = Time.time;
        CreateNewCheckpoint_UI();
        _gameStarted = true;

        yield return new WaitForSeconds(1f);

        GameScreen.GetComponentInChildren<Animator>().SetTrigger("toggleVisibility");
    }

    /// <summary>
    /// Check to see if the game has started
    /// </summary>
    /// <returns>True if started</returns>
    public bool IsGameStarted()
    {
        return _gameStarted;
    }

    /// <summary>
    /// Specifies in the camera should follow the car 
    /// </summary>
    /// <returns>True if the start animations has finished</returns>
    public bool ShouldCameraFollowCar()
    {
        return _cameraFollowCar;
    }

}
