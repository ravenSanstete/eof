using UnityEngine;
using System.Collections;

public class Driving : MonoBehaviour
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
    public float motorTorque = 500;
    public float steerAngle = 10;
    public Transform centerOfMass;
    public float speed;
    private float wheelAngel;

    private GameObject firstCamera, thirdCamera,finishCamera;
    private bool isFirstPersonView=false;

    private Sound soundManager;
    public float maxSpeed = 180;
    private AudioSource audioSource;

    public Vector3 startPos;
    public Quaternion startQua;
    private bool isReset = false;
    private bool isFire = false;
    private float fireTime = 0;


    private WheelFrictionCurve r_frict;
    private WheelFrictionCurve f_frict;
    // Use this for initialization
    void Start()
    {
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
        r_frict= rlWheelCollider.sidewaysFriction;
        f_frict = flWheelCollider.sidewaysFriction;


    }
    private float resetTime = 0;
    public static bool banana = false;

    private float banana_time = 0.0f;





    // Update is called once per frame
    void Update()
    {
      // print("rpm: " + rlWheelCollider.rpm);
      // print("torque: " + rlWheelCollider.motorTorque);

        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        leftLightColor.color = Color.yellow;
        rightLightColor.color = Color.yellow;



        if(isFire)
        {
            leftLightColor.color = Color.blue;
            leftLight.SetActive(true);
            rightLightColor.color = Color.blue;
            rightLight.SetActive(true);

            fireTime += Time.deltaTime;
            if (speed < maxSpeed)
            {
                rlWheelCollider.motorTorque = 4 * motorTorque;
                rrWheelCollider.motorTorque = 4 * motorTorque;
            }
            if(fireTime>0.5f)
            {
                fireTime = 0;
                isFire = false;
            }
            return;
        }

        // for the banana effect
        if(Driving.banana){
          print("banana effect");
          banana_time += Time.deltaTime;

          leftLightColor.color = Color.green;
          leftLight.SetActive(true);
          rightLightColor.color = Color.green;
          rightLight.SetActive(true);

          if(banana_time > 1.0f){
            banana_time = 0;
            Driving.banana = false;
            print("banana over");
          }
        }


        if (isReset)
        {
            resetTime += Time.deltaTime;
            if(resetTime>1)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                resetTime = 0;
                isReset = false;
            }
        }

        if (!gc.isStart)
        {
            finishCamera.SetActive(false);
            firstCamera.SetActive(false);
            thirdCamera.SetActive(true);
            return;
        }


        //compute the speed of the cars
        speed = rlWheelCollider.rpm * (rlWheelCollider.radius * 2 * Mathf.PI) * 60 / 1000;




        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isFire = true;
        }




        SpeedPtRotate(speed);



        leftLight.SetActive(false);
        rightLight.SetActive(false);


        if (gc.isOver)
        {
            finishCamera.SetActive(true);
            firstCamera.SetActive(false);
            thirdCamera.SetActive(false);
            GetComponent<AudioSource>().volume = (30 - Vector3.Distance(transform.position, thirdCamera.transform.position)) / 30;
            BrakeCar();
            return;
        }
        else
        {
            finishCamera.SetActive(false);
            if (Input.GetKeyDown(KeyCode.V))
            {
                isFirstPersonView = !isFirstPersonView;
            }
            if (isFirstPersonView)
            {
                firstCamera.SetActive(true);
                thirdCamera.SetActive(false);
            }
            else
            {
                firstCamera.SetActive(false);
                thirdCamera.SetActive(true);
            }
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            maxSpeed = 80;
            gear.text = "1";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            maxSpeed = 120;
            gear.text = "2";
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            maxSpeed = 160;
            gear.text = "3";
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            maxSpeed = 200;
            gear.text = "4";
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            maxSpeed = 280;
            gear.text = "5";
        }


        float axisV = Input.GetAxis("Vertical");
        float axisH = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.R))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.localPosition = startPos;
            transform.localRotation = startQua;
            isReset = true;
        }




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


        wheelAngel = axisH * steerAngle;

        WheelFrictionCurve a = new WheelFrictionCurve();
        a.asymptoteSlip = 0.4f;
        a.extremumSlip = 0.8f;
        a.stiffness = 1f;
        WheelFrictionCurve b = a;
        bool isPY = false;

        //print(speed);



        Transform t = transform;
        if (Input.GetKey(KeyCode.LeftShift) || Driving.banana)
        {
            leftLightColor.color = Color.red;
            leftLight.SetActive(true);
            rightLightColor.color = Color.red;
            rightLight.SetActive(true);






            t = transform;
            isPY = true;
            b.extremumValue = 0.1f;
            b.asymptoteValue = 0.05f;
            a.extremumValue = 0.1f;
            a.asymptoteValue = 0.05f;
            //BrakeCar();
            if (audioSource.clip != soundBrake)
            {
                audioSource.clip = soundBrake;
            }

            audioSource.pitch = 1;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            //print("shift sideway set");
        }else{
          b = f_frict;
          a = f_frict;
          //print("sideway reset");
        }


        flWheelCollider.sidewaysFriction = b;
        frWheelCollider.sidewaysFriction = b;
        rlWheelCollider.sidewaysFriction = a;
        rrWheelCollider.sidewaysFriction = a;
        ////
        flWheelCollider.steerAngle = wheelAngel;
        frWheelCollider.steerAngle = wheelAngel;



        if (speed * axisV < 0)
        {
            BrakeCar();
        }
        else
        {
            if(!isPY)
            audioSource.clip = soundEngine;
            if (speed > maxSpeed)
                speed = maxSpeed;
            GetComponent<AudioSource>().pitch = maxSpeed/300 + speed / maxSpeed;
            flWheelCollider.brakeTorque = 0;
            frWheelCollider.brakeTorque = 0;
            rlWheelCollider.brakeTorque = 0;
            rrWheelCollider.brakeTorque = 0;
            if(!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Play();
        }
        if (speed < maxSpeed)
        {
          //print("axisV: " + axisV);
          //print("motorTorque: " + motorTorque);
          flWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
          frWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
            rlWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
            rrWheelCollider.motorTorque = axisV * motorTorque * (100 + speed) / maxSpeed;
            //print("after set rl: " + rlWheelCollider.motorTorque);
        }
        else
        {
            rlWheelCollider.motorTorque = 0;
            rrWheelCollider.motorTorque = 0;
        }

        WheelRotated(axisV);
    }
    float pyTime = 0;



    private void BrakeCar()
    {
        if (speed == 0)
        {
            leftLight.SetActive(false);
            rightLight.SetActive(false);
            audioSource.enabled = false;
            GetComponent<AudioSource>().Stop();
            return;
        }
        leftSmoke.emit = false;
        rightSmoke.emit = false;
        leftLight.SetActive(true);
        rightLight.SetActive(true);

        rlWheelCollider.motorTorque = 0;
        rrWheelCollider.motorTorque = 0;

        rlWheelCollider.brakeTorque = 100;
        rrWheelCollider.brakeTorque = 100;
        //audio.Stop();
        if (audioSource.clip != soundBrake)
        {
            audioSource.clip = soundBrake;
        }

        audioSource.pitch = 1;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }


    private void SpeedPtRotate(float speed){
      Quaternion target;
      float smooth = 2.0f;
       if (speed > 0)
           target = Quaternion.Euler(0, 0, -130 - speed * 27 / 14);
       else
           target = Quaternion.Euler(0, 0, -130);

      speedPointTrans.rotation = Quaternion.Slerp(speedPointTrans.rotation, target, Time.deltaTime * smooth);
    }




    private void WheelRotated(float axisV)
    {
        foreach (Transform wheel in wheelTrans)
        {
            wheel.Rotate(flWheelCollider.rpm * Time.deltaTime * 6, 0, 0);
        }
        flWheelTrans.localEulerAngles = new Vector3(0, wheelAngel * 3, 0);
        frWheelTrans.localEulerAngles = new Vector3(0, wheelAngel * 3, 0);
    }
}
