using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class ParticleArgs
{
    public ParticleType particleType;
    public ParticleSystem particle;
}
public enum ParticleType
{
    Explosion,
}
public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    [SerializeField] private List<ParticleArgs> particleArgs = new();
    private void Awake()
    {
        Instance = this;
    }
    public void Play(ParticleType particleType, Vector3 pos, Quaternion rot = default)
    {
        if (rot == default)
            rot = Quaternion.identity;

        Instantiate(GetParticle(particleType), pos, rot);
    }

    private ParticleSystem GetParticle(ParticleType particleType)
    {
        return particleArgs.Where(x => x.particleType == particleType).FirstOrDefault().particle;
    }
}
