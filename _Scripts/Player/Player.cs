using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Player script holds all player actions except for:
/// movement (like dash)
/// attacking (like melee combat and shooting projectile)
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;

    [Header("Health")]
    [SerializeField] private Health health;
    public GameObject damageText;

    [Header("SFX")]
    [SerializeField] private SFX sfx;

    private ParticleSystem ps; //particles for damage hit indication
    private CameraShake shake;


    private void Start()
    {
        //makes sure the same player object stays in the run
        DontDestroyOnLoad(this.gameObject);

        //finds and get all the component needed for the player to function
        health = GetComponent<Health>();
        sfx = GameObject.FindGameObjectWithTag("SFX").GetComponent<SFX>();
        shake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
        ps = GameObject.FindGameObjectWithTag("BloodVFX").GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Call this when attack animation is needed
    /// Add the int for which attack animation to call
    /// </summary>
    /// <param name="attackAnimation"></param>
    public void PlayerAttack(int attackAnimation)
    {
        animator.SetInteger("RandomAttack", attackAnimation);
    }

    /// <summary>
    /// Call this when the player dies
    /// Shows death animation, destroy player to not interfere with main menu
    /// and the loads the main menu scene
    /// </summary>
    public void PlayerDeath()
    {
        animator.SetTrigger("DeathTrigger");
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Call this when the player needs to lose health
    /// add int of how much health the player is to lose on hit
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        //player lose health based on how much int of damage
        health.currentHealth -= damage;

        /*instantiate the damage popup when the player takes damage
         * the damage text displayed = to the int of damage*/
        DamagePopup indicator = Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamagePopup>();
        indicator.transform.SetParent(this.transform);
        indicator.SetDamageText(damage);

        /*checks if the health of the player is 0 or less
         * this idicates the player being dead and calls the PlayerDeath() method*/
        if (health.currentHealth <= 0)
        {
            PlayerDeath();
        }
        //creates a camera shake when the player gets hit
        shake.CamShake();
        //plays the sound
        sfx.TakeDamageSFX();
        //and the plays the damage hit indication effect
        ps.Play();
    }

    public void InstantiateInPlayer(GameObject prefab, Transform transform)
    {
        Instantiate(prefab, transform);
    }
}
