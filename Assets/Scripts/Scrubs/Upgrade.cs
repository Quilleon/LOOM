using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades", menuName = "Scriptable Objects/Upgrades")]
public class Upgrade : ScriptableObject
{
    public float atkBonus, atkMultiplier;
    public GameObject spawningPrefab;
    public int jumpAbility;
}
