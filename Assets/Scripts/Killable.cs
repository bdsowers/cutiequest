using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public enum DeathResponse
    {
        Destroy,
        MakeInactive,
    }

    public delegate void Died(Killable entity);
    public event Died onDeath;

    public int health;
    public DeathResponse deathResponse;

    public void TakeDamage(int damage)
    {
        // todo bdsowers - where to factor in defense?

        // If we're talking about the player, use their info that's stashed away in a saveable place
        if (GetComponent<PlayerController>())
        {
            Game.instance.playerData.health -= damage;
        }

        health -= damage;
        if (health <= 0f)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (onDeath != null)
        {
            onDeath(this);
        }

        if (deathResponse == DeathResponse.Destroy)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
