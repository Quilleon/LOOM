using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamageEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [SerializeField] private HitEffects hitEffectsScrub;

    private void OnTriggerEnter(Collider other)
    {
        print("Entered");
        if (other.gameObject.CompareTag("DamageBox"))
        {
            switch (other.gameObject.layer)
            {
                case 10: // Lightning
                    print("Lightning Hit");
                    SpawnHitEffect(1);
                    break;
                case 11: // Water
                    print("Water Hit");
                    SpawnHitEffect(2);
                    break;
                case 12: // Fire
                    print("Fire Hit");
                    SpawnHitEffect(3);
                    break;
                case 13: // Ice
                    print("Ice Hit");
                    SpawnHitEffect(4);
                    break;
                case 14: // Crush
                    print("Crush Hit");
                    //SpawnHitEffect(hitEffects[4]);
                    break;
                case 15: // Pierce
                    print("Pierce Hit");
                    //SpawnHitEffect(hitEffects[5]);
                    break;
                default:
                    print("Not elemental");
                    SpawnHitEffect(0);
                    break;
            }
        }
    }

    private void SpawnHitEffect(int effectNum)
    {
        var effect = Instantiate(hitEffectsScrub.hitEffects[effectNum], transform.position, transform.GetChild(0).rotation);
        StartCoroutine(DestroyEffect(effect));
    }

    private IEnumerator DestroyEffect(GameObject effect)
    {
        yield return new WaitForSeconds(1f);
        Destroy(effect);
    }
}
