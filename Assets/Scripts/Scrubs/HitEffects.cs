using UnityEngine;

[CreateAssetMenu(fileName = "HitEffects", menuName = "Scriptable Objects/HitEffects")]

public class HitEffects : ScriptableObject
{
    public GameObject[] hitEffects;
    public GameObject[] lingeringEffects;
    public Material[] lingeringEffectMaterials;
    
    public GameObject[] reactionEffects;
    public float[] reactionMultiplier;

    //public LingeringElements
}
