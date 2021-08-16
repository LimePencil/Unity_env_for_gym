using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;



public class MoveToGoalScrip : Agent
{
    private Transform tr;
    private Rigidbody rb;
    //public Renderer floorRender;

    //바닥의 색생을 변경하기 위한 머티리얼
    private Material originMt;     
    public Material badMt;          
    public Material goodMt;
    public GameObject goalPrefab;
    private GameObject goalClone = null;

    MazeRenderer render;
    Transform parentTransform;
    //초기화 작업을 위해 한번 호출되는 메소드

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        render = GetComponentInParent<MazeRenderer>();
        parentTransform = GetComponentInParent<MazeRenderer>().transform;
        instantiateGoal();
        //originMt = floorRender.material;
    }

    //에피소드(학습단위)가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        
        //물리력을 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        tr.localPosition = new Vector3(0, 0.27f, 0);
        Destroy(goalClone);
        instantiateGoal();
        //StartCoroutine(RevertMaterial());
    }
    private void instantiateGoal()
    {
        if(render.width % 2 != 0)
        {
            goalClone = (GameObject)Instantiate(goalPrefab, parentTransform.position + new Vector3((((render.width - 1) / 2.0f) * (Random.Range(0, 2) * 2 - 1)), 0.25f, ((render.height - 1) / 2.0f) * (Random.Range(0, 2) * 2 - 1)), Quaternion.identity, parentTransform);
        }
        else
        {
            goalClone = (GameObject)Instantiate(goalPrefab, parentTransform.position + new Vector3((-0.5f + ((render.width-1) / 2.0f) * (Random.Range(0, 2) * 2 - 1)), 0.25f, (-0.5f + ((render.width -1)/ 2.0f) * (Random.Range(0, 2) * 2 - 1))), Quaternion.identity, parentTransform);
        }

    }

    //IEnumerator RevertMaterial()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    floorRender.material = originMt;
    //}
    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(goalClone.transform.localPosition);  //3 (x,y,z)
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        sensor.AddObservation(tr.rotation.y);           //1 (x)
    }
    //브레인(정책)으로 부터 전달 받은 행동을 실행하는 메소드
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //데이터를 정규화
        float h = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1.0f, 1.0f);
        Vector3 dir = (Vector3.forward * v);
        tr.Translate(dir * 0.005f);
        tr.Rotate(Vector3.up * 0.1f*h);
        //지속적으로 이동을 이끌어내기 위한 마이너스 보상
        //SetReward(-0.001f + distanceReward(tr.localPosition.x, tr.localPosition.z, goalClone.transform.localPosition.x, goalClone.transform.localPosition.z));
        AddReward(-0.001f);
        if(StepCount == MaxStep)
        {
            Debug.Log("Finished without Reaching");
            AddReward(-1.0f);
            EndEpisode();
        }
    }
    //private float distanceReward(float playerX,float playerZ, float targetX,float targetZ)
    //{
    //    float distance = Mathf.Sqrt(Mathf.Pow(playerX - targetX, 2) + Mathf.Pow(playerZ - targetZ, 2));
    //    float maxDisPos = Mathf.Sqrt(Mathf.Pow(render.width - 0.5f, 2) + Mathf.Pow(render.height - 0.5f, 2));
    //    float reward = 0.0001f * (maxDisPos-distance);
    //    return reward;
    //}

    //개발자(사용자)가 직접 명령을 내릴때 호출하는 메소드(주로 테스트용도 또는 모방학습에 사용)
    public override void Heuristic(in ActionBuffers actionBuffersOut)
    {
        ActionSegment<float> ContinuousActions = actionBuffersOut.ContinuousActions;
        ContinuousActions[0] = Input.GetAxis("Horizontal"); 
        ContinuousActions[1] = Input.GetAxis("Vertical");   
        //Debug.Log($"[0]={ContinuousActions[0]} [1]={ContinuousActions[1]}");
    }
    void OnCollisionEnter(Collision coll)
    {
        /*
        if (coll.collider.CompareTag("Wall"))
        {
            floorRd.material = badMt;
            //잘못된 행동일 때 마이너스 보상을 준다.
            SetReward(-1.0f);
            //학습을 종료시키는 메소드
            EndEpisode();
        }
        */
        if (coll.collider.CompareTag("Target"))
        {

            //floorRender.material = goodMt;
            //올바른 행동일 때 플러스 보상을 준다.
            Debug.Log("Collided!");
            SetReward(+1.0f);
            //학습을 종료시키는 메소드
            EndEpisode();
        }
    }
}
