using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //reference to player used to find projectiles. Player ref for child class
    public GameObject player, enemyProjectileObject;
    //spawn point for enemy projectile
    public Transform enemyProjectileSpawnPointF, enemyProjectileSpawnPointB;
    //declares speed variable, set in child class
    public float speed;
    //State machine for enemy direction
    public enum EnemyFacing { f, b}
    public EnemyFacing enemyDirection;
    // timer for shooting
    private float shootTimer;
    //check if child class is able to shoot
    //public bool canShoot = true;
    //check if enemy is boss or ranged
    public bool isBoss, isRanged = false;
    //ref to audio source
    [SerializeField] private AudioSource rangedAttackSound, bossAttackSound;

    //State machine for enemy state 
    public enum EnemyAiSM { idle, chasing, shooting}
    public EnemyAiSM enemyAiSM;

    //private float enemyProjectileDamage;


    // Start is called before the first frame update
    void Start()
    {
        //On game run, look for instiated player actor
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected void Update()
    {
        //Check player distance relavtive to enemy
        //Debug.Log("Player is" + Vector3.Distance(transform.position, player.transform.position) + " from " + gameObject.name);

        //State machine conditions
        switch (enemyAiSM)
        {
            //Idle State
            case EnemyAiSM.idle:
                //Wait for the player to get close enough
                if(Vector3.Distance(transform.position, player.transform.position) < 10f)
                {
                    //Player is close enough so switch to chasing
                    enemyAiSM = EnemyAiSM.chasing;
                }
                break;

            //Chasing State
            case EnemyAiSM.chasing:
                //Chase the player 
                ChasePlayer();

                //If close enough, shoot at the player
                if (Vector3.Distance(transform.position, player.transform.position) < 5f)
                {
                    enemyAiSM = EnemyAiSM.shooting;
                }
                break;

            //Shooting State
            case EnemyAiSM.shooting:
                //Shoot projectiles on timer
                if (shootTimer >= 1.5f)
                {
                    if (isRanged == true)
                    {
                        rangedAttackSound.Play();
                        RangedAttack();
                        shootTimer = 0f;
                    }
                    if (isBoss == true)
                    {
                        bossAttackSound.Play();
                        RangedAttack();
                        shootTimer = 0f;
                    }
                   
                }
                else
                {
                    shootTimer += Time.deltaTime;
                }

                //if the player is too far away, return to chase
                if (Vector3.Distance(transform.position, player.transform.position) > 7f)
                {
                    enemyAiSM = EnemyAiSM.chasing;
                }
                break;

        }
    }

    private void ChasePlayer()
    {
        //on the x axis check if enemy is in front or behind player
        //on the y axis check if enemy is in above or below player
        float x = transform.position.x - player.transform.position.x;
        float y = transform.position.y - player.transform.position.y;

        //Check if x is less than 0, and move right. If x is more than 0 move character left
        if (x < 0)
        {
            MoveRight(1);
            //set enemy direction to forward
            enemyDirection = EnemyFacing.f;
        }

        else
        {
            MoveRight(-1);
            //set enemy direction to backward
            enemyDirection = EnemyFacing.b;
        }

        //Check if y is less than 0, and move up. If y is more than 0 move character down
        if (y < 0)
        {
            MoveUp(1);
        }

        else
        {
            MoveUp(-1);
        }

    }

    private void MoveRight(float backwards)
    {
        //clamp backwards value to either -1 of 1 to preventing putting in higher / lower numbers
        backwards = Mathf.Clamp(backwards, -1, 1);
        //checking clamp works
        //Debug.Log(backwards);
        //move forward along the X axis (Fowards = X = 1, backwards = -1) (X, Y, Z)
        transform.Translate(backwards * speed * Time.deltaTime, 0, 0);
       
    }

    private void MoveUp(float upwards)
    {
        //clamp backwards value to either -1 of 1 to preventing putting in higher / lower numbers
        upwards = Mathf.Clamp(upwards, -1, 1);
        //checking clamp works
        //Debug.Log(upwards);
        //move forward along the Y axis (Upwards = X = 1, downwards = -1) (X, Y, Z)
        transform.Translate(0, upwards * speed * Time.deltaTime, 0);

    }

    private void RangedAttack()
    {
        
        //set enemy projectile spawn point, depending on their facing direction
        switch (enemyDirection)
        {
                case EnemyFacing.f:
                //if facing foward, spawn projectile at foward spawn point
                    Instantiate(enemyProjectileObject, enemyProjectileSpawnPointF.transform.position, 
                        enemyProjectileSpawnPointF.transform.rotation);
                    break;

                case EnemyFacing.b:
                //if facing backwards, spawn projectile at back spawn point
                Instantiate(enemyProjectileObject, enemyProjectileSpawnPointB.transform.position, 
                    enemyProjectileSpawnPointB.transform.rotation);
                    break;
        }
      
        
    }
}
