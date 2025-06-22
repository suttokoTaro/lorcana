using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] Transform enemyFieldTransform;
	[SerializeField] Transform enemyHandTransform;
	[SerializeField] Transform playerFieldTransform;
    [SerializeField] Transform playerHandTransform;
    [SerializeField] CardController cardPrefab;
    [SerializeField] Text playerLeaderHPText, enemyLeaderHPText;

    bool isPlayerTurn = true; 
    List<int> deck = new List<int>() { 1, 2, 3, 1, 1, 2, 2, 3, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3 };
    
    public static GameManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
    	//CreateCard(enemyHandTransform);
    	//CreateCard(playerHandTransform);
        StartGame();
    }

    void StartGame()
    {
        enemyLeaderHP = 5000;
        playerLeaderHP = 5000;
        ShowLeaderHP();

        // 初期手札を配る
        for (int i=0; i<5; i++)
        {
            DrawCard(playerHandTransform);
        }
        // ターンの決定
        TurnCalc();
    }

    void CreateCard(int cardID, Transform placeArea)
    {
        //カードの生成とデータの受け渡し
        CardController card = Instantiate(cardPrefab, placeArea, false);
        card.Init(cardID);
    }

    /**
        カードを引く
    */
    void DrawCard(Transform handArea)
    {
        // デッキがないなら引かない
        if (deck.Count == 0)
        {
            return;
        }
 
        // デッキの一番上のカードを抜き取り、手札に加える
        int cardID = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardID, handArea);
    }

    /**
        ターンを管理する
    */
    void TurnCalc()
    {
        if (isPlayerTurn){ PlayerTurn(); }
        else { EnemyTurn(); }
    }

    /**
        ターンを変更する
    */
        public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn; 
        TurnCalc(); 
    }

    /**
        ターンの開始
    */
    void PlayerTurn()
    {
        Debug.Log("Playerのターン");

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(playerFieldCardList, true);

        DrawCard(playerHandTransform); 
    }
 
    /**
        敵のターン(仮の動き)
    */
    void EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        CreateCard(1, enemyFieldTransform); 
        ChangeTurn();
    }

    /**
        カードバトルメソッド
    */
    public void CardBattle(CardController attackCard, CardController defenceCard)
    {
        if (!attackCard.model.canAttackFlag) { return; }

        // 攻撃側のパワーが高かった場合、攻撃されたカードを破壊する
        if (attackCard.model.hp > defenceCard.model.hp)
        {
            defenceCard.DestroyCard(defenceCard);
        }
 
        // 攻撃された側のパワーが高かった場合、攻撃側のカードを破壊する
        if (attackCard.model.hp < defenceCard.model.hp)
        {
            attackCard.DestroyCard(attackCard);
        }
 
        // パワーが同じだった場合、両方のカードを破壊する
        if (attackCard.model.hp == defenceCard.model.hp)
        {
            attackCard.DestroyCard(attackCard);
            defenceCard.DestroyCard(defenceCard);
        }
        // バトル終了後、アタックしたカードのアタックフラグを不可にする
        attackCard.model.canAttackFlag = false;
        attackCard.view.SetCanAttackPanel(false);
    }

    /**
        指定したカードたちのアタックフラグを変更する
    */
    void SetAttackableFieldCard(CardController[] cardList, bool canAttackFlag)
    {
        foreach (CardController card in cardList)
        {
            card.model.canAttackFlag = canAttackFlag;
            card.view.SetCanAttackPanel(canAttackFlag);
        }
    }

    public int playerLeaderHP;
    public int enemyLeaderHP;
 
    /**
        敵のリーダーにアタック
    */
    public void AttackToLeader(CardController attackCard, bool isPlayerCard)
    {
        if (attackCard.model.canAttackFlag == false)
        {
            return;
        }
 
        enemyLeaderHP -= attackCard.model.hp;
 
        attackCard.model.canAttackFlag = false;
        attackCard.view.SetCanAttackPanel(false);
        Debug.Log("敵のHPは、"+enemyLeaderHP);
        ShowLeaderHP();
    }

    /**
        リーダーHPの表示
    */
    public void ShowLeaderHP()
    {
        if (playerLeaderHP <= 0)
        {
            playerLeaderHP = 0;
        }
        if (enemyLeaderHP <= 0)
        {
            enemyLeaderHP = 0;
        }

        playerLeaderHPText.text = playerLeaderHP.ToString();
        enemyLeaderHPText.text = enemyLeaderHP.ToString();
    }
}
