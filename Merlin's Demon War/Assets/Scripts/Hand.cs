using System;
using UnityEngine;

[System.Serializable]
public class Hand
{
    [SerializeField]
    public Card[] cards;
    [SerializeField]
    public Transform[] positions;
    [SerializeField]
    public string[] animNames;
    public bool isPlayer;

    public void RemoveCard(Card card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == card)
            {
                GameObject.Destroy(cards[i].gameObject);
                cards[i] = null;

                if (isPlayer)
                {
                    GameController.instance.playerDeck.DealCard(this);
                }
                else
                {
                    GameController.instance.enemyDeck.DealCard(this);
                }

                return;
            }
        }
    }

    public void ClearHand()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            GameObject.Destroy(cards[i].gameObject);
            cards[i] = null;
        }
    }
}
