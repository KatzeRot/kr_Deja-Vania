﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    Rigidbody2D rb2D;
    Animator playerAnimator;
    SpriteRenderer playerSprite;

    enum StatusPlayer { Stop, WalkingR, WalkingL, Jumping, Enjuring }
    StatusPlayer status = StatusPlayer.Stop;

    [SerializeField] float radioOverlap = 0.5f;
    [SerializeField] LayerMask floorLayer;
    [SerializeField] Image barraMagic;
    [SerializeField] Text statusPlayer;
    float magic = 1;

    //bool jumping;

    [Header("Atributtes")]
    private const int TOTALHEALTH = 5;
    private int health = TOTALHEALTH;
    private int puntuation = 0;
    [SerializeField] float speed = 5;
    [SerializeField] float jumpForce = 10;
    //[SerializeField] int hearts;

    [Header("References")]
    [SerializeField] Text txtPuntuation;
    [SerializeField] Transform posFoot;

    // Use this for initialization
    void Start() {
        health = TOTALHEALTH;
        statusPlayer.text = "StatusPlayer: " + status.ToString(); // Unica función de mostrar en pantalla el estado del player
        rb2D = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        txtPuntuation.text = "Score: " + puntuation;
        Vector2 position = GameController.GetPosition();
        this.transform.position = position;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            magic -= 0.2f;
            print("Space PULSADO");
        }
        statusPlayer.text = "StatusPlayer: " + status.ToString(); // Unica función de mostrar en pantalla el estado del player        

        StartCoroutine("LoadContainer");

        flipSprite();
        Die();
    }
    // Update is called once per frame
    void FixedUpdate () {
        MovePlayer();
	}
    private void MovePlayer() {
        float xPos = Input.GetAxis("Horizontal");
        //float ySpeed = rb2D.velocity.y;
        float ySpeedActual = rb2D.velocity.y;
        if(Mathf.Abs(xPos) > 0.01f) {
            playerAnimator.SetBool("Running", true);
        } else {
            playerAnimator.SetBool("Running", false);   
        }

        if(status == StatusPlayer.Jumping){
            status = StatusPlayer.Stop;
            if (IsInGround() == true) {
                rb2D.velocity = new Vector2(xPos * speed, jumpForce);
            } else {
                rb2D.velocity = new Vector2(xPos * speed, ySpeedActual); // Esto sirve para que cuando el player salte puede moverse en el aire
            }
        } else {
            rb2D.velocity = new Vector2(xPos * speed, ySpeedActual);
        }
    }
    private void IncreasePuntuation(int valuePuntuation) {
        puntuation += valuePuntuation;
        txtPuntuation.text = "Score: " + puntuation;
    }
    private void flipSprite() {
        status = StatusPlayer.WalkingL;
        if (Input.GetKeyDown(KeyCode.W)) {
            status = StatusPlayer.Jumping;
            print("W PULSADA");
        } else if (Input.GetKey(KeyCode.D)) {
            status = StatusPlayer.WalkingR;
            playerSprite.flipX = false;
        } else if (Input.GetKeyDown(KeyCode.A)) {
            status = StatusPlayer.WalkingL;
            playerSprite.flipX = true;
        } else{
            status = StatusPlayer.Stop;
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        print("Tocaste " + collision.gameObject.name);

        if (collision.gameObject.name == "Coin10") {
            IncreasePuntuation(collision.gameObject.GetComponent<Coin>().TakeValue());
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.name == "Heart") {
            if(health < TOTALHEALTH) {
                //hearts[health].enabled = true;
                health++;
                Destroy(collision.gameObject);
            }
        }
    }
    public void TakeDamage(int damage) {
        health -= damage;
        if(health <= 0){
            health = 0;
        }
        
    }
    public bool Die() {
        bool isAlive = false;
        if (health == 0) {
            isAlive = true;
            print("MUERTO!!!");
        }
        return isAlive;
    }
    private bool IsInGround() {
        bool inGround = false;
        Collider2D collider = Physics2D.OverlapCircle(posFoot.position, radioOverlap, floorLayer);
        if(collider != null) {
            inGround = true;
        }
        return inGround;
    }
    IEnumerator LoadContainer() {
        if (magic <= 1f) {
            magic += 0.002f;
            barraMagic.fillAmount = magic;
        } else if (magic <= 0f) {
            magic += 0.002f;
            magic = 0f;
            
        } else if (magic >= 1f) {
            magic = 1f;
        }
            yield return null;
    }

    public int GetHealth() {
        return health;
    }
    //private bool IsInGround() {
    //    bool inGround = false;
    //    Collider2D[] cols = Physics2D.OverlapCircleAll(posFoot.position, radioOverlap);
    //    for(int i = 0; i < cols.Length; i++) {
    //        if (cols[i] != null && cols[i].gameObject.tag == "Ground") {
    //            //print("Los pies han tocado: " + cols[i].gameObject.name);
    //            inGround = true;
    //            break;
    //        }  
    //    }
    //    return inGround;
    //}
}
