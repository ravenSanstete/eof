using UnityEngine;
using System.Collections;

public class DrivingComputer2 : MonoBehaviour
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
    public Transform flWheelTrans;
    public Transform frWheelTrans;

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
            flWheelCollider.motorTorque = 0;
            frWheelCollider.motorTorque = 0;
            flWheelCollider.brakeTorque = 100;
            frWheelCollider.brakeTorque = 100;
            return;
        }
        flWheelCollider.transform.rotation = Quaternion.LookRotation(poss[nextPos].transform.position - transform.position);
        frWheelCollider.transform.rotation = Quaternion.LookRotation(poss[nextPos].transform.position - transform.position);
        //flWheelCollider.transform.LookAt(poss[nextPos].transform);
        //frWheelCollider.transform.LookAt(poss[nextPos].transform);
        flWheelTrans.transform.LookAt(poss[nextPos].transform);
        frWheelTrans.transform.LookAt(poss[nextPos].transform);

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
        speed = flWheelCollider.rpm * (flWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;


        leftLight.SetActive(false);
        rightLight.SetActive(false);


        float axisV = Random.Range(3,5);

        //if (Input.GetKey(KeyCode.R))
        //{
        //    rigidbody.isKinematic = true;
        //    transform.localPosition = startPos;
        //    transform.localRotation = startQua;
        //    isReset = true;
        //}
        //加速、倒车

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

        if (speed < maxSpeed)
        {
            flWheelCollider.motorTorque = axisV * motorTorque;
            frWheelCollider.motorTorque = axisV * motorTorque;
        }
        else
        {
            flWheelCollider.motorTorque = 0;
            frWheelCollider.motorTorque = 0;
        }

        WheelRotated(axisV);//控制轮子转动和转向
    }
    private void WheelRotated(float axisV)
    {
        //转动
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
            if (other.gameObject.name == poss[nextPos].name && nextPos<47)
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
