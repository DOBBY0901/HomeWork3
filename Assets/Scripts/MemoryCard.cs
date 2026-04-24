using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image cardImage; //카드이미지
    [SerializeField] private Button button; //버튼

    [Header("Effect")]
    [SerializeField] private GameObject clickEffectPrefab;

    private int cardId;
    private Sprite frontSprite; //카드 앞면 이미지
    private Sprite backSprite; //카드 뒷면 이미지
   

    private bool isFlipped = false;
    private bool isMatched = false;

    private MemoryGameManager gameManager;

    public int CardId => cardId;
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;

    public void Init(int id,Sprite front,Sprite back,MemoryGameManager manager) //초기화
    {
        cardId = id;
        frontSprite = front;
        backSprite = back;
        gameManager = manager;

        isFlipped = false;
        isMatched = false;

        cardImage.sprite = backSprite;
        button.interactable = true;
    }

    private void Awake()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnClickCard);
    }



    private void OnClickCard() //카드 클릭(버튼)
    {
        if (isFlipped || isMatched)
            return;

        PlayClickEffect();

        gameManager.SelectCard(this);
    }

    public void FlipToFront() //앞면으로 뒤집기
    {
        isFlipped = true;
        cardImage.sprite = frontSprite;
    }


    public void FlipToBack() //뒷면으로 돌아가기 (맞추지 못함)
    {
        isFlipped = false;
        cardImage.sprite = backSprite;
    }

    public void SetMatched() //그림이 맞을 때
    {
        isMatched = true;
        button.interactable = false;
    }

    public void SetClickable(bool value) //클릭이 가능한 경우 (맞지 않을때)
    {
        if (isMatched) return;

        button.interactable = value;
    }

    private void PlayClickEffect()
    {
        if (clickEffectPrefab == null)
            return;

        GameObject effect = Instantiate(clickEffectPrefab, transform);
        effect.transform.localPosition = Vector3.zero;

        Destroy(effect, 1f);
    }
}