using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image bar; 
    private Player player;
    private float playerMaxHealth;
    private float currentPercent = 1;
    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerMaxHealth = player.Health;
    }

    // Update is called once per frame
    void Update()
    {
        bar.fillAmount = currentPercent;
        currentPercent = Mathf.Lerp(currentPercent, player.Health / playerMaxHealth, .08f);
        //Mathf.Lerp(currentPercent, player.Health / playerMaxHealth, 0);
        //currentPercent = player.Health / playerMaxHealth;
    }
}
