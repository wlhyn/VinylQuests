using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool gameOver;
    private bool isSuccess;
    private float successScore;
    private bool isSpawning;

    public GameObject[] lpPrefabs;
    private GameObject currentLP;
    public GameObject lpPlayer;
    public GameObject leverPrefab;
    public GameObject background;
   
    private int currentLPIndex;
    public Text successText;
    public Text failText;
    public Text scoreText;
    public Text isMissionSuccessText;

    public Button startButton;
    public Button restartButton;
    
    public CircularTimer circularTimer;

    public AudioSource[] bgm;
    public AudioSource successAudio;
    public AudioSource FailAudio;
    


    void Start(){ 
        gameOver = false;
        isSuccess = false;
        isSpawning = false;

        currentLPIndex = 0;

        startButton.onClick.AddListener(SetGameStart);
        startButton.gameObject.SetActive(true);
        restartButton.onClick.AddListener(SetGameRestart);
        restartButton.gameObject.SetActive(false);
        circularTimer.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        successText.gameObject.SetActive(false);
        failText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        isMissionSuccessText.gameObject.SetActive(false);

        lpPlayer.SetActive(false);
    }

    public void StartTimer()
    {
        circularTimer.StartTimer(); 
    }
    
    public void SetGameStart(){ 
        lpPlayer.SetActive(true);
        UpdateScoreText();
        scoreText.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);    
        circularTimer.gameObject.SetActive(true);

        if(currentLP != null){
            Destroy(currentLP);
        }
        currentLP = Instantiate(lpPrefabs[currentLPIndex]);
        currentLP.transform.position = new Vector3(-1.66f, -0.48f, -1f);

        Vector3 leverScale = leverPrefab.transform.localScale;
        leverPrefab.transform.localScale = new Vector3(leverScale.x, -leverScale.y, leverScale.z); 

        bgm[0].Play();
    }

    public void SetGameRestart(){
        bgm[0].Stop();
        bgm[1].Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetGameOver(){
        gameOver = true;
        circularTimer.PauseTimer();
        restartButton.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        
        bgm[0].Stop();

        if(successScore >= 4){
            successText.gameObject.SetActive(false);
            failText.gameObject.SetActive(false);
            isMissionSuccessText.text = "QUESTS\n COMPLETE";
            //isMissionSuccessText.fontSize = 60;
            isMissionSuccessText.color = new Color32(255, 255, 52, 255);
            isMissionSuccessText.gameObject.SetActive(true);

            bgm[1].Play();
        }else{
            successText.gameObject.SetActive(false);
            failText.gameObject.SetActive(false);       
            isMissionSuccessText.text = "QUESTS\n INCOMPLETE";
            //isMissionSuccessText.fontSize = 60;
            isMissionSuccessText.color = new Color32(255, 0, 0, 255);
            isMissionSuccessText.gameObject.SetActive(true);

            FailAudio.Play();
        }
    }


    public void SuccessRound(bool IsSuccessRound){
        if(IsSuccessRound){
            successScore++;
            UpdateScoreText();
            StartCoroutine(DisplaySuccessText());
            isSuccess = true; 

            bgm[0].Stop();
            successAudio.Play();

            if (!isSpawning){
                StartCoroutine(SpawnDelay());
            }

        }else{
            isSuccess = false; 
            bgm[0].Stop();

            StartCoroutine(DisplayFailText());
            StartCoroutine(RestartCurrentRound());
        }
    }


    IEnumerator SpawnDelay(){
        isSpawning = true;
        yield return new WaitForSeconds(1);

        if(currentLP != null){
            Destroy(currentLP);
        }

        if(currentLPIndex < lpPrefabs.Length - 1){
            currentLPIndex++;
            currentLP = Instantiate(lpPrefabs[currentLPIndex]);
            currentLP.transform.position = new Vector3(-1.66f, -0.48f, -1f);
        }else{
            gameOver = true;
            SetGameOver();
        }

        isSpawning = false;
        isSuccess = false; 

        if(successScore != 4){
            bgm[0].Play();
        }
    }


    private void UpdateScoreText(){
        scoreText.text = "Completed Quests: " + successScore.ToString();
    }

    IEnumerator DisplaySuccessText(){
        successText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        successText.gameObject.SetActive(false);
    }

    IEnumerator DisplayFailText(){
        failText.gameObject.SetActive(true);
        FailAudio.Play();

        yield return new WaitForSeconds(1);
        failText.gameObject.SetActive(false);
        if(!gameOver){
            bgm[0].Play();
        }
    }

    IEnumerator RestartCurrentRound(){
        yield return new WaitForSeconds(1);

        if(currentLP != null){
            Destroy(currentLP);
        }

        currentLP = Instantiate(lpPrefabs[currentLPIndex]);
        currentLP.transform.position = new Vector3(-1.66f, -0.48f, -1f);
    }
}
