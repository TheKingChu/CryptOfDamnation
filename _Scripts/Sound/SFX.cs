using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound Effect Manager
/// </summary>
public class SFX : MonoBehaviour
{
    public AudioSource[] source;
    [SerializeField] private AudioClip dmgTaken, attack, dash, dmgHit, walking;

    public void TakeDamageSFX()
    {
        if (!source[0].isPlaying)
        {
            source[0].clip = dmgTaken;
            source[0].PlayOneShot(dmgTaken);
        }
    }

    public void AttackSFX()
    {
        if(!source[1].isPlaying)
        {
            source[1].clip = attack;
            source[1].pitch = (Random.Range(0.8f, 1f));
            source[1].PlayOneShot(attack);
        }
        else
        {
            source[1].Stop();
        }
    }

    public void DashSFX()
    {
        source[2].clip = dash;
        source[2].PlayOneShot(dash);
    }

    public void WalkingSFX()
    {
        if(!source[3].isPlaying)
        {
            source[3].clip = walking;
            source[3].pitch = (Random.Range(0.7f, 1f));
            source[3].PlayOneShot(walking);
        }
    }

    public void DamageHitSFX()
    {
        if (!source[4].isPlaying)
        {
            source[4].clip = dmgHit;
            source[4].pitch = (Random.Range(0.8f, 1f));
            source[4].PlayOneShot(dmgHit);
        }
    }
}
