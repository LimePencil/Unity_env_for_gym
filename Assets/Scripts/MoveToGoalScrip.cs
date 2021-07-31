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
    public Transform targetTr;
    public Renderer floorRd;

    //�ٴ��� ������ �����ϱ� ���� ��Ƽ����
    private Material originMt;     
    public Material badMt;          
    public Material goodMt;         
    //�ʱ�ȭ �۾��� ���� �ѹ� ȣ��Ǵ� �޼ҵ�

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        originMt = floorRd.material;
    }

    //���Ǽҵ�(�н�����)�� �����Ҷ����� ȣ��
    public override void OnEpisodeBegin()
    {
        //�������� �ʱ�ȭ
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //������Ʈ�� ��ġ�� �ұ�Ģ�ϰ� ����
        tr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f)
                                     , 1f
                                     , Random.Range(-4.0f, 4.0f));
        targetTr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f)
                                            , 1f
                                            , Random.Range(-4.0f, 4.0f));
        StartCoroutine(RevertMaterial());
    }

    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }
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
        rb.AddRelativeForce(dir.normalized * 50.0f);
        tr.Rotate(Vector3.up * 10.0f*h);
        //���������� �̵��� �̲���� ���� ���̳ʽ� ����
        SetReward(-0.001f);
    }

    //������(�����)�� ���� ����� ������ ȣ���ϴ� �޼ҵ�(�ַ� �׽�Ʈ�뵵 �Ǵ� ����н��� ���)
    public override void Heuristic(in ActionBuffers actionBuffersOut)
    {
        ActionSegment<float> ContinuousActions = actionBuffersOut.ContinuousActions;
        ContinuousActions[0] = Input.GetAxis("Horizontal"); 
        ContinuousActions[1] = Input.GetAxis("Vertical");   
        Debug.Log($"[0]={ContinuousActions[0]} [1]={ContinuousActions[1]}");
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Wall"))
        {
            floorRd.material = badMt;
            //�߸��� �ൿ�� �� ���̳ʽ� ������ �ش�.
            SetReward(-1.0f);
            //�н��� �����Ű�� �޼ҵ�
            EndEpisode();
        }

        if (coll.collider.CompareTag("Target"))
        {
            floorRd.material = goodMt;
            //�ùٸ� �ൿ�� �� �÷��� ������ �ش�.
            SetReward(+1.0f);
            //�н��� �����Ű�� �޼ҵ�
            EndEpisode();
        }
    }
}
