using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float Width = 500;
    private Image bar; 
    private Player player;
    private float playerMaxHealth;
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
        bar.rectTransform.localScale = new Vector3(player.Health / playerMaxHealth, 1, 1);
    }
}
