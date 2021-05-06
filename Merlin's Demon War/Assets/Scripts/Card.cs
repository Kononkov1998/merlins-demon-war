using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData cardData;

    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text description;

    [SerializeField]
    private Image damageImage;
    [SerializeField]
    private Image costImage;
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Image frameImage;
    public Image burnImage;

    public void Initialize()
    {
        if (cardData == null)
        {
            Debug.LogError("Card has no CardData!");
            return;
        }

        titleText.text = cardData.title;
        description.text = cardData.description;
        cardImage.sprite = cardData.cardImage;
        frameImage.sprite = cardData.frameImage;
        costImage.sprite = GameController.instance.healthNumbers[cardData.cost];
        damageImage.sprite = GameController.instance.damageNumbers[cardData.damage];
    }
}
