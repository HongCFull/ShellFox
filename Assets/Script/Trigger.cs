using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public PlayerAttributes player;
    public EnemyAttributes enemy;

    // Update is called once per frame
    void Update()
    {
        if (enemy.enemyBA.inBattle) {
            player.playerBA.inBattle = true;
        } else {
            player.playerBA.inBattle = false;
        }
    }
}
