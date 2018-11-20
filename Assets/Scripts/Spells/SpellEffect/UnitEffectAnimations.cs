using UnityEngine;
using System.Collections;

public class UnitEffectAnimations : MonoBehaviour
{
    public ParticleSystem burnAnimation;
    public ParticleSystem freezeAnimation;
    public ParticleSystem attackBuffAnimation;
    public ParticleSystem berserkAnimation;

    private void Start()
    {
        
    }

    public void StartAnimation(SpellUtils.EffectTypes type)
    {
        switch (type)
        {
            case (SpellUtils.EffectTypes.BURN):
                burnAnimation.Play();
                break;
            case (SpellUtils.EffectTypes.FROST):
                freezeAnimation.Play();
                break;
            case (SpellUtils.EffectTypes.ATTACK_MODIFIER):
                attackBuffAnimation.Play();
                break;
            case (SpellUtils.EffectTypes.ARMOR_GAIN):
                berserkAnimation.Play();
                break;
        }
    }

    public void StopAnimation(SpellUtils.EffectTypes type)
    {
        switch (type)
        {
            case (SpellUtils.EffectTypes.BURN):
                burnAnimation.Stop();
                break;
            case (SpellUtils.EffectTypes.FROST):
                freezeAnimation.Stop();
                break;
            case (SpellUtils.EffectTypes.ATTACK_MODIFIER):
                attackBuffAnimation.Stop();
                break;
            case (SpellUtils.EffectTypes.ARMOR_GAIN):
                berserkAnimation.Stop();
                break;
        }
    }

}
