using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAOISpawner : MonoBehaviour
{
    public enum Category{ STAGE1,STAGE2,STAGE3}   //used to indicate which stage of the model shd be generated

    public GameObject[] stage1Enemy ;
    public GameObject[] stage2Enemy ;
    public GameObject[] stage3Enemy ;

    public float maxRadius;
    public GameObject player;
    public int maxEnemySpawned;
    public float spawnPeriod;
    public int maxLvDiff;
    
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
        int enemyLv = GetAdmissibleEnemyLevel();
        Category enemyCategory = GetEnemyCategoryWithLevel(enemyLv);
        for(int i=0; i<toSpawn ; i++){
            SpawnEnemy(enemyCategory,enemyLv);
        }
    }
    
    void SpawnEnemy(Category stage,int enemyLv){
        Debug.Log("SpawnEnemy");
        Vector3 pos = GetSpawnPosition();
        if(!isAdmissibleSpawnPos(pos))  return;

        int enemyType =0;
        GameObject spanwedEnemy=null;
        if(stage==Category.STAGE1){
            enemyType = Random.Range(0,stage1Enemy.Length);
            spanwedEnemy= Instantiate(stage1Enemy[enemyType],pos,Quaternion.identity);
            spanwedEnemy.GetComponent<EnemyAttributes>().expGainedByPlayer*=Mathf.Pow(2,1);
        }
        else if(stage==Category.STAGE2){
            enemyType = Random.Range(0,stage2Enemy.Length);
            spanwedEnemy= Instantiate(stage2Enemy[enemyType],pos,Quaternion.identity);
            spanwedEnemy.GetComponent<EnemyAttributes>().expGainedByPlayer*=Mathf.Pow(2,2);
        }
        else{
            enemyType = Random.Range(0,stage3Enemy.Length);
            spanwedEnemy= Instantiate(stage3Enemy[enemyType],pos,Quaternion.identity);
            spanwedEnemy.GetComponent<EnemyAttributes>().expGainedByPlayer*=Mathf.Pow(2,3);
        }

        spanwedEnemy.SetActive(true);
        spanwedEnemy.tag = "Enemy";
        spanwedEnemy.layer=LayerMask.NameToLayer("EnemyClone");
        spanwedEnemy.GetComponent<EnemyAttributes>().Lv = enemyLv;
        spanwedEnemy.GetComponent<EnemyAttributes>().UpdateCharacterCurrentAttributes();
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
   
    private int GetAdmissibleEnemyLevel(){
        int playerLv = player.GetComponent<PlayerAttributes>().Lv;

        return Mathf.Clamp(Random.Range(playerLv-maxLvDiff,playerLv+maxLvDiff),1,100);
    } 

    private Category GetEnemyCategoryWithLevel(int lv){
        if(lv<15)   return Category.STAGE1;
        else if(lv<40)   return Category.STAGE2;
        else    return Category.STAGE3;
    }
// =========================Depreciated============================

}
