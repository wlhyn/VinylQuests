using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPRotating : MonoBehaviour{
    public float rotationSpeed;
    private bool isRotating;
    private bool hasCollided;

    private Rigidbody2D rb;
    public GameController gameController;

    void Start(){
        isRotating = true;
        hasCollided = false;

        rb = GetComponent<Rigidbody2D>();
        gameController = FindObjectOfType<GameController>();
        GetComponent<Collider2D>().enabled = false;
    }

    void Update(){
        if(gameController.gameOver){
            StopRotating();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            StopRotating();
            StartCoroutine(FailCollisionCheck());
        }

        if(isRotating){
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    private void StopRotating(){
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        isRotating = false;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(isRotating){
            return;
        }

        if(other.CompareTag("pin")){
            hasCollided = true;
            gameController.SuccessRound(true);
        }
    }

    private IEnumerator FailCollisionCheck(){
        hasCollided = false;
        yield return new WaitForSeconds(0.05f);

        if (!hasCollided){
            gameController.SuccessRound(false);
        }
    }   
    
}