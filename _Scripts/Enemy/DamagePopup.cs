using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI dmgText;

    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private float minDist = 3f;
    [SerializeField] private float maxDist = 4f;
    private float timer;

    private Vector2 initialPos;
    private Vector2 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        float dist = Random.Range(minDist, maxDist);
        float rndPos = Random.Range(-5f, 5f);

        targetPos = initialPos + new Vector2(rndPos, dist);
        transform.localScale = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float fraction = lifetime / 2f;

        if(timer > lifetime)
        {
            Destroy(gameObject);
        }
        else if(timer > fraction)
        {
            dmgText.color = Color.Lerp(dmgText.color, Color.clear, (timer - fraction) / (lifetime - fraction));
        }

        transform.position = Vector2.Lerp(initialPos, targetPos, Mathf.Sin(timer / lifetime));
        transform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, Mathf.Sin(timer / lifetime));
    }

    public void SetDamageText(int damage)
    {
        dmgText.text = damage.ToString();
    }
}
