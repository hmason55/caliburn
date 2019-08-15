using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlantView : MonoBehaviour {
    public int uniqueId;
    public string ownerId;
    public Vector2 position;
    public int creationDate = 0;
    public int completionDate = 100;
    public int previousGrowthDate = 0;
    public int waterDate = 0;
    public Growable growable;
    public SpriteRenderer waterRenderer;
    public SpriteRenderer plantRenderer;

    public List<GrowthStage> growthStages;

    CircleCollider2D circleCollider2D;
    NetworkIdentity identity;
    
    Coroutine tickGrowthStage = null;
    bool render = true;

    int currentGrowthStage = 0;

    void Awake() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        identity = GetComponent<NetworkIdentity>();
        Destroy(GetComponent<Rigidbody2D>());
    }

    void Start() {
        if(render) {
            RequestGrowableData();
            EvaluateGrowthStage();
        }
    }

    void Update() {
        if(tickGrowthStage == null && render) {
            tickGrowthStage = StartCoroutine(TickGrowthStage());
        }
    }

    void RequestGrowableData() {
        GrowableDataRequest growableData = new GrowableDataRequest {
            networkId = identity.netId
        };
        growableData.HandleRequest();
    }

    public void Water() {
        WaterGrowableRequest waterRequest = new WaterGrowableRequest {
            networkId = identity.netId,
        };
        waterRequest.HandleRequest();
        /*
        int timeRemaining = completionDate - TimeUtility.CurrentUnixTimestamp();
        if(timeRemaining <= 0) { return; }
        Debug.Log("Completion date was " +  completionDate + "s");

        waterDate = TimeUtility.CurrentUnixTimestamp();

        int newTimeRemaining = (int)((float)(completionDate - waterDate) * 0.8f);
        completionDate = waterDate + newTimeRemaining;
        
        
        Debug.Log("Time remaining reduced by " +  (timeRemaining - newTimeRemaining) + "s ( " + (1f - (float)newTimeRemaining / (float)timeRemaining) + "% )");
        Debug.Log("Completion is now " +  completionDate + "s");
        waterRenderer.enabled = true;
        */
    }

    public void UpdateGrowableData(GrowableDataRequest request) {
        if(request.waterDate > waterDate) {
            Debug.Log("Total growth time decreased by " + (completionDate - request.completionDate) + "s");
            Debug.Log("This plant can be watered again in " + (request.waterDate - TimeUtility.CurrentUnixTimestamp()) + "s");
        }

        uniqueId = request.uniqueId;
        ownerId = request.ownerId;
        growable = Growables.Instance.growablesById[request.growableId];
        position = request.position;
        creationDate = request.creationDate;
        completionDate = request.completionDate;
        waterDate = request.waterDate;
    }

    void EvaluateGrowthStage() {
        int currentTime = TimeUtility.CurrentUnixTimestamp();
        
        if(currentTime > completionDate) {
            plantRenderer.sprite = growthStages[growthStages.Count-1].animation[0];
            waterRenderer.enabled = false;
        } else {
            int growthDuration = completionDate - creationDate;
            float progressAsPercentage = (currentTime - creationDate) / (float)growthDuration;
            int ndx = (int)(progressAsPercentage * (growthStages.Count-1));


            if(currentGrowthStage != ndx) {
                currentGrowthStage = ndx;
                waterRenderer.enabled = false;
            }

            plantRenderer.sprite = growthStages[ndx].animation[0];
        }
    }

    IEnumerator TickGrowthStage() {
        while(render) {
            EvaluateGrowthStage();
            yield return new WaitForSecondsRealtime(1f);
        }
        yield break;
    }

}
