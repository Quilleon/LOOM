using System;
using System.Collections;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class DamageEffect : MonoBehaviour
{
    void Start()
    {
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
        
        _effectsSpawnParent = transform.GetChild(0).GetChild(0);
    }

    void Update()
    {
        
    }

    private EnemyBehaviour _enemyBehaviour;
    
    [SerializeField] private HitEffects effectsScrub;
    
    private float hitEffectDestroyTime = 1f, lingeringEffectTime = 7f;
    private int _lingeringEffect;
    private Transform _effectsSpawnParent;

    private void OnTriggerEnter(Collider other)
    {
        print("Entered");
        if (other.gameObject.CompareTag("DamageBox"))
        {
            if (_enemyBehaviour)
            {
                var incomingDamage = 20; // other.GetComponent<>()..;
                _enemyBehaviour.TakeDamage(incomingDamage);
                
            }
            
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
        
        SpawnEffect(hitEffect, hitEffectDestroyTime, false);
    }
    

    private void SpawnLingeringEffect(int lingeringEffect)
    {
        if (effectsScrub.lingeringEffects[lingeringEffect] == null)
        {
            Debug.LogError("No matching effect!");
            return;
        }

        
        if (_effectsSpawnParent.childCount > 0)
        {
            print("Removed Previous Lingering Effect");
            Destroy(_effectsSpawnParent.GetChild(0).gameObject);
        }
        
        
        var effect = effectsScrub.lingeringEffects[lingeringEffect];
        
        SpawnEffect(effect, lingeringEffectTime, true);
    }
    
    private void SpawnEffect(GameObject effect, float time, bool lingering)
    {
        var effectRotation = transform.GetChild(0).rotation;
        GameObject spawnedEffect;
        
        if (lingering)
        {
            spawnedEffect = Instantiate(effect, transform.position, effectRotation, _effectsSpawnParent);
        }
        else // Hit Effect
        {
            spawnedEffect =  Instantiate(effect, transform.position, effectRotation);
        }
        
        StartCoroutine(DestroyEffect(spawnedEffect, time));
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
