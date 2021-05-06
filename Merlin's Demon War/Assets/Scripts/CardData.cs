using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName ="CardGame/Card")]
public class CardData : ScriptableObject
{
    public enum DamageType
    {
        Fire,
        Ice,
        Both,
        Destruct
    }

    public string title;
    public string description;
    public int cost;
    public int damage;
    public DamageType damageType;
    public Sprite cardImage;
    public Sprite frameImage;
    public int numberInDeck;
    public bool isDefenceCard;
    public bool isMirrorCard;
    public bool isMulti;
}
