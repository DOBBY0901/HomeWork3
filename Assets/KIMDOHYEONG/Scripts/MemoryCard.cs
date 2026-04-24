using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image cardImage;      // 카드 이미지
    [SerializeField] private Button button;        // 카드 버튼

    [Header("Effect")]
    [SerializeField] private GameObject clickEffectPrefab; // 클릭 시 나올 UI 이펙트 프리팹

    private int cardId;              // 카드 구분 번호
    private Sprite frontSprite;      // 카드 앞면 이미지
    private Sprite backSprite;       // 카드 뒷면 이미지

    private bool isFlipped = false;  // 현재 앞면인지 확인
    private bool isMatched = false;  // 매칭 완료 여부

    private MemoryGameManager gameManager;

    public int CardId => cardId;
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;

    public void Init(int id, Sprite front, Sprite back, MemoryGameManager manager) // 카드 초기화
    {
        cardId = id;
        frontSprite = front;
        backSprite = back;
        gameManager = manager;

        isFlipped = false;
        isMatched = false;

        cardImage.sprite = backSprite;
        button.interactable = true;

        transform.localScale = Vector3.one;
    }

    private void Awake()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnClickCard);
    }

    private void OnClickCard() // 카드 클릭 처리
    {
        if (isFlipped || isMatched)
            return;

        PlayClickEffect();

        gameManager.SelectCard(this);
    }

    public void FlipToFront() // 앞면으로 뒤집기
    {
        StartCoroutine(FlipAnimation(frontSprite, true));
    }

    public void FlipToBack() // 뒷면으로 뒤집기
    {
        StartCoroutine(FlipAnimation(backSprite, false));
    }

    private IEnumerator FlipAnimation(Sprite targetSprite, bool toFront) // 카드 뒤집기 애니메이션
    {
        float duration = 0.15f;
        float time = 0f;

        Vector3 originalScale = transform.localScale;

        // 1단계: 카드의 가로 크기를 1에서 0으로 줄임
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localScale = new Vector3(Mathf.Lerp(1f, 0f, t), 1f, 1f);
            yield return null;
        }

        // 카드가 접힌 순간 이미지 교체
        cardImage.sprite = targetSprite;
        isFlipped = toFront;

        time = 0f;

        // 2단계: 카드의 가로 크기를 0에서 1로 다시 늘림
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localScale = new Vector3(Mathf.Lerp(0f, 1f, t), 1f, 1f);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    public void SetMatched() // 매칭 성공 처리
    {
        isMatched = true;
        button.interactable = false;
    }

    public void SetClickable(bool value) // 클릭 가능 여부 설정
    {
        if (isMatched)
            return;

        button.interactable = value;
    }

    private void PlayClickEffect() // 클릭 이펙트 생성
    {
        if (clickEffectPrefab == null)
            return;

        GameObject effect = Instantiate(clickEffectPrefab, transform);
        effect.transform.localPosition = Vector3.zero;

        Destroy(effect, 1f);
    }
}