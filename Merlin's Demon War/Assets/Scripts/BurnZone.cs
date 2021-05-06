using UnityEngine;
using UnityEngine.EventSystems;

public class BurnZone : MonoBehaviour, IDropHandler
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable)
        {
            return;
        }

        Card card = eventData.pointerDrag.GetComponent<Card>();
        if (card != null)
        {
            PlayBurnSound();
            GameController.instance.playerHand.RemoveCard(card);
            GameController.instance.NextPlayersTurn();
        }
    }

    private void PlayBurnSound()
    {
        audioSource.Play();
    }
}
