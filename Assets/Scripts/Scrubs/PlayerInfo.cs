using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Objects/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public Upgrade[] rightUpgrades;
    public Upgrade[] leftUpgrades;
}
