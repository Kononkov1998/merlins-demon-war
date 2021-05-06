using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDropHandler
{
    public Image playerImage;
    [SerializeField]
    private Image mirrorImage;
    [SerializeField]
    private Image healthImage;
    public Image glowImage;

    public int maxHealth = 5;
    public int health = 5;
    public int maxMana = 5;
    public int mana = 1;

    public bool isPlayer;
    public bool isFire; // whether an enemy is a fire monster or not

    [SerializeField]
    private GameObject[] manaBalls;

    private Animator animator;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip cardClip;
    [SerializeField]
    private AudioClip mirrorClip;
    [SerializeField]
    private AudioClip smashClip;
    [SerializeField]
    private AudioClip healClip;    

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        UpdateHealthUI();
        UpdateManaBallsUI();
    }

    public void PlayHitAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable)
        {
            return;
        }

        GameObject obj = eventData.pointerDrag;

        if (obj != null)
        {
            Card card = obj.GetComponent<Card>();
            if (card != null)
            {
                GameController.instance.UseCard(card, this, GameController.instance.playerHand);
            }
        }
    }

    public void UpdateHealthUI()
    {
        if (health >= 0 && health < GameController.instance.healthNumbers.Length)
        {
            healthImage.sprite = GameController.instance.healthNumbers[health];
        }
        else
        {
            Debug.LogError($"Health is not a valid number, {health}");
        }
    }

    public void SetMirror(bool on)
    {
        mirrorImage.gameObject.SetActive(on);
    }

    public bool HasMirror()
    {
        return mirrorImage.gameObject.activeInHierarchy;
    }

    public void UpdateManaBallsUI()
    {
        for (int i = 0; i < manaBalls.Length; i++)
        {
            if (mana > i)
            {
                manaBalls[i].SetActive(true);
            }
            else
            {
                manaBalls[i].SetActive(false);
            }
        }
    }

    public void PlayMirrorSound()
    {
        audioSource.PlayOneShot(mirrorClip);
    }

    public void PlaySmashSound()
    {
        audioSource.PlayOneShot(smashClip);
    }

    public void PlayHealSound()
    {
        audioSource.PlayOneShot(healClip);
    }

    public void PlayCardSound()
    {
        audioSource.PlayOneShot(cardClip);
    }    
}
