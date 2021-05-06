using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public Hand playerHand = new Hand();
    public Hand enemyHand = new Hand();

    public Player player;
    public Player enemy;

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers;
    public Sprite[] damageNumbers;

    public GameObject cardPrefab = null;
    public Canvas canvas = null;

    public bool isPlayable = false;

    [SerializeField]
    private GameObject effectFromLeftPrefab;
    [SerializeField]
    private GameObject effectFromRightPrefab;
    [SerializeField]
    private Sprite fireBallImage;
    [SerializeField]
    private Sprite iceBallImage;
    [SerializeField]
    private Sprite multiFireBallImage;
    [SerializeField]
    private Sprite multiIceBallImage;
    [SerializeField]
    private Sprite fireAndIceBallImage;
    [SerializeField]
    private Sprite DestroyBallImage;

    public bool playersTurn = true;
    public Text turnText;
    public Image enemySkipTurn;

    public Sprite fireDemon;
    public Sprite iceDemon;

    public Text scoreText;
    public int score = 0;
    public int kills = 0;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip playerDieClip;
    [SerializeField]
    private AudioClip enemyDieClip;
    [SerializeField]
    private AudioClip iceClip;
    [SerializeField]
    private AudioClip fireClip;
    [SerializeField]
    private AudioClip destructClip;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();

        SetUpEnemy();

        playerDeck.Create();
        enemyDeck.Create();

        StartCoroutine(DealHands());
    }

    public void PlayDestructSound()
    {
        audioSource.PlayOneShot(destructClip);
    }

    public void PlayIceSound()
    {
        audioSource.PlayOneShot(iceClip);
    }

    public void PlayFireSound()
    {
        audioSource.PlayOneShot(fireClip);
    }

    public void PlayPlayerDieSound()
    {
        audioSource.panStereo = -0.5f;
        audioSource.PlayOneShot(playerDieClip);
    }

    public void PlayEnemyDieSound()
    {
        audioSource.panStereo = 0.5f;
        audioSource.PlayOneShot(enemyDieClip);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        if (playersTurn && isPlayable)
        {
            NextPlayersTurn();
        }
    }

    private IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 3; i++)
        {
            playerDeck.DealCard(playerHand);
            enemyDeck.DealCard(enemyHand);
            yield return new WaitForSeconds(1);
        }

        isPlayable = true;
    }

    public bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (!IsCardValid(card, usingOnPlayer, fromHand))
        {
            return false;
        }

        isPlayable = false;
        CastCard(card, usingOnPlayer, fromHand);
        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);
        fromHand.RemoveCard(card);

        return false;
    }

    public bool IsCardValid(Card cardBeingPlayed, Player usingOnPlayer, Hand fromHand)
    {
        if (fromHand.isPlayer)
        {
            if (cardBeingPlayed.cardData.cost <= player.mana)
            {
                if (usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenceCard)
                {
                    return true;
                }
                else if (!usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenceCard)
                {
                    return true;
                }
            }
        }
        else // from enemy
        {
            if (cardBeingPlayed.cardData.cost <= enemy.mana)
            {
                if (!usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenceCard)
                {
                    return true;
                }
                else if (usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenceCard)
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (card.cardData.isMirrorCard)
        {
            usingOnPlayer.SetMirror(true);
            usingOnPlayer.PlayMirrorSound();
            NextPlayersTurn();
            isPlayable = true;
        }
        else if (card.cardData.isDefenceCard) // health card
        {
            usingOnPlayer.health += card.cardData.damage;
            usingOnPlayer.PlayHealSound();

            if (usingOnPlayer.health > usingOnPlayer.maxHealth)
            {
                usingOnPlayer.health = usingOnPlayer.maxHealth;
            }

            UpdateHealths();
            StartCoroutine(CastHealEffect(usingOnPlayer));
        }
        else // attack card
        {
            CastAttackEffect(card, usingOnPlayer);
        }

        if (fromHand.isPlayer)
        {
            player.mana -= card.cardData.cost;
            player.UpdateManaBallsUI();
        }
        else
        {
            enemy.mana -= card.cardData.cost;
            enemy.UpdateManaBallsUI();
        }

        if (fromHand.isPlayer)
        {
            score += card.cardData.damage;
            UpdateScoreText();
        }
    }

    private IEnumerator CastHealEffect(Player usingOnPlayer)
    {
        yield return new WaitForSeconds(.5f);
        NextPlayersTurn();
        isPlayable = true;
    }

    public void CastAttackEffect(Card card, Player usingOnPlayer)
    {
        GameObject effectGO = usingOnPlayer.isPlayer ?
            Instantiate(effectFromRightPrefab, canvas.transform) :
            Instantiate(effectFromLeftPrefab, canvas.transform);

        audioSource.panStereo = usingOnPlayer.isPlayer ? 0.5f : -0.5f;

        Effect effect = effectGO.GetComponent<Effect>();
        if (effect != null)
        {
            effect.targetPlayer = usingOnPlayer;
            effect.sourceCard = card;

            switch (card.cardData.damageType)
            {
                case CardData.DamageType.Fire:
                    effect.effectImage.sprite = card.cardData.isMulti ? multiFireBallImage : fireBallImage;
                    PlayFireSound();
                    break;
                case CardData.DamageType.Ice:
                    effect.effectImage.sprite = card.cardData.isMulti ? multiIceBallImage : iceBallImage;
                    PlayIceSound();
                    break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceBallImage;
                    PlayFireSound();
                    PlayIceSound();
                    break;
                case CardData.DamageType.Destruct:
                    effect.effectImage.sprite = DestroyBallImage;
                    PlayDestructSound();
                    break;
            }
        }
    }

    public void UpdateHealths()
    {
        player.UpdateHealthUI();
        enemy.UpdateHealthUI();

        if (player.health <= 0)
        {
            isPlayable = false;
            PlayPlayerDieSound();
            StartCoroutine(GameOver());
        }
        else if (enemy.health <= 0)
        {
            isPlayable = false;
            PlayEnemyDieSound();
            kills++;
            score += 100;
            UpdateScoreText();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemyHand.ClearHand();
        yield return new WaitForSeconds(1);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealthUI();
        enemy.UpdateManaBallsUI();

        if (Random.Range(0, 2) == 0)
        {
            enemy.isFire = true;
            enemy.playerImage.sprite = fireDemon;
        }
        else
        {
            enemy.isFire = false;
            enemy.playerImage.sprite = iceDemon;
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void NextPlayersTurn()
    {
        bool enemyIsDead = enemy.health == 0;

        if (enemyIsDead)
        {
            playersTurn = true;
        }
        else
        {
            playersTurn = !playersTurn;
        }

        if (playersTurn)
        {
            if (player.mana < player.maxMana)
            {
                player.mana++;
            }
        }
        else
        {
            if (!enemyIsDead)
            {
                if (enemy.mana < enemy.maxMana)
                {
                    enemy.mana++;
                }
            }
        }

        UpdateTurnText();
        if (!playersTurn)
        {
            EnemysTurn();
        }

        player.UpdateManaBallsUI();
        enemy.UpdateManaBallsUI();
    }

    public void UpdateTurnText()
    {
        if (playersTurn)
        {
            turnText.text = "Merlin's turn";
        }
        else
        {
            turnText.text = "Enemy's turn";
        }
    }

    private void EnemysTurn()
    {
        Card card = EnemyChooseCard();
        StartCoroutine(EnemyCastCard(card));
    }

    private Card EnemyChooseCard()
    {
        List<Card> availableCards = new List<Card>();

        foreach (Card card in enemyHand.cards)
        {
            if (IsCardValid(card, enemy, enemyHand))
            {
                if (card.cardData.isMirrorCard || enemy.health < enemy.maxHealth)
                {
                    availableCards.Add(card);
                }
            }
            else if (IsCardValid(card, player, enemyHand))
            {
                availableCards.Add(card);
            }
        }

        if (availableCards.Count == 0)
        {
            return null;
        }

        return availableCards[Random.Range(0, availableCards.Count)];
    }

    private IEnumerator EnemyCastCard(Card card)
    {
        yield return new WaitForSeconds(.5f);

        if (card == null)
        {
            enemySkipTurn.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            enemySkipTurn.gameObject.SetActive(false);
            NextPlayersTurn();
        }
        else
        {
            TurnCard(card);

            yield return new WaitForSeconds(2);

            if (card.cardData.isDefenceCard)
            {
                UseCard(card, enemy, enemyHand);
            }
            else
            {
                UseCard(card, player, enemyHand);
            }

            yield return new WaitForSeconds(1);

            enemyDeck.DealCard(enemyHand);

            yield return new WaitForSeconds(1);
        }
    }

    public void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();

        if (animator)
        {
            animator.SetTrigger("Flip");
        }
        else
        {
            Debug.LogError("No Animator found");
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Demons killed: {kills}. Score: {score}.";
    }
}
