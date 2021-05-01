using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAOISpawner : MonoBehaviour
{
    public GameObject[] enemy ;
    public float maxRadius;
    public GameObject player;
    public int maxEnemySpawned;
    public float spawnPeriod;
    
   // [SerializeField] Collider[] colliders = new Collider[20];    //at most store 20 enemies
    [SerializeField] GameObject[] scannedObj;
    [SerializeField] List<GameObject> enemiesList = new List<GameObject>();
    [SerializeField] LayerMask layers;  //obj with this layer can be scanned
    
    private int currentEnemySpawned =0; 
    private float accumulator=0f;

    private SceneHandler sceneHandler;
    private static GameObject obj;

    void Start() {
        DontDestroyThisObj();
        sceneHandler = GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>();
        
    }


    void DontDestroyThisObj(){
        if(obj!=null){  //if there is already one gameobject
            Destroy(gameObject);
            return ;
        }
        obj = gameObject;
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if(sceneHandler.IsInBattleScene())  return;

        accumulator += Time.deltaTime;
        if(accumulator> spawnPeriod){
            accumulator=0f;
            ScanAreaOfInterest();
            SpawnGroupOfEnemies(maxEnemySpawned-enemiesList.Count);
        }
    }

    Vector3 GetSpawnPosition(){
        float wanderRadius = Mathf.Max(0,maxRadius);  //clamp it to positive

        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        Vector3 randPos = (randomDir + player.transform.position);
        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit,maxRadius,1);
        return hit.position;
    }

    void ScanAreaOfInterest(){ //the count indicate how much enemies are inside the AOI
        enemiesList.Clear();

        scannedObj = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i=0;i<scannedObj.Length;i++){
            float dist = (scannedObj[i].transform.position-player.transform.position).magnitude;
            if(dist > maxRadius) continue;
            enemiesList.Add(scannedObj[i]);
        }
    }

    void SpawnGroupOfEnemies(int toSpawn){
        Debug.Log("Spawn Gp");
        if(toSpawn>maxEnemySpawned)    return;
        for(int i=0; i<toSpawn ; i++){
            SpawnEnemy(i);
        }
    }
    
    void SpawnEnemy(int index){
        Debug.Log("SpawnEnemy");
        Vector3 pos = GetSpawnPosition();
        if(!isAdmissibleSpawnPos(pos))  return;

        int enemyType = Random.Range(0,enemy.Length);
        GameObject spanwedEnemy= Instantiate(enemy[enemyType],pos,Quaternion.identity);
        spanwedEnemy.SetActive(true);
        //spanwedEnemy.name+=index;
        spanwedEnemy.tag = "Enemy";
        spanwedEnemy.layer=LayerMask.NameToLayer("EnemyClone");
        currentEnemySpawned++;

        enemiesList.Add(spanwedEnemy);

        Debug.Log("spawned at x ="+pos.x +" y="+pos.y+" z="+pos.z);
    }

    bool isAdmissibleSpawnPos(Vector3 pos){
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 1f, NavMesh.AllAreas))
            return true;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.transform.position, maxRadius);
    }
   

// =========================Depreciated============================

}
