using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;




/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    public ParticleSystem dustParticles;
    [SerializeField] private SFX sfx;
    [SerializeField] private GameObject pausemenu;
    [SerializeField] private Player player;
    public Health health;
    private Room room;
    private GameObject inventory;

    //Attacking and type
    public int attackPower;
    [SerializeField] float attackCooldownTime;
    public bool vampire, dasher, sniper, hunter, protector, helmet, chestplate, boots, sword;
    public List<Av3> equipment;
    public List<Curse> currentCurses;
    public Av3 defaultHelmet, defaultChestplate, defaultBoots, defaultSword;
    [SerializeField] float curseCooldownTime, speed, dashSpeed, dashTime, dashCooldownTime, dashRecoverSpeed;
    private float actualDashCooldown, actualMagicCooldown, actualCurseCooldown, currentSpeed, horizontalInput, verticalInput, attackTimer;
    private float xMin, xMax, yMin, yMax;
    public bool isAttacking, hasPaused;
    
        //Projectile
    [SerializeField] Projectile magic;
    [SerializeField] float magicCooldownTime, magicSpeed, snipeSpeed, snipeDelay;
   

    //artifact cooldowns
    [Header("Helmet")]
    public Image helmetImage;
    bool isCooldown1 = false;
    public float cooldown1 = 10;

    [Header("Chestplate")]
    public Image chestplateImage;
    //bool isCooldown2 = false;
    public float cooldown2 = 10;

    [Header("Boots")]
    public Image bootsImage;
    bool isCooldown3 = false;
    public float cooldown3 = 10;

    [Header("Sword")]
    public Image swordImage;
    //bool isCooldown4 = false;
    public float cooldown4 = 10;

    /*Example for UI cooldown on artifact abilities
     * private void WhereTheAbilitesAreUsed()
     *  {
     *     if (whatever input the artifact uses && isCooldown == false)
     *     {
     *         isCooldown = true;
     *         helmetImage.fillAmount = 1;
     *     }
     *     
     *     if (isCooldown)
     *     {
     *          helmetImage.fillAmount -= 1 / helmetCooldown * Time.deltaTime;
     *          
         *      if(helmetImage.fillAmount <= 0)
         *      {
         *          helmetImage.fillAmount = 0;
         *           isCooldown = false;
         *      }
     *     }
     *  }
     */


    // Start is called before the first frame update
    void Start()
    {
        equipment = new List<Av3>(4);
        equipment.Insert(0, defaultHelmet);
        equipment.Insert(1, defaultChestplate);
        equipment.Insert(2, defaultBoots);
        equipment.Insert(3, defaultSword);
        helmet = false;
        chestplate = false;
        boots = false;
        sword = false;

        /*gets the room object in the scene and setting boundaries for movement
         * by setting the room values for the current scene*/
        room = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Room>();
        xMax = room.maxX;
        xMin = room.minX;
        yMax = room.maxY;
        yMin = room.minY;

        player = GetComponent<Player>();
        health = GetComponent<Health>();
        inventory = this.gameObject.transform.GetChild(1).GetChild(2).gameObject;

        sfx = GameObject.FindGameObjectWithTag("SFX").GetComponent<SFX>();

        //cooldown ui images
        bootsImage.fillAmount = 0;
        helmetImage.fillAmount = 0;
        chestplateImage.fillAmount = 0;
        swordImage.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        PauseMenu();

        /*checks if the artifact chest is in the scene or not
         * if there are no chest in the scene then the player methods should run*/
        if (GameObject.FindGameObjectWithTag("Chest") == null)
        {
            AnimationCaller();
            PlayerMovement();
            PlayerDash();
            PlayerAttack();

            //Cooldown timers
            if (vampire || protector)
            {
                if (actualCurseCooldown < 0)
                {
                    actualCurseCooldown = curseCooldownTime;
                    if (vampire)
                    {
                        health.currentHealth--;
                    }
                }

                if (actualCurseCooldown > 0)
                {
                    actualCurseCooldown -= Time.deltaTime;
                }
            }
            if (actualDashCooldown > 0)
            {
                actualDashCooldown -= Time.deltaTime;
            }
            if (attackTimer >= 0)
            {
                attackTimer -= Time.deltaTime;
            }
            if (actualMagicCooldown > 0)
            {
                actualMagicCooldown -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlayerMovement()
    {
        Vector2 currentPos = gameObject.transform.position;

        horizontalInput = Input.GetAxis("Horizontal");
        if ((currentPos.x < xMax && horizontalInput >= 0) || (currentPos.x > xMin && horizontalInput <= 0))
        {
            transform.Translate(Vector2.right * horizontalInput * Time.deltaTime * currentSpeed);
        }
        else if (currentPos.x < xMin)
        {
            currentPos.x = xMin;
        }
        else if (currentPos.x > xMax)
        {
            currentPos.x = xMax;
        }

        verticalInput = Input.GetAxis("Vertical");
        if ((currentPos.y < yMax && verticalInput >= 0) || (currentPos.y > yMin && verticalInput <= 0))
        {
            transform.Translate(Vector2.up * verticalInput * Time.deltaTime * currentSpeed);
        }
        else if (currentPos.y < yMin)
        {
            currentPos.y = yMin;
        }
        else if (currentPos.y > yMax)
        {
            currentPos.y = yMax;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlayerDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dasher)
        {
            if (actualDashCooldown <= 0 || actualDashCooldown >= dashCooldownTime)
            {
                if (actualDashCooldown <= 0)
                {
                    actualDashCooldown = dashCooldownTime + dashTime;
                }
                if (actualDashCooldown >= dashCooldownTime - dashTime)
                {
                    currentSpeed = dashSpeed;
                }
                else
                {
                    currentSpeed += dashRecoverSpeed;
                }

                if (isCooldown3 == false)
                {
                    isCooldown3 = true;
                    bootsImage.fillAmount = 1;
                }
            }

            CreateDust();
        }
        else if (actualDashCooldown <= dashCooldownTime - dashTime)
        {
            currentSpeed = speed;

            if (isCooldown3)
            {
                bootsImage.fillAmount -= 1 / cooldown3 * Time.deltaTime;

                if (bootsImage.fillAmount <= 0)
                {
                    bootsImage.fillAmount = 0;
                    isCooldown3 = false;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlayerAttack()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") != null || GameObject.FindGameObjectWithTag("Boss") != null)
        {
            //float radius = 2f;

            if (Input.GetMouseButton(0) && attackTimer <= 0)
            {
                isAttacking = true;
                player.PlayerAttack(3);
                sfx.AttackSFX();
                attackTimer = attackCooldownTime;
            }
            else if (attackTimer > 0 && attackTimer < attackCooldownTime - 0.019)
            {
                isAttacking = false;
                player.PlayerAttack(0);
            }

            //Projectile
            if (Input.GetMouseButton(1) && (sniper || hunter) && actualMagicCooldown <= 0)
            {
                Vector3 magicTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Quaternion magicRotation = Quaternion.LookRotation((magicTarget - transform.position), Vector3.forward);

                if (isCooldown1 == false)
                {
                    isCooldown1 = true;
                    helmetImage.fillAmount = 1;

                    if (sniper && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                    {
                        
                        magic.SetTarget(magicTarget, WhoToHurt.Enemies, snipeSpeed);
                        Instantiate(magic, transform.position + (magicTarget * 0.01f), magicRotation);
                        actualMagicCooldown = magicCooldownTime * snipeDelay;
                    }

                    else if (hunter)
                    {
                        magic.SetTarget(magicTarget, WhoToHurt.Enemies, magicSpeed);
                        Instantiate(magic, transform.position, magicRotation);
                        actualMagicCooldown = magicCooldownTime;


                    }
                }
            }
            //resets cooldown for helmet
            if (isCooldown1)
            {
                helmetImage.fillAmount -= 1 / cooldown1 * Time.deltaTime;

                if (helmetImage.fillAmount <= 0)
                {
                    helmetImage.fillAmount = 0;
                    isCooldown1 = false;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PauseMenu()
    {
        Vector2 currentPos = gameObject.transform.position;

        if (Input.GetKeyDown(KeyCode.Escape) && !hasPaused)
        {
            isAttacking = false;
            Instantiate(pausemenu, currentPos, Quaternion.identity);
            hasPaused = true;
        }
        else if (FindObjectOfType<PauseMenu>() == null && Input.GetKeyDown(KeyCode.Escape) && hasPaused)
        {
            hasPaused = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }



    private void AnimationCaller()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            player.animator.SetBool("isWalking", true);
            if(Input.GetKey(KeyCode.D))
            {
                player.animator.SetBool("WalkingDirection", true);
                player.animator.SetBool("AttackDirection", false);
                player.animator.SetBool("IdleDirection", false);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                player.animator.SetBool("WalkingDirection", false);
                player.animator.SetBool("AttackDirection", true);
                player.animator.SetBool("IdleDirection", true);
            }
            else
            {
                player.animator.SetBool("isWalking", false);
            }

            CreateDust();
            sfx.WalkingSFX();
        }
        else
        {
            player.animator.SetBool("isWalking", false);
            //stop walking SFX here
        }
    }

    /*public void ModifySpeed(float newWalkingSpeed, float newDashSpeed, float duration, float cooldown, float recoverSpeed)
    {
        if(speed + newWalkingSpeed > 0)
        {
            speed += newWalkingSpeed;
        }
        if (dasher)
        {
            if (dashSpeed + newDashSpeed > 0)
            {
                dashSpeed += newDashSpeed;
            }
            if (dashTime + duration > 0)
            {
                dashTime += duration;
            }
            if (dashCooldownTime + cooldown > 0)
            {
                dashCooldownTime += cooldown;
            }
            if (dashRecoverSpeed + recoverSpeed < speed)
            {
                dashRecoverSpeed += recoverSpeed;
            }
        }


        
    }*/

    /*public void ModifyAttack(int newAttackPower, float newCooldownTime)
    {
        if (attackPower + newAttackPower >= 1)
        {
            attackPower += newAttackPower;
        }
        else
        {
            attackPower = 1;
        }

        if (attackCooldownTime + newCooldownTime > 0)
        {
            attackCooldownTime += newCooldownTime;
        }
        else
        {
            attackCooldownTime = 0.1f;
        }
        
        
    }*/

    /*public void ModifyHealth(int maxHealthChange, int currentHealthChange)
    {
        if (health.maxHealth + maxHealthChange > 0)
        {
            health.maxHealth += maxHealthChange;
        }
        if (health.currentHealth + currentHealthChange > 0)
        {
            health.currentHealth += currentHealthChange;
        }

    }*/

    /*public void ModifyRangedAttack(float speed, float cooldown)
    {
        if (sniper || hunter)
        {
            if (magicSpeed + speed > 0)
            {
                magicSpeed += speed;
            }
            if (magicCooldownTime + cooldown > 0)
            {
                magicCooldownTime += cooldown;
            }
        }
    }*/
    /*public void ModifyAllStats(float walkingSpeedMod, float dashSpeedMod, float durationMod, float dashCooldownMod, float recoverSpeedMod, float magicSpeedMod, float magicCooldownMod, int maxHealthMod, int currentHealthMod, int attackPowerMod, float attackCooldownMod)
    {
        ModifySpeed(walkingSpeedMod, dashSpeedMod, durationMod, dashCooldownMod, recoverSpeedMod);
        ModifyRangedAttack(magicSpeedMod, magicCooldownMod);
        ModifyHealth(maxHealthMod, currentHealthMod);
        ModifyAttack(attackPowerMod, attackCooldownMod);


    }*/

    /*public void UpdateCurse(bool addCurse, Curse newCurse, float curseRefreshTime)
    {
        if(newCurse == Curse.Vampire && addCurse != vampire)
        {
            vampire = addCurse;
        }
        else if(newCurse == Curse.Dasher && addCurse != dasher)
        {
            dasher = addCurse;
        }
        else if(newCurse == Curse.Sniper && addCurse != sniper)
        {
            sniper = addCurse;
        }
        else if (newCurse == Curse.Hunter && addCurse != hunter)
        {
            hunter = addCurse;
        }
        else if (newCurse == Curse.Protector && addCurse != protector)
        {
            protector = addCurse;
        }

        if(curseCooldownTime + curseRefreshTime > 0)
        {
            curseCooldownTime += curseRefreshTime;
        }
        else
        {
            curseCooldownTime = 0.5f;
        }
    }*/

    public bool Invinsibility()
    {
        if (protector && curseCooldownTime <= 0.05)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
        {
            if (isAttacking)
            {
                Debug.Log("Attack!");
                other.gameObject.GetComponent<Enemy>().TakeDamage(attackPower);
                if (vampire)
                {
                    if (health.currentHealth < health.maxHealth - attackPower / 2)
                    {
                        health.currentHealth += attackPower / 2;
                    }
                    else
                    {
                        health.currentHealth = health.maxHealth;
                    }
                }
                //Debug.LogWarning(isAttacking.ToString());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
        {
            if (isAttacking)
            {
                Debug.Log("Attack!");
                other.gameObject.GetComponent<Enemy>().TakeDamage(attackPower);
                if (vampire)
                {
                    if (health.currentHealth < health.maxHealth - attackPower / 2)
                    {
                        health.currentHealth += attackPower / 2;
                    }
                    else
                    {
                        health.currentHealth = health.maxHealth;
                    }
                }
                //Debug.LogWarning(isAttacking.ToString());
            }
        }
    }

    private void CreateDust()
    {
        dustParticles.Play();
    }

    public void Equip(Av3 artifact)
    {
        Image cursedItem = null;

        ArtifactPart type = artifact.part;
        health.currentHealth += artifact.currentHealthMod;
        if (type == ArtifactPart.Helmet)
        {
            health.maxHealth -= equipment[0].maxHealthMod;
            currentCurses.Remove(equipment[0].curse);
            equipment[0] = artifact;
            health.maxHealth += equipment[0].maxHealthMod;
            cursedItem = inventory.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            cursedItem.sprite = equipment[0].GetComponent<Image>().sprite;
            cursedItem.color = equipment[0].GetComponent<Image>().color;
        }
        else if (type == ArtifactPart.Chestplate)
        {
            health.maxHealth -= equipment[1].maxHealthMod;
            equipment[1] = artifact;
            health.maxHealth += equipment[1].maxHealthMod;
            cursedItem = inventory.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            cursedItem.sprite = equipment[1].GetComponent<Image>().sprite;
            cursedItem.color = equipment[1].GetComponent<Image>().color;
        }
        if (type == ArtifactPart.Boots)
        {
            health.maxHealth -= equipment[2].maxHealthMod;
            equipment[2] = artifact;
            health.maxHealth += equipment[2].maxHealthMod;
            cursedItem = inventory.transform.GetChild(2).GetChild(0).GetComponent<Image>();
            cursedItem.sprite = equipment[2].GetComponent<Image>().sprite;
            cursedItem.color = equipment[2].GetComponent<Image>().color;
        }
        else if (type == ArtifactPart.Sword)
        {
            health.maxHealth -= equipment[3].maxHealthMod;
            equipment[3] = artifact;
            health.maxHealth += equipment[3].maxHealthMod;
            cursedItem = inventory.transform.GetChild(3).GetChild(0).GetComponent<Image>();
            cursedItem.sprite = equipment[3].GetComponent<Image>().sprite;
            cursedItem.color = equipment[3].GetComponent<Image>().color;
        }

        curseCooldownTime = equipment[0].curseRefreshTime + equipment[1].curseRefreshTime + equipment[2].curseRefreshTime + equipment[3].curseRefreshTime;
        if(curseCooldownTime <= 0)
        {
            curseCooldownTime = 0.5f;
        }

        speed = equipment[0].walkingSpeed + equipment[1].walkingSpeed + equipment[2].walkingSpeed + equipment[3].walkingSpeed;
        if (speed < 0.5f)
        {
            speed = 0.5f;
        }

        dashSpeed = equipment[0].dashSpeed + equipment[1].dashSpeed + equipment[2].dashSpeed + equipment[3].dashSpeed;
        if (dashSpeed <= speed)
        {
            dashSpeed = speed + 0.5f;
        }

        dashTime = equipment[0].dashDuration + equipment[1].dashDuration + equipment[2].dashDuration + equipment[3].dashDuration;
        if (dashTime < 0.2f)
        {
            dashTime = 0.2f;
        }

        dashCooldownTime = equipment[0].dashCooldown + equipment[1].dashCooldown + equipment[2].dashCooldown + equipment[3].dashCooldown;
        if (dashCooldownTime < 0.5f)
        {
            dashCooldownTime = 0.5f;
        }

        dashRecoverSpeed = equipment[0].dashRecoverSpeed + equipment[1].dashRecoverSpeed + equipment[2].dashRecoverSpeed + equipment[3].dashRecoverSpeed;
        if (dashRecoverSpeed < 0.4f)
        {
            dashRecoverSpeed = 0.4f;
        }

        magicCooldownTime = equipment[0].magicCooldown + equipment[1].magicCooldown + equipment[2].magicCooldown + equipment[3].magicCooldown;
        if (magicCooldownTime < 0.1f)
        {
            magicCooldownTime = 0.1f;
        }

        magicSpeed = equipment[0].magicSpeed + equipment[1].magicSpeed + equipment[2].magicSpeed + equipment[3].magicSpeed;
        if (magicSpeed < 0.2f)
        {
            magicSpeed = 0.2f;
        }

        attackPower = equipment[0].swordDamage + equipment[1].swordDamage + equipment[2].swordDamage + equipment[3].swordDamage;
        if (attackPower < 1)
        {
            attackPower = 1;
        }

        UpdateCurse();
    }

    private void UpdateCurse()
    {
        for (int i = 0; i < equipment.Count; i++)
        {
            if (!currentCurses.Contains(equipment[i].curse))
            {
                currentCurses.Add(equipment[i].curse);
            }
        }
        if (currentCurses.Contains(Curse.Vampire))
        {
            vampire = true;
        }
        else
        {
            vampire = false;
        }

        if (currentCurses.Contains(Curse.Dasher))
        {
            dasher = true;
        }
        else
        {
            dasher = false;
        }

        if (currentCurses.Contains(Curse.Sniper))
        {
            sniper = true;
        }
        else
        {
            sniper = false;
        }

        if (currentCurses.Contains(Curse.Hunter))
        {
            hunter = true;
        }
        else
        {
            hunter = false;
        }

        if (currentCurses.Contains(Curse.Protector))
        {
            protector = true;
        }
        else
        {
            protector = false;
        }
    }
}
