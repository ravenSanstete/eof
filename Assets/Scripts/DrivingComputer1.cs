using UnityEngine;
using System.Collections;

public class DrivingComputer1 : MonoBehaviour
{
    public GameObject brakeLine;
    private GameControls gc;
    public GameObject leftLight;
    public GameObject rightLight;
    public Light leftLightColor;
    public Light rightLightColor;
    public ParticleEmitter leftSmoke;
    public ParticleEmitter rightSmoke;
    public UILabel gear;
    public AudioClip soundEngine;
    public AudioClip soundBrake;
    public Transform speedPointTrans;
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;
    public Transform rlWheelTrans;
    public Transform rrWheelTrans;

    public Transform[] wheelTrans;
    public float motorTorque = 465;
    public float steerAngle = 10;
    public Transform centerOfMass;
    public float speed;
    private float wheelAngel;

    private GameObject firstCamera, thirdCamera,finishCamera;
    private bool isFirstPersonView=false;

    private Sound soundManager;
    public float maxSpeed;
    private AudioSource audioSource;

    public Vector3 startPos;
    public Quaternion startQua;
    private bool isReset = false;
    private GameObject[] poss = new GameObject[48];
    private GameObject pos;
    // Use this for initialization
    void Start()
    {
        GetPoss();
        pos = GameObject.Find("poss");

        startPos = transform.position;
        startQua = transform.rotation;
        gc = GameObject.Find("Finish").GetComponent<GameControls>();
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        thirdCamera = GameObject.Find("ThirdCamera");
        firstCamera = GameObject.Find("FirstCamera");
        finishCamera = GameObject.Find("FinishCamera");
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundEngine;
        leftLightColor = leftLight.GetComponent<Light>();
        rightLightColor = rightLight.GetComponent<Light>();
    }
    private void GetPoss()
    {
        int a = 101;
        while (a<149)
        {
            GameObject g=GameObject.Find("pos"+a);
            if(g!=null)
            {
                poss[a - 101] = g;
                a++;
            }
            else
            {
                break;
            }
        }
    }
    private float resetTime = 0;
    private int nextPos=0;
    Transform t;
    // Update is called once per frame
    void Update()
    {
        if(isComputerOver)
        {
            rlWheelCollider.motorTorque = 0;
            rrWheelCollider.motorTorque = 0;
            rlWheelCollider.brakeTorque = 100;
            rrWheelCollider.brakeTorque = 100;
            return;
        }
        rlWheelCollider.transform.rotation = Quaternion.LookRotation(poss[nextPos].transform.position - transform.position);
        rrWheelCollider.transform.rotation = Quaternion.LookRotation(poss[nextPos].transform.position - transform.position);
        //flWheelCollider.transform.LookAt(poss[nextPos].transform);
        //frWheelCollider.transform.LookAt(poss[nextPos].transform);
        rlWheelTrans.transform.LookAt(poss[nextPos].transform);
        rrWheelTrans.transform.LookAt(poss[nextPos].transform);

        leftLightColor.color = Color.yellow;
        rightLightColor.color = Color.yellow;

        //if (isReset)
        //{
        //    resetTime += Time.deltaTime;
        //    if(resetTime>1)
        //    {
        //        rigidbody.isKinematic = false;
        //        resetTime = 0;
        //        isReset = false;
        //    }
        //}

        if (!gc.isStart)
        {
            return;
        }
        speed = rlWheelCollider.rpm * (rlWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;


        leftLight.SetActive(false);
        rightLight.SetActive(false);


        float axisV = Random.Range(3,5);
        float axisH = 0.1f * Random.Range(-1, 1);
        wheelAngel = axisH * steerAngle;



        if (axisV > 0)
        {
            leftSmoke.emit = true;
            rightSmoke.emit = true;
        }
        else
        {
            leftSmoke.emit = false;
            rightSmoke.emit = false;
            if(axisV<0)
            {
                leftLight.SetActive(true);
                rightLight.SetActive(true);
            }
        }



        flWheelCollider.steerAngle = wheelAngel;
        frWheelCollider.steerAngle = wheelAngel;


        if (speed < maxSpeed)
        {
            flWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
            frWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
            rlWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
            rrWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed ;
        }
        else
        {
            rlWheelCollider.motorTorque = 0;
            rrWheelCollider.motorTorque = 0;
        }

        WheelRotated(axisV);
    }
    private void WheelRotated(float axisV)
    {

        foreach (Transform wheel in wheelTrans)
        {
            wheel.Rotate(flWheelCollider.rpm * Time.deltaTime * 6, 0, 0);
        }
    }
    private bool isComputerOver = false;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Finish")
        {
            if (other.gameObject.name == poss[nextPos].name && nextPos < 47)
            {
                nextPos++;
            }
        }
        else
        {
            isComputerOver = true;
        }
    }

}
