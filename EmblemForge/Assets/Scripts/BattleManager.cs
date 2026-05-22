// GameManager class
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("单位列表（由 SpawnAllUnits 自动填充）")]
    public List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    public List<EnemyUnit> enemyUnits = new List<EnemyUnit>();

    [Header("出生配置")]
    public GameObject[] playerPrefabs;
    public UnitSpawnEntry[] playerSpawns;     // 与 playerPrefabs 一一对应
    public GameObject[] enemyPrefabs;
    public UnitSpawnEntry[] enemySpawns;      // 与 enemyPrefabs 一一对应

    private bool playerTurn = true;
    private bool isPlayerActing = false;
    private bool enemyTurn = false;
    private bool isEnemyActing = false;
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera

    public GameObject buttonOptions; // Reference to the ButtonOptions GameObject
    public GameObject attackButton;
    private bool isAttackButtonActive = false;
    private bool isButtonOptionsActive = false;

    public Animator turnChange;
    public Text whosTurnText;
    public Image turnVig;
    public Image turnbg;

    public Color playerColor1;
    public Color playerColor2;
    public Color enemyColor1;
    public Color enemyColor2;

    private UnitController selectedUnit;
    private GridManager gridManager;

    [Header("敌人回合")]
    public float turnChangeDuration = 0.5f; // 回合切换动画时长
    public float enemyTurnDelay = 0.5f; // 每个敌人行动之间的延迟

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        SpawnAllUnits();
        StartPlayerTurn();
    }

    // ─── 单位生成 ─────────────────────────────────────────────

    /// <summary>
    /// 根据 playerSpawns / enemySpawns 配置生成所有单位。
    /// 可在编辑器通过右键菜单或代码调用，用于动态重置/重新开局。
    /// </summary>
    public void SpawnAllUnits()
    {
        ClearAllUnits();

        SpawnGroup(playerPrefabs, playerSpawns, unit => playerUnits.Add((PlayerUnit)unit));
        SpawnGroup(enemyPrefabs, enemySpawns, unit => enemyUnits.Add((EnemyUnit)unit));
    }

    /// <summary>
    /// 通过平行数组 (prefabs[i], spawns[i].gridPosition) 生成一组单位
    /// </summary>
    private void SpawnGroup(GameObject[] prefabs, UnitSpawnEntry[] spawns, Action<UnitController> onSpawned)
    {
        if (prefabs == null || spawns == null) return;

        int count = Mathf.Min(prefabs.Length, spawns.Length);
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabs[i];
            if (prefab == null) continue;

            Vector3 worldPos = new Vector3(spawns[i].gridPosition.x, spawns[i].gridPosition.y, 0);
            GameObject go = Instantiate(prefab, worldPos, Quaternion.identity);
            go.name = prefab.name;

            UnitController unit = go.GetComponent<UnitController>();
            if (unit != null)
            {
                unit.MoveToClosestTile();
                onSpawned(unit);
            }
        }
    }

    /// <summary>清除所有已生成的单位</summary>
    private void ClearAllUnits()
    {
        foreach (var u in playerUnits)
            if (u != null) Destroy(u.gameObject);
        playerUnits.Clear();

        foreach (var u in enemyUnits)
            if (u != null) Destroy(u.gameObject);
        enemyUnits.Clear();
    }

    public void Update()
    {
        if (playerTurn)
        {
            CheckPlayerTurnEnd();
        }
        if(enemyTurn && !isEnemyActing)
        {
            CheckEnemyTurnEnd();
        }
    }

    private void StartPlayerTurn()
    {
        playerTurn = true;
        isPlayerActing = true;
        RunTurnChange();

        // Reset all player units for the new turn
        foreach (PlayerUnit unit in playerUnits)
        {
            unit.hasGoneThisTurn = false;
            unit.lastUnitThing = 0; // Set lastUnitThing to 0
        }

        // Disable buttonOptions at the start of the turn
        DisableButtonOptions();
    }

    private void EndPlayerTurn()
    {
        playerTurn = false;
        isPlayerActing = false;

        // End the player's turn and trigger the enemy's turn or other game logic
        // For example:
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        enemyTurn = true;
        isEnemyActing = true;
        RunTurnChange();
        
        foreach (EnemyUnit unit in enemyUnits)
        {
            unit.hasGoneThisTurn = false;
            unit.lastUnitThing = 0;
        }
        DisableButtonOptions();

        // 协程按顺序逐个执行敌人行动
        StartCoroutine(EnemyTurnRoutine());
    }

    /// <summary>
    /// 敌人回合协程：等待切换动画 → 敌人按顺序逐个行动 → 结束回合
    /// </summary>
    private IEnumerator EnemyTurnRoutine()
    {
        // 等待回合切换动画播放完毕
        yield return new WaitForSeconds(turnChangeDuration);

        foreach (EnemyUnit enemy in enemyUnits)
        {
            if (enemy == null) continue;

            // 等待当前敌人行动完成（通过事件回调）
            bool completed = false;
            Action handler = () => completed = true;
            enemy.OnEnemyActionCompleted += handler;
            enemy.ExecuteEnemyTurn();

            yield return new WaitUntil(() => completed);

            enemy.OnEnemyActionCompleted -= handler;

            // 每个敌人之间加点间隔
            yield return new WaitForSeconds(enemyTurnDelay);
        }

        // 所有敌人行动完毕
        isEnemyActing = false;
    }
    private void EndEnemyTurn()
    {
        enemyTurn = false;
        isEnemyActing = false;

        // End the enemy's turn and trigger the player's turn or other game logic
        // For example:
        StartPlayerTurn();
    }

    private void RunTurnChange()
    {
        if (playerTurn)
        {
            turnVig.color = playerColor1;
            turnbg.color = playerColor2;
            whosTurnText.text = "PLAYER TURN";
        }
        else
        {
            turnVig.color = enemyColor1;
            turnbg.color = enemyColor2;
            whosTurnText.text = "ENEMY TURN";
        }

        turnChange.SetTrigger("turnChange");
    }


    public void CheckPlayerTurnEnd()
    {
        foreach (UnitController unit in playerUnits)
        {
            if (!unit.hasGoneThisTurn)
                return; // If any unit hasn't gone, return to continue the player's turn
        }
        // All player units have completed their actions
        EndPlayerTurn();
    }
    public void CheckEnemyTurnEnd()
    {
        foreach (EnemyUnit unit in enemyUnits)
        {
            if (!unit.hasGoneThisTurn)
                return;
        }
        EndEnemyTurn();
    }

    // Method to handle unit selection and highlighting
    public void SelectUnit(UnitController selectedUnit)
    {
        if (playerTurn && isPlayerActing && !selectedUnit.hasGoneThisTurn && !isButtonOptionsActive)
        {
            this.selectedUnit = selectedUnit; // Set the selected unit
            selectedUnit.HighlightAvailableMoves();
            FollowSelectedUnit(selectedUnit.transform);
        }
    }

    private bool CheckAdjacentEnemy(UnitController selectedUnit)
    {
        foreach (UnitController enemyUnit in enemyUnits)
        {
            float distance = Vector3.Distance(selectedUnit.transform.position, enemyUnit.transform.position);
            if (distance <= 1.0f) // You can adjust the distance as needed
            {
                return true;
            }
        }
        return false;
    }




    // Use Cinemachine to follow the selected unit
    private void FollowSelectedUnit(Transform target)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
        }
    }

    // Enable the buttonOptions
    public void EnableButtonOptions()
    {

        // Check if the selected unit is next to an enemy unit
        if (CheckAdjacentEnemy(selectedUnit))
        {
            isAttackButtonActive = true;
            attackButton.SetActive(true);
        }
        else
        {
            isAttackButtonActive = false;
            attackButton.SetActive(false);
        }

        isButtonOptionsActive = true;
        if (buttonOptions != null)
        {
            buttonOptions.SetActive(true);
        }
    }

    // Disable the buttonOptions
    public void DisableButtonOptions()
    {
        isButtonOptionsActive = false;
        if (buttonOptions != null)
        {
            buttonOptions.SetActive(false);
        }
    }

    public void IdleButton()
    {
        selectedUnit.lastUnitThing = 1;
        selectedUnit.hasGoneThisTurn = true;
        DisableButtonOptions();
    }

    public void AttackButton()
    {
        if (isAttackButtonActive)
        {
            UnitController defender = FindAdjacentEnemy(selectedUnit);
            if (defender != null)
            {
                Attack(selectedUnit, defender);
            }
        }
        DisableButtonOptions();
        selectedUnit.lastUnitThing = 2;
        selectedUnit.hasGoneThisTurn = true;
    }

    public void Attack(UnitController attacker, UnitController defender)
    {
        defender.TakeDamage(attacker.unit.strength);
    }

    private UnitController FindAdjacentEnemy(UnitController selectedUnit)
    {
        foreach (UnitController enemyUnit in enemyUnits)
        {
            float distance = Vector3.Distance(selectedUnit.transform.position, enemyUnit.transform.position);
            if (distance <= 1.0f) // You can adjust the distance as needed
            {
                return enemyUnit;
            }
        }
        return null;
    }



}
