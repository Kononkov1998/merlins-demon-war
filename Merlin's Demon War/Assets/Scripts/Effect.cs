using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player targetPlayer;
    public Card sourceCard;
    public Image effectImage;

    private void EndTrigger()
    {
        if (targetPlayer.HasMirror())
        {
            targetPlayer.SetMirror(false);
            targetPlayer.PlaySmashSound();

            if (targetPlayer.isPlayer)
            {
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.enemy);
            }
            else
            {
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.player);
            }
        }
        else
        {
            int damage = sourceCard.cardData.damage;

            if (!targetPlayer.isPlayer) // enemy
            {
                if (sourceCard.cardData.damageType == CardData.DamageType.Fire && targetPlayer.isFire)
                {
                    damage /= 2;
                }
                else if (sourceCard.cardData.damageType == CardData.DamageType.Ice && !targetPlayer.isFire)
                {
                    damage /= 2;
                }
            }
            targetPlayer.health -= damage;

            if (targetPlayer.health < 0)
            {
                targetPlayer.health = 0;                
            }

            targetPlayer.PlayHitAnim();

            GameController.instance.UpdateHealths();
            GameController.instance.NextPlayersTurn();

            if (targetPlayer.health != 0)
            {
                GameController.instance.isPlayable = true;
            }
        }

        Destroy(gameObject);
    }
}
