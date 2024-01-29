using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public Light2D lightInScene;

    // Start is called before the first frame update
    void Start()
    {
        lightInScene = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RandomIntensity();
    }

    private void RandomIntensity()
    {
        lightInScene.intensity = Mathf.PingPong(1f, 10f);
    }
}
