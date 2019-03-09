using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public int health;

    public void TakeDamage(int damage)
    {
        // If we're talking about the player, use their info that's stashed away in a saveable place
        if (GetComponent<PlayerController>())
        {
            Game.instance.playerData.health -= damage;
        }

        health -= damage;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
