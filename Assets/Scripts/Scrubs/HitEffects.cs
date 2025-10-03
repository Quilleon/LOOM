using UnityEngine;

[CreateAssetMenu(fileName = "HitEffects", menuName = "Scriptable Objects/HitEffects")]
public class HitEffects : ScriptableObject
{
    public GameObject[] hitEffects;
    public GameObject[] lingeringEffects;
    
    public enum LingeringElements
    {
        None,
        Lightning,
        Water,
        Fire,
        Ice,
        Frozen
    }
}
