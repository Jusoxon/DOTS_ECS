using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMng : MonoBehaviour
{
    #region SINGLETON
    static GameMng _instance = null;
    public static GameMng Ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameMng)) as GameMng;
                if (_instance == null)
                {
                    _instance = new GameObject("GameMng", typeof(GameMng)).GetComponent<GameMng>();
                }
            }
            return _instance;
        }
    }
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }
    #endregion

    [Header("Game Object References")]
    public Transform player;

    [Header("Collision Info")]
    public float playerCollisionRadius = .5f;
    public static float PlayerCollisionRadius
    {
        get { return Ins.playerCollisionRadius; }
    }

    public float enemyCollisionRadius = .5f;
    public static float EnemyCollisionRadius
    {
        get { return Ins.enemyCollisionRadius; }
    }

    public static Vector3 PlayerPosition
    {
        get { return Ins.player.position; }
    }

    public static Vector3 GetPositionAroundPlayer(float radius)
    {
        Vector3 playerPos = Ins.player.position;

        float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        return new Vector3(c * radius, 1.1f, s * radius) + playerPos;
    }

    public static void PlayerDied()
    {
        if (Ins.player == null)
            return;

        PlayerController playerMove = Ins.player.GetComponent<PlayerController>();
        playerMove.Died();

        Ins.player = null;
    }

    public static bool IsPlayerDead()
    {
        return Ins.player == null;
    }
}
