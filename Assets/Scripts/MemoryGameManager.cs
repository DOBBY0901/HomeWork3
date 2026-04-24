using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private MemoryCard[] cards; //카드 배열
    [SerializeField] private Sprite backSprite; //뒷면 이미지
    [SerializeField] private Sprite[] frontSprites; //앞면 이미지 배열

    [Header("Panels")]
    [SerializeField] private GameObject startPanel; //시작 패널
    [SerializeField] private GameObject gamePanel; //게임 패널
    [SerializeField] private GameObject endPanel; //종료 패널

    [Header("UI")]
    [SerializeField] private TMP_Text resultText; //결과 텍스트

    [Header("Settings")]
    [SerializeField] private float checkDelay = 0.8f; //뒤집어지는 시간 딜레이
    [SerializeField] private float previewTime = 1.5f; //시작 시, 카드 앞면 보여주는 시간

    private MemoryCard firstCard; //첫번째 뒤집은 카드
    private MemoryCard secondCard; //두번째 뒤집은 카드

    private int flipCount = 0; //뒤집기 횟수
    private int matchedCount = 0; //맞는 카드 카운트

    private bool isChecking = false;
    private bool isGamePlaying = false;

    private void Start()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endPanel.SetActive(false);

    }

    public void StartGame() //게임 시작 (버튼 클릭), 초기화
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        endPanel.SetActive(false);

        flipCount = 0;
        matchedCount = 0;
        firstCard = null;
        secondCard = null;
        isChecking = false;
        isGamePlaying = true;

        
        SetupCards();
        StartCoroutine(PreviewCards());

    }
    private IEnumerator PreviewCards() //시작시 카드 앞면을 보여주는 코루틴
    {
        SetAllCardsClickable(false);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipToFront();
        }

        yield return new WaitForSeconds(previewTime);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipToBack();
        }

        yield return new WaitForSeconds(0.3f);

        isGamePlaying = true;
        SetAllCardsClickable(true);
    }

    private void SetupCards() //카드 배치
    {
        List<int> cardIds = new List<int>();

        // 앞면 이미지 8개를 각각 2장씩 넣어서 총 16장 생성
        for (int i = 0; i < frontSprites.Length; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        Shuffle(cardIds);

        for (int i = 0; i < cards.Length; i++)
        {
            int id = cardIds[i];
            cards[i].Init(id, frontSprites[id], backSprite, this);
        }
    }

    private void Shuffle(List<int> list) //카드 섞기
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SelectCard(MemoryCard selectedCard) //카드 선택 (버튼)
    {
        if (!isGamePlaying)
            return;

        if (isChecking)
            return;

        if (selectedCard.IsFlipped || selectedCard.IsMatched)
            return;

        selectedCard.FlipToFront();

        flipCount++;

        if (firstCard == null)
        {
            firstCard = selectedCard;
            return;
        }

        secondCard = selectedCard;
        StartCoroutine(CheckMatch());
    }

    private IEnumerator CheckMatch() //맞는 카드 확인 코루틴
    {
        isChecking = true;
        SetAllCardsClickable(false);

        yield return new WaitForSeconds(checkDelay);

        if (firstCard.CardId == secondCard.CardId)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();

            matchedCount += 2; //2장이 맞아지기 떄문에 2상승

            if (matchedCount >= cards.Length)
            {
                EndGame();
            }
        }
        else
        {
            firstCard.FlipToBack();
            secondCard.FlipToBack();
        }

        firstCard = null;
        secondCard = null;

        SetAllCardsClickable(true);
        isChecking = false;
    }

    private void SetAllCardsClickable(bool value) //전체 카드 클릭 가능 관리
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetClickable(value);
        }
    }

    private void EndGame() //게임 종료, 결과 텍스트 출력
    {
        isGamePlaying = false;
        gamePanel.SetActive(false);
        endPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "Result : "+ flipCount;
    }

    public void RestartGame() //게임 재시작
    {
        StartGame();
    }
}