using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLoopingParticleSystemAfterTime : MonoBehaviour
{
    public float time;
    private float mTimer;

    private void Start()
    {
        mTimer = time;
    }

    private void Update()
    {
        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; ++i)
            {
                ParticleSystem.MainModule module = particleSystems[i].main;
                module.loop = false;
            }
        }

        if (mTimer < -3f)
        {
            Destroy(gameObject);
        }
    }
}
