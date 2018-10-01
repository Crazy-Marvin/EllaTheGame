using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour {
    enum PlayerState
    {
        Running,
        Damaged,
        Jumping,
        Dead
    }
    private PlayerState playerState;
    [SerializeField]
    public float  jumpForce, maxMovementSpeed=7.5f, speedIncreaseAmount = 0.1f;
    private float movementSpeed = 0;
    private Rigidbody2D playerRB;
    public bool grounded;
    public LayerMask groundLayer;
    private Collider2D playerCollider;
    public Animator playeranimator;
    public bool isJumpAnimationPlaying;
    private int health=100;
    private int score = 0;
    private const int maxHealth = 100;
    private bool isPlayerDeath = false;
    private LevelGlobals levelGlobals;
    private AudioSource audioSource;
    EventSystem eventSystem;
    public AudioClip[] audioClips;
    
    
    // private
    // Use this for initialization
    void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playeranimator = GetComponent<Animator>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        levelGlobals = GameObject.FindGameObjectWithTag("LevelGlobalGO").GetComponent<LevelGlobals>();
        audioSource = GetComponent<AudioSource>();
        playerState = PlayerState.Running;
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            if (movementSpeed < maxMovementSpeed)
            {
                movementSpeed += speedIncreaseAmount;
            }
            playeranimator.SetFloat("Speed", movementSpeed);
            if (playerState != PlayerState.Dead)
            {
                grounded = Physics2D.IsTouchingLayers(playerCollider, groundLayer);
                playerRB.velocity = new Vector2(movementSpeed, playerRB.velocity.y);
                if (jumping())
                {
                    if (grounded && playerState == PlayerState.Running)
                    {
                        playerState = PlayerState.Jumping;
                        audioSource.clip = audioClips[1]; // 1 for Jumping Sound
                        audioSource.Play();
                        playerRB.velocity = new Vector2(playerRB.velocity.y, jumpForce);
                        playeranimator.SetBool("Jumping", true);
                    }
                }
            }
        }
       
	}
    public void stopJumping()
    {
        playerState = PlayerState.Running;
        playeranimator.SetBool("Jumping", false);
    }
    public void changeHealth(int value)
    {
        if (GameManager.Instance.gameState == GameManager.GameState.GameRunning)
        {
            health += value;
            inGameUI.Instance.setHealthAmount(health);
            playerState = PlayerState.Damaged;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            else if (health <= 0)
            {
                playerState = PlayerState.Dead;
                GameManager.Instance.ExecuteGameOverEvent();
                playeranimator.SetBool("isDeath", true);
                playerRB.velocity = new Vector2(30, playerRB.velocity.y);
            }
            playeranimator.SetBool("Damaged", true);
            movementSpeed = 0.1f;
            audioSource.clip = audioClips[0]; // 0 for the Dog in pain Sound
            audioSource.Play();
        }

    }
    public void stopDeadAnimation()
    {
        playeranimator.SetBool("isDeath", false);
    }
    private int damageAppliedCounter=0;
    public void removeDamageEffect()
    {
        
        if (damageAppliedCounter == 3)
        {
            damageAppliedCounter++;
        }
        else
        {
            playerState = PlayerState.Running;
            playeranimator.SetBool("Damaged", false);
        }
    }
    public void changeScore(int value)
    {
        score += value;
        inGameUI.Instance.setScoreAmount(score);
        GameManager.Instance.SetMatchScore(score);
    }
    private bool jumping()
    {
        bool isJumping=false;

        if(Input.GetKeyDown(KeyCode.Space)){
            isJumping = true;
        }

        if (Input.touchCount > 0 && eventSystem.currentSelectedGameObject == null)
        {
                Touch myTouch = Input.touches[0];
                if (myTouch.phase == TouchPhase.Began)
                {
                    isJumping = true;
                }
        }
        return isJumping;
    }
}
