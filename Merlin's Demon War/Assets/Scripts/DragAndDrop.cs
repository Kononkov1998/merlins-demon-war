using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private Card card;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        card = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable)
        {
            return;
        }

        transform.position += (Vector3)eventData.delta;
        bool isHoveringPlayer = false;

        foreach (var obj in eventData.hovered)
        {
            Player player = obj.GetComponent<Player>();

            if (player != null)
            {
                if (GameController.instance.IsCardValid(card, player, GameController.instance.playerHand))
                {
                    player.glowImage.gameObject.SetActive(true);
                    isHoveringPlayer = true;
                }
            }

            if (!isHoveringPlayer)
            {
                GameController.instance.player.glowImage.gameObject.SetActive(false);
                GameController.instance.enemy.glowImage.gameObject.SetActive(false);
            }

            BurnZone burnZone = obj.GetComponent<BurnZone>();

            if (burnZone != null)
            {
                card.burnImage.gameObject.SetActive(true);
            }
            else
            {
                card.burnImage.gameObject.SetActive(false);
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
    }
}
