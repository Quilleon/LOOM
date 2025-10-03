using System;
using System.Collections;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class DamageEffect : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    
    [SerializeField] private HitEffects effectsScrub;
    
    private float hitEffectDestroyTime = 1f, lingeringEffectTime = 7f;

    private void OnTriggerEnter(Collider other)
    {
        print("Entered");
        if (other.gameObject.CompareTag("DamageBox"))
        {
            int hitEffect;
            
            switch (other.gameObject.layer)
            {
                case 10: // Lightning
                    print("Lightning Hit");
                    hitEffect = 1;
                    break;
                case 11: // Water
                    print("Water Hit");
                    hitEffect = 2;
                    break;
                case 12: // Fire
                    print("Fire Hit");
                    hitEffect = 3;
                    break;
                case 13: // Ice
                    print("Ice Hit");
                    hitEffect = 4;
                    break;
                case 14: // Crush
                    print("Crush Hit");
                    hitEffect = 5;
                    break;
                case 15: // Pierce
                    print("Pierce Hit");
                    hitEffect = 6;
                    break;
                default:
                    print("Not elemental");
                    hitEffect = 0;
                    break;
            }
            
            SpawnHitEffect(hitEffect);
            SpawnLingeringEffect(hitEffect);
        }
    }
    
    private void SpawnHitEffect(int effectNum)
    {
        // Defaults to 0
        var hitEffect = effectsScrub.hitEffects[0];
        
        if (effectsScrub.hitEffects[effectNum] != null)
            hitEffect = effectsScrub.hitEffects[effectNum];
        
        var effect = Instantiate(hitEffect, transform.position, transform.GetChild(0).rotation);
        StartCoroutine(DestroyEffect(effect, hitEffectDestroyTime));
    }
    

    private void SpawnLingeringEffect(int lingeringEffect)
    {
        if (effectsScrub.lingeringEffects[lingeringEffect] == null)
        {
            Debug.LogError("No matching effect!");
            return;
        }
        
        
        var effect = effectsScrub.lingeringEffects[lingeringEffect];
        
        var spawnedEffect =  Instantiate(effect, transform.position, transform.GetChild(0).rotation);
        StartCoroutine(DestroyEffect(spawnedEffect, lingeringEffectTime));
    }
    
    

    private IEnumerator DestroyEffect(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        if (effect)
        {
            Destroy(effect);
        }
    }
}
