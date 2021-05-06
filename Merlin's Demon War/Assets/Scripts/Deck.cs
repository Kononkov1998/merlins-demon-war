using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField]
    private List<CardData> cardDatas = new List<CardData>();

    public void Create()
    {
        List<CardData> cardDatasInOrder = new List<CardData>();

        foreach (var cardData in GameController.instance.cards)
        {
            for (int i = 0; i < cardData.numberInDeck; i++)
            {
                cardDatasInOrder.Add(cardData);
            }
        }

        while (cardDatasInOrder.Count > 0)
        {
            int randomIndex = Random.Range(0, cardDatasInOrder.Count);
            cardDatas.Add(cardDatasInOrder[randomIndex]);
            cardDatasInOrder.RemoveAt(randomIndex);
        }
    }

    private CardData GetCardData()
    {
        CardData result = null;

        if (cardDatas.Count == 0)
        {
            Create();
        }

        result = cardDatas[0];
        cardDatas.RemoveAt(0);

        return result;
    }

    private Card CreateNewCard(Vector3 position, string animName)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPrefab, GameController.instance.canvas.transform);
        newCard.transform.position = position;
        Card card = newCard.GetComponent<Card>();

        if (card)
        {
            card.cardData = GetCardData();
            card.Initialize();

            Animator animator = newCard.GetComponentInChildren<Animator>();
            if (animator)
            {
                animator.CrossFade(animName, 0);
            }
            else
            {
                Debug.LogError("No animator found!");
                return null;
            }

            return card;
        }
        else
        {
            Debug.LogError("No card component found!");
            return null;
        }
    }

    public void DealCard(Hand hand)
    {
        for (int i = 0; i < 3; i++)
        {
            if (hand.cards[i] == null)
            {
                if (hand.isPlayer)
                {
                    GameController.instance.player.PlayCardSound();
                }
                else
                {
                    GameController.instance.enemy.PlayCardSound();
                }

                hand.cards[i] = CreateNewCard(hand.positions[i].position, hand.animNames[i]);
                return;
            }
        }
    }
}
