using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public HealthBar enemyHB;
    public EnergyBar enemyEB;

    public Vector3 viewPos;
    public Vector3 HB_pos;
    public Vector3 EB_pos;

    // Update is called once per frame
    void Update()
    {
        viewPos = Camera.main.WorldToViewportPoint(this.transform.position);
        if ((viewPos.x < 0f || viewPos.x > 1f) || (viewPos.y < 0f || viewPos.y > 1f) || (viewPos.z < 0f || viewPos.z > 1f)) {
            enemyHB.gameObject.SetActive(false);
            enemyEB.gameObject.SetActive(false);
        } else {
            enemyHB.gameObject.SetActive(true);
            enemyEB.gameObject.SetActive(true);
        }
        HB_pos = Camera.main.WorldToScreenPoint(this.transform.position);
        EB_pos = Camera.main.WorldToScreenPoint(this.transform.position);
        HB_pos.y += 30;
        EB_pos.y += 10;
        enemyHB.transform.position = HB_pos;
        enemyEB.transform.position = EB_pos;
    }
}
