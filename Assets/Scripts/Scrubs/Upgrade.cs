using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades", menuName = "Scriptable Objects/Upgrades")]
public class Upgrade : ScriptableObject
{
    public float atkBonus, atkMultiplier = 1;
    public GameObject spawningPrefab;
    public float despawningTime = .1f;
    public int jumpAbility = 0;
}
