using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] float speed;
    [SerializeField] int attackPower;
    [SerializeField] float Cooldown;
    [SerializeField] bool isRanged;
    [SerializeField] Projectile projectile;
    [SerializeField] float attackRange, projectileSpeed;
    private float attackCountdown;
    private float horizontal;
    private float vertical;
    private Health health;
    private Vector2 currentPos;
    private bool isAlive = true; //to check if enemy is alive so that movement can be stopped when not alive
    private bool isAttacking = false;

    [Header("Room")]
    private float xMax, xMin, yMax, yMin;
    private Room room;

    [Header("Player")]
    Player player;
    private PlayerController playerController;
    private Transform playerPos;

    [Header("Animation")]
    public Animator animator;

    [Header("Effects")]
    private ParticleSystem ps; //damage indicator splash effect
    public GameObject damageText; //damage for the damage popup indicator

    // Start is called before the first frame update
    void Start()
    {
        /*Getting the room in the scene and setting boundaries for movement
         * by assigning the current scene boundries as the boundries to use*/
        room = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Room>();
        xMax = room.maxX;
        xMin = room.minX;
        yMax = room.maxY;
        yMin = room.minY;
        gameObject.transform.parent = null;

        //Finds the player by looking for the tag Player and getting the script player so we can use it to deal damage to player etc
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        //Finds the health script component on the enemy object
        health = GetComponent<Health>();
        //Finds the animator component on the enemy object
        animator = GetComponent<Animator>();
        //Finds the particlesystem(child) that is attached as a child object to the enemy(parent) object
        ps = gameObject.GetComponentInChildren<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            //if the enemy is dead then do not continue movement
            return;
        }

        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            Debug.Log("Update found player at: " + player.transform.position);
        }
        currentPos = transform.position;
        playerPos = player.transform;

        //Finding direction to player
        if (playerPos.position.y - currentPos.y > 0)
        {
            vertical = 1;
        }
        else if (playerPos.position.y - currentPos.y < 0)
        {
            vertical = -1;
        }
        else
        {
            vertical = 0;
        }

        if (playerPos.position.x - currentPos.x > 0)
        {
            horizontal = 1;
        }
        else if (playerPos.position.x - currentPos.x < 0)
        {
            horizontal = -1;
        }
        else
        {
            horizontal = 0;
        }

        //Enforcing boundaries for movement
        if (currentPos.x > xMax)
        {
            currentPos.x = xMax;
        }
        else if (currentPos.x < xMin)
        {
            currentPos.x = xMin;
        }

        if (currentPos.y > yMax)
        {
            currentPos.y = yMax;
        }
        else if (currentPos.y < yMin)
        {
            currentPos.y = yMin;
        }

        //MOVEMENT AND ATTACKING GETS CALLED HERE
        if (!isRanged)
        {
            // Melee enemies logic
            if (Vector2.Distance(playerPos.position, currentPos) > attackRange)
            {
                // Walking logic
                if (currentPos.x < xMax && horizontal >= 0)
                {
                    animator.SetBool("isWalkingRight", true);
                    animator.SetBool("isWalkingLeft", false);
                }
                else if (currentPos.x > xMin && horizontal <= 0)
                {
                    animator.SetBool("isWalkingLeft", true);
                    animator.SetBool("isWalkingRight", false);
                }
                else
                {
                    animator.SetBool("isWalkingRight", false);
                    animator.SetBool("isWalkingLeft", false);
                }

                transform.Translate(new Vector2(horizontal, vertical) * speed * Time.deltaTime);
            }
            else
            {
                // Attack logic
                animator.SetBool("isWalkingRight", false);
                animator.SetBool("isWalkingLeft", false);
                //OnAttackAnimationHit();
            }
        }
        else if(isRanged)
        {
            // Ranged enemies logic
            if (Vector2.Distance(playerPos.position, currentPos) > attackRange)
            {
                // Walking logic
                if (currentPos.x < xMax && horizontal >= 0)
                {
                    animator.SetBool("isWalkingRight", true);
                    animator.SetBool("isWalkingLeft", false);
                }
                else if (currentPos.x > xMin && horizontal <= 0)
                {
                    animator.SetBool("isWalkingLeft", true);
                    animator.SetBool("isWalkingRight", false);
                }
                else
                {
                    animator.SetBool("isWalkingRight", false);
                    animator.SetBool("isWalkingLeft", false);
                }

                transform.Translate(new Vector2(horizontal, vertical) * speed * Time.deltaTime);
            }
            else if (Vector2.Distance(playerPos.position, currentPos) < attackRange * 0.50)
            {
                // Ranged attack logic
                animator.SetBool("isWalkingRight", false);
                animator.SetBool("isWalkingLeft", false);
                //OnAttackAnimationHit();
            }
        }
        else if (Vector2.Distance(playerPos.position, currentPos) < attackRange * 0.50 && isRanged)
        {
            if ((currentPos.x < xMax && -horizontal >= 0) || (currentPos.x > xMin && -horizontal <= 0))
            {
                if (currentPos.x < xMax && -horizontal >= 0)
                {
                    animator.SetBool("isWalkingRight", true);
                    animator.SetBool("isWalkingLeft", false);
                }
                if (currentPos.x > xMin && -horizontal <= 0)
                {
                    animator.SetBool("isWalkingLeft", true);
                    animator.SetBool("isWalkingRight", false);
                }
                transform.Translate(Vector2.right * horizontal * -speed * Time.deltaTime);
            }
            if ((currentPos.y < yMax && -vertical >= 0) || (currentPos.y > yMin && -vertical <= 0))
            {
                transform.Translate(Vector2.up * vertical * -speed * Time.deltaTime);
            }
        }
        if (attackCountdown <= 0 && Vector2.Distance(playerPos.position, currentPos) <= attackRange)
        {
            isAttacking = true;
            Attack(playerPos);
        }

        attackCountdown -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (isAttacking)
        {
            animator.SetTrigger("AttackTrigger");
            isAttacking = false;
        }
    }

    IEnumerator ResetAttackFlags(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset attack-related boolean parameters
        animator.SetBool("isAttackingLeft", false);
        animator.SetBool("isAttackingRight", false);
    }

    /// <summary>
    /// This gets called in the Attack function
    /// The triggers for the attack animations are set here
    /// The ranged attackes get set here
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackDirectionParam"></param>
    private void PerformAttack(Transform target, string attackDirectionParam)
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, target.position.z);

        //Triggers the attack animations
        animator.SetBool("isAttackingLeft", attackDirectionParam == "isWalkingLeft");
        animator.SetBool("isAttackingRight", attackDirectionParam == "isWalkingRight");
        animator.SetTrigger("AttackTrigger");

        if (isRanged)
        {
            Debug.Log("Shooting from: " + transform.position + ", towards: " + targetPos);
            Quaternion projectileRotation = Quaternion.LookRotation(Vector3.forward, targetPos - transform.position);
            Debug.Log("Projectile rotation: " + projectileRotation);
            projectile.SetTarget(targetPos, WhoToHurt.Player, projectileSpeed);
            Instantiate(projectile, transform.position + (targetPos * 0.01f), projectileRotation);
        }
        else if (!playerController.Invinsibility())
        {
            player.TakeDamage(attackPower);
        }

        StartCoroutine(ResetAttackFlags(0.5f));
    }



    void Attack(Transform target)
    {
        attackCountdown = Cooldown;

        if (animator.GetBool("isWalkingLeft"))
        {
            PerformAttack(target, "isWalkingLeft");
        }
        else if (animator.GetBool("isWalkingRight"))
        {
            PerformAttack(target, "isWalkingRight");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        health.currentHealth -= amount;
        ps.Play();

        DamagePopup indicator = Instantiate(damageText, currentPos, Quaternion.identity).GetComponent<DamagePopup>();
        indicator.transform.SetParent(this.transform);
        indicator.SetDamageText(amount);

        if (health.currentHealth <= 0)
        {
            attackCountdown = 5; //so that the enemy wont accidentaly attack while the death animation plays
            StartCoroutine(WaitToDie());
        }
    }

    /// <summary>
    /// So the death animation wont trigger to fast after hitting 0 health
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitToDie()
    {
        isAlive = false;
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isRanged)
        {
            if (attackCountdown <= 0)
            {
                Attack(other.transform);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isRanged)
        {
            if (attackCountdown <= 0)
            {
                Attack(other.transform);
            }
        }
    }
}
