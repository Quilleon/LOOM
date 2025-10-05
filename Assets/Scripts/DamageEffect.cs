using System;
using System.Collections;
using UnityEngine;


public enum LingeringElements
{
    None,
    Lightning,
    Water,
    Fire,
    Ice,
    Frozen
}


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
        if (_enemyBehaviour.isDead && _effectsSpawnParent.childCount > 0)
        {
            // Destroy Lingering Effects
            //Destroy(_effectsSpawnParent.GetChild(0).gameObject);
        }
    }

    private EnemyBehaviour _enemyBehaviour;
    
    [SerializeField] private HitEffects effectsScrub;
    private float _incomingDamage, _damageMultiplier = 1f;
    
    private float hitEffectDestroyTime = 1f, lingeringEffectTime = 7f;
    private LingeringElements _incomingLingeringEffect, _activeLingeringEffect;
    private Transform _effectsSpawnParent;

    private void OnTriggerEnter(Collider other)
    {
        if (_enemyBehaviour.isDead)
            return;
        
        print("Entered");
        if (other.gameObject.CompareTag("DamageBox") && other.transform.parent.CompareTag("PlayerAttack"))
        {
            if (_enemyBehaviour)
            {
                _incomingDamage = other.GetComponent<Damage>().damageValue; // other.GetComponent<>()..;
            }
            else
            {
                print("No EnemyBehaviour!");
            }
            
            LingeringElements hitEffect = 0;
            
            switch (other.gameObject.layer)
            {
                case 10: // Lightning
                    print("Lightning Hit");
                    hitEffect = LingeringElements.Lightning;
                    break;
                case 11: // Water
                    print("Water Hit");
                    hitEffect = LingeringElements.Water;
                    break;
                case 12: // Fire
                    print("Fire Hit");
                    hitEffect = LingeringElements.Fire;
                    break;
                case 13: // Ice
                    print("Ice Hit");
                    hitEffect = LingeringElements.Ice;
                    break;
                case 14: // Crush
                    Debug.LogError("NOT IMPLEMENTED CRUSH!");
                    print("Crush Hit");
                    //hitEffect = 5;
                    break;
                case 15: // Pierce
                    Debug.LogError("NOT IMPLEMENTED PIERCE!");
                    print("Pierce Hit");
                    //hitEffect = 6;
                    break;
                default:
                    print("Not elemental");
                    hitEffect = LingeringElements.None;
                    break;
            }
            
            SpawnHitEffect(hitEffect);
            CalculateLingeringEffect(hitEffect);
        }
    }
    
    private void SpawnHitEffect(LingeringElements effectNum)
    {
        // Defaults to 0
        var hitEffect = effectsScrub.hitEffects[0];
        
        if (effectsScrub.hitEffects[(int)effectNum] != null)
            hitEffect = effectsScrub.hitEffects[(int)effectNum];
        
        SpawnEffect(hitEffect, hitEffectDestroyTime, false);
    }
    

    private void CalculateLingeringEffect(LingeringElements lingeringEffect)
    {
        if (effectsScrub.lingeringEffects[(int)lingeringEffect] == null)
        {
            Debug.LogError("No matching effect to: " + lingeringEffect);
            return;
        }

        var spawnReaction = true;
        
        if (_effectsSpawnParent.childCount > 0)
        {
            print("Calculating reaction");
            
            
            CalculateEffectReaction(lingeringEffect, _activeLingeringEffect, out spawnReaction);
            
            print("Incoming damage: " + _incomingDamage + " * Damage multiplier: " + _damageMultiplier);
            
            
            // Do not spawn reaction
            
            
            //Destroy(_effectsSpawnParent.GetChild(0).gameObject);
        }
        
        _enemyBehaviour.TakeDamage(_incomingDamage * _damageMultiplier);
        // Reset values
        _incomingDamage = 0;
        _damageMultiplier = 1;
        
        if (!spawnReaction) return;
        _activeLingeringEffect = lingeringEffect;
        var effect = effectsScrub.lingeringEffects[(int)_activeLingeringEffect];
        
        SpawnEffect(effect, lingeringEffectTime, true);
    }
    
    private void SpawnEffect(GameObject effect, float time, bool lingering)
    {
        var effectRotation = transform.GetChild(0).rotation;
        GameObject spawnedEffect;
        
        if (lingering)
        {
            spawnedEffect = Instantiate(effect, transform.position, effectRotation, _effectsSpawnParent);
            
            if (effectsScrub.lingeringEffectMaterials[(int)_activeLingeringEffect])
                _enemyBehaviour.spriteRenderer.material = effectsScrub.lingeringEffectMaterials[(int)_activeLingeringEffect];
            else
                Debug.LogError("No Lingering effect material for active effect: " + _activeLingeringEffect);
            
        }
        else // Hit Effect
        {
            spawnedEffect =  Instantiate(effect, transform.position, effectRotation);
        }
        
        StartCoroutine(DestroyEffect(spawnedEffect, time, lingering));
    }
    
    

    private IEnumerator DestroyEffect(GameObject effect, float time, bool hasMaterial)
    {
        yield return new WaitForSeconds(time);
        if (effect)
        {
            if (hasMaterial)
            {
                _enemyBehaviour.spriteRenderer.material = effectsScrub.lingeringEffectMaterials[0];
                _activeLingeringEffect = 0;
            }
            
            Destroy(effect);
        }
    }

    private void RemoveLingeringEffect()
    {
        _enemyBehaviour.spriteRenderer.material = effectsScrub.lingeringEffectMaterials[0];
        Destroy(_effectsSpawnParent.GetChild(0).gameObject);
    }


    private void CalculateEffectReaction(LingeringElements incomingEffect, LingeringElements currentEffect, out bool spawnIncomingReaction)
    {
        spawnIncomingReaction = true;
        
        switch (currentEffect)
        {
            case LingeringElements.Lightning:
                switch (incomingEffect)
                {
                    case LingeringElements.Fire:
                        ExplosionReaction();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Ice:
                        SuperConduct();
                        break;
                    default: // Lightning, Water, ice
                        print("no reaction, switch lingering effect");
                        break;
                }
                break;
            case LingeringElements.Water:
                switch (incomingEffect)
                {
                    case LingeringElements.Lightning:
                        ElectrocuteReaction();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Fire:
                        VaporizeReaction();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Ice:
                        FreezeReaction();
                        break;
                    default: // Water
                        print("no reaction, switch lingering effect");
                        break;
                }
                break;
            case LingeringElements.Fire:
                switch (incomingEffect)
                {
                    case LingeringElements.Lightning:
                        ExplosionReaction();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Water:
                        VaporizeReaction();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Ice:
                        MeltReaction();
                        spawnIncomingReaction = false;
                        break;
                    default: // fire
                        print("no reaction, switch lingering effect");
                        break;
                }
                break;
            case LingeringElements.Ice:
                switch (incomingEffect)
                {
                    case LingeringElements.Lightning:
                        SuperConduct();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Water:
                        FreezeReaction();
                        break;
                    case LingeringElements.Fire:
                        MeltReaction();
                        spawnIncomingReaction = false;
                        break;
                    default: // Ice
                        print("no reaction, switch lingering effect");
                        break;
                }
                break;
            case LingeringElements.Frozen:
                switch (incomingEffect)
                {
                    case LingeringElements.Lightning:
                        SuperConduct();
                        spawnIncomingReaction = false;
                        break;
                    case LingeringElements.Fire:
                        MeltReaction();
                        spawnIncomingReaction = false;
                        break;
                    default:
                        print("no reaction, keep frozen effect");
                        break;
                }
                break;
            default:
                Debug.LogError("Not recognized Lingering Effect: " + currentEffect);
                break;
        }
        
        
        Destroy(_effectsSpawnParent.GetChild(0).gameObject);
        spawnIncomingReaction = true;
    }

    private void ExplosionReaction() // Fire on lightning, lightning on fire
    {
        var reactionEffect = 0;
        print("Explosion Reaction");
        if (effectsScrub.reactionEffects[reactionEffect])
        {
            // Spawn Explosion
            // Does close aoe damage
            SpawnEffect(effectsScrub.reactionEffects[0], .5f, false);
            RemoveLingeringEffect();
            
            _damageMultiplier = effectsScrub.reactionMultiplier[reactionEffect];
        }
    }
    
    private void ElectrocuteReaction() // lightning on water
    {
        var reactionEffect = 1;
        print("Electrocute Reaction");
        if (effectsScrub.reactionEffects[reactionEffect])
        {
            // Spawn electrocute reaction
            // Reaction sends it forward
            SpawnEffect(effectsScrub.reactionEffects[0], .5f, false);

            _damageMultiplier = effectsScrub.reactionMultiplier[reactionEffect];
        }
    }

    private void VaporizeReaction() // Fire on water, water on fire
    {
        var reactionEffect = 2;
        
        print("Vaporize Reaction");
        if (effectsScrub.reactionEffects[reactionEffect])
        {
            // Spawn vaporize reaction
            SpawnEffect(effectsScrub.reactionEffects[0], .5f, false);
            
            RemoveLingeringEffect();
            
            _damageMultiplier = effectsScrub.reactionMultiplier[reactionEffect];
        }
        
        // Do damage
        
    }
    
    private void MeltReaction() // Ice on Fire, Fire on ice
    {
        var reactionEffect = 3;
        
        print("Melt Reaction");
        if (effectsScrub.reactionEffects[reactionEffect])
        {
            // Do 2x damage
            
            // Spawn Melt reaction
            SpawnEffect(effectsScrub.reactionEffects[0], .5f, false);
            
            RemoveLingeringEffect();
            
            _damageMultiplier = effectsScrub.reactionMultiplier[reactionEffect];
        }
    }

    private void SuperConduct() // Lightning on ice, lightning on freeze
    {
        var reactionEffect = 4;
        print("SuperConduct Reaction");
        if (effectsScrub.reactionEffects[0])
        {
            // Spawn Superconduct reaction
            // This does large aoe damage, but not that much damage
            SpawnEffect(effectsScrub.reactionEffects[0], .5f, false);
            
            _damageMultiplier = effectsScrub.reactionMultiplier[reactionEffect];
        }
    }
    
    private void FreezeReaction() // Ice on water
    {
        
        print("Freeze Reaction");
        if (effectsScrub.lingeringEffects[(int)LingeringElements.Frozen])
        {
            // Change material
            _enemyBehaviour.ChangeMaterial(effectsScrub.lingeringEffectMaterials[(int)LingeringElements.Frozen]);

            RemoveLingeringEffect();

            // Spawn an ice effect afterwards
            _activeLingeringEffect = LingeringElements.Ice;
        }
    }
}
