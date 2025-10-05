using System;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damageValue;

    private void Start()
    {
        gameObject.layer = transform.parent.gameObject.layer;
    }
}
