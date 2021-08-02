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

    //�ٴ��� ������ �����ϱ� ���� ��Ƽ����
    private Material originMt;     
    public Material badMt;          
    public Material goodMt;
    public GameObject goalPrefab;
    private GameObject goalClone = null;

    public int multiplier = 4;
    MazeRenderer render;
    Transform parentTransform;
    //�ʱ�ȭ �۾��� ���� �ѹ� ȣ��Ǵ� �޼ҵ�

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        render = GetComponentInParent<MazeRenderer>();
        parentTransform = GetComponentInParent<MazeRenderer>().transform;
        instantiateGoal();
        //originMt = floorRender.material;
    }

    //���Ǽҵ�(�н�����)�� �����Ҷ����� ȣ��
    public override void OnEpisodeBegin()
    {
        
        //�������� �ʱ�ȭ
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        tr.localPosition = new Vector3(0, 1, 0);
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
    //ȯ�� ������ ���� �� ������ ��å ������ ���� �극�ο� �����ϴ� �޼ҵ�
    //public override void CollectObservations(VectorSensor sensor)
    //{
    //    sensor.AddObservation(targetTr.localPosition);  //3 (x,y,z)
    //    sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
    //    sensor.AddObservation(rb.velocity.x);           //1 (x)
    //    sensor.AddObservation(rb.velocity.z);           //1 (z)
    //}
    //�극��(��å)���� ���� ���� ���� �ൿ�� �����ϴ� �޼ҵ�
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //�����͸� ����ȭ
        float h = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1.0f, 1.0f);
        Vector3 dir = (Vector3.forward * v);
        tr.Translate(dir * 0.005f*4);
        tr.Rotate(Vector3.up * 0.1f*h*4);
        //���������� �̵��� �̲���� ���� ���̳ʽ� ����
        SetReward(-0.001f);
    }

    //������(�����)�� ���� ����� ������ ȣ���ϴ� �޼ҵ�(�ַ� �׽�Ʈ�뵵 �Ǵ� ����н��� ���)
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
            //�߸��� �ൿ�� �� ���̳ʽ� ������ �ش�.
            SetReward(-1.0f);
            //�н��� �����Ű�� �޼ҵ�
            EndEpisode();
        }
        */
        if (coll.collider.CompareTag("Target"))
        {

            //floorRender.material = goodMt;
            //�ùٸ� �ൿ�� �� �÷��� ������ �ش�.
            Debug.Log("Collided!");
            SetReward(+1.0f);
            //�н��� �����Ű�� �޼ҵ�
            EndEpisode();
        }
    }
}
