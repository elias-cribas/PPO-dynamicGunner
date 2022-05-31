using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public ParticleSystem destroyParticles;

    public void takeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            objectDestroyed();
        }
    }

    void objectDestroyed()
    {
        Instantiate(destroyParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
