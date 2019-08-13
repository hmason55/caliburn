using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlantView : MonoBehaviour {
    public string ownerId;
    public Vector2 position;
    public int creationDate = 0;
    public int completionDate = 100;
    public Growable growable;

    public List<GrowthStage> growthStages;

    CircleCollider2D circleCollider2D;
    SpriteRenderer spriteRenderer;
    Coroutine tickGrowthStage = null;
    bool render = true;

    void Awake() {
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Destroy(GetComponent<Rigidbody2D>());
    }

    void Start() {
        if(render) {
            RequestGrowableData();
        }
    }

    void Update() {
        if(tickGrowthStage == null && render) {
            tickGrowthStage = StartCoroutine(TickGrowthStage());
        }
    }

    void RequestGrowableData() {
        GrowableDataRequest growableData = new GrowableDataRequest {
            networkId = GetComponent<NetworkIdentity>().netId
        };
        growableData.HandleRequest();
    }

    public void UpdateGrowableData(GrowableDataRequest request) {
        ownerId = request.ownerId;
        creationDate = request.creationDate;
        completionDate = request.completionDate;
    }

    void EvaluateGrowthStage() {
        int currentTime = TimeUtility.CurrentUnixTimestamp();

        if(currentTime > completionDate) {
            spriteRenderer.sprite = growthStages[growthStages.Count-1].animation[0];
        } else {
            int growthDuration = completionDate - creationDate;
            float progressAsPercentage = (currentTime - creationDate) / (float)growthDuration;
            int ndx = (int)(progressAsPercentage * (growthStages.Count-1));
            spriteRenderer.sprite = growthStages[ndx].animation[0];
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
