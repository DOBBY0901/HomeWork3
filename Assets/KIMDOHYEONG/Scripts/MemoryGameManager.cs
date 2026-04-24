using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private MemoryCard[] cards;       // 카드 배열
    [SerializeField] private Sprite backSprite;        // 카드 뒷면 이미지
    [SerializeField] private Sprite[] frontSprites;    // 카드 앞면 이미지 배열

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;    // 시작 패널
    [SerializeField] private GameObject gamePanel;     // 게임 패널
    [SerializeField] private GameObject endPanel;      // 결과 패널

    [Header("UI")]
    [SerializeField] private TMP_Text resultText;      // 결과 텍스트

    [Header("Settings")]
    [SerializeField] private float checkDelay = 0.8f;  // 두 카드 비교 전 대기 시간
    [SerializeField] private float previewTime = 1.5f; // 시작 시 카드 앞면을 보여주는 시간

    private MemoryCard firstCard;      // 첫 번째로 선택한 카드
    private MemoryCard secondCard;     // 두 번째로 선택한 카드

    private int flipCount = 0;         // 카드를 뒤집은 횟수
    private int matchedCount = 0;      // 매칭된 카드 개수

    private bool isChecking = false;   // 현재 카드 비교 중인지 확인
    private bool isGamePlaying = false; // 게임 진행 중인지 확인

    private void Start()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endPanel.SetActive(false);
    }

    public void StartGame() // 게임 시작 및 초기화
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        endPanel.SetActive(false);

        flipCount = 0;
        matchedCount = 0;
        firstCard = null;
        secondCard = null;
        isChecking = false;

        // 미리보기 중에는 클릭하지 못하게 false로 시작
        isGamePlaying = false;

        SetupCards();
        StartCoroutine(PreviewCards());
    }

    private IEnumerator PreviewCards() // 게임 시작 시 모든 카드 앞면을 잠깐 보여주는 코루틴
    {
        SetAllCardsClickable(false);

        // 모든 카드 앞면 공개
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipToFront();
        }

        yield return new WaitForSeconds(previewTime);

        // 다시 뒷면으로 전환
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].FlipToBack();
        }

        yield return new WaitForSeconds(0.3f);

        // 미리보기가 끝난 뒤 게임 시작
        isGamePlaying = true;
        SetAllCardsClickable(true);
    }

    private void SetupCards() // 카드에 랜덤으로 이미지 배치
    {
        List<int> cardIds = new List<int>();

        // 앞면 이미지 8개를 각각 2장씩 넣어 총 16장 구성
        for (int i = 0; i < frontSprites.Length; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        Shuffle(cardIds);

        // 섞인 카드 번호를 실제 카드에 적용
        for (int i = 0; i < cards.Length; i++)
        {
            int id = cardIds[i];
            cards[i].Init(id, frontSprites[id], backSprite, this);
        }
    }

    private void Shuffle(List<int> list) // 카드 순서를 랜덤으로 섞는 함수
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SelectCard(MemoryCard selectedCard) // 카드 선택 처리
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

    private IEnumerator CheckMatch() // 선택한 두 카드가 같은지 확인하는 코루틴
    {
        isChecking = true;
        SetAllCardsClickable(false);

        yield return new WaitForSeconds(checkDelay);

        if (firstCard.CardId == secondCard.CardId)
        {
            // 카드 번호가 같으면 매칭 성공
            firstCard.SetMatched();
            secondCard.SetMatched();

            matchedCount += 2;

            if (matchedCount >= cards.Length)
            {
                EndGame();
            }
        }
        else
        {
            // 카드 번호가 다르면 다시 뒷면으로 전환
            firstCard.FlipToBack();
            secondCard.FlipToBack();
        }

        firstCard = null;
        secondCard = null;

        SetAllCardsClickable(true);
        isChecking = false;
    }

    private void SetAllCardsClickable(bool value) // 전체 카드 클릭 가능 여부 설정
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetClickable(value);
        }
    }

    private void EndGame() // 게임 종료 및 결과 출력
    {
        isGamePlaying = false;
        gamePanel.SetActive(false);
        endPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "Result : " + flipCount;
    }

    public void RestartGame() // 게임 재시작
    {
        StartGame();
    }
}