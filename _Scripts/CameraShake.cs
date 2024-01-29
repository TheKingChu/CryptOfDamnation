using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CameraShake : MonoBehaviour
{
    [SerializeField] private Animator camAnimator;

    private void Start()
    {
        camAnimator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
    }

    public void CamShake()
    {
        camAnimator.SetTrigger("Shake");
    }
}
