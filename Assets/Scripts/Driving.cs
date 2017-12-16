using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

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
    public float motorTorque = 450;
    public float maxBrakeTorque = 1000;
    public float steerAngle = 5;
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
    private float original_mass = 0.0f;


    private WheelFrictionCurve r_frict;
    private WheelFrictionCurve f_frict;
    public List<float> logs;

    private string LOG_FORMAT = "/Users/morino/Desktop/bp/{0}.csv";
    private string log_path;
    private string generate_random_record_name(){
      return String.Format(LOG_FORMAT, Guid.NewGuid().ToString());
    }
    private Vector3 generate_random_position(){
      float s = -0.8f;
      float e = 1.5f;
      float bound = 1.0f;
      float y = 102.9f;
      float x_0 = 1088.177f;
      float x_1 = 1095.667f;
      float z_0 = 650.7893f;
      float z_1 = 658.1313f;

      // compute the normal
      Vector3 n = new Vector3(z_0 - z_1, 0 , x_1 - x_0);

      float k = UnityEngine.Random.Range(0.0f, bound);
      // float k = bound;
      float t = UnityEngine.Random.Range(s, e);
      //print("Random Position:"+t);
      //float t = e
      return new Vector3(x_0 + (x_1 - x_0) * t, y, z_0 + (z_1 - z_0) * t) + n * k; //with some fluctuation over the normal direction
    }

    // Use this for initialization
    void Start()
    {
        UnityEngine.Random.seed = (int)DateTime.Now.Ticks;
        // generate random log log_path;
        log_path = generate_random_record_name();
        print(log_path);
        logs = new List<float>();
        startPos = transform.position;
        startQua = transform.rotation;
        gc = GameObject.Find("Finish").GetComponent<GameControls>();
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        original_mass = GetComponent<Rigidbody>().mass;

        thirdCamera = GameObject.Find("ThirdCamera");
        firstCamera = GameObject.Find("FirstCamera");
        finishCamera = GameObject.Find("FinishCamera");
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundEngine;
        leftLightColor = leftLight.GetComponent<Light>();
        rightLightColor = rightLight.GetComponent<Light>();
        r_frict= rlWheelCollider.sidewaysFriction;
        f_frict = flWheelCollider.sidewaysFriction;
        //set the starting point of the car
        this.gameObject.transform.position = generate_random_position();

    }


    private float resetTime = 0;

    //parameter for the items
    /* BANANA */
    public static bool banana = false;
    private float banana_time = 0.0f;
    private float banana_duration = 1.0f;

    /* SHRINK */
    public static bool shrink = false;
    private float shrink_time = 0.0f;
    private float shrink_duration = 5.0f;
    private float shrink_rate = 0.5f;

    /* MIRA */
    public static bool mirror = false;
    private float mirror_time = 0.0f;
    private float mirror_duration = 7.0f;

    /* LOSS gravity */
    public static bool anti_gravity = false;
    private float anti_gravity_time = 0.0f;
    private float anti_gravity_duration = 3.0f;
    private float new_mass = 90.0f;
    private bool anti_gravity_rd = false;
    private float anti_gravity_ub = 1.0f;
    private Quaternion anti_gravity_or;


    /* freeze */
    public static bool freeze = false;
    private float freeze_time = 0.0f;
    private float freeze_duration = 1.0f;





    private int k = 24; //24 frame per log
    private int count = 0;

    private bool has_written = false;


    // Update is called once per frame
    void Update()
    {


        if (!gc.isStart){
                  finishCamera.SetActive(false);
                  firstCamera.SetActive(false);
                  thirdCamera.SetActive(true);

                  return;
              }

      // print("rpm: " + rlWheelCollider.rpm);
      // print("torque: " + rlWheelCollider.motorTorque);

          //add data to the log

          if(!BananaEffect.item_enabled && !has_written){
            Transform current = this.gameObject.transform;
            print("log");
            logs.Add(current.position.x);
            logs.Add(current.position.y);
            logs.Add(current.position.z);
            logs.Add(current.eulerAngles.x);
            logs.Add(current.eulerAngles.y);
            logs.Add(current.eulerAngles.z);
            //print(current.eulerAngles);
            //print(logs.Count);
          }


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



        float axisV = Input.GetAxis("Vertical");
        float axisH = Input.GetAxis("Horizontal");
/*** HERE BEGIN THE IMPLEMENTATION OF THE ITEMS***/

        // for the banana effect
        if(Driving.banana){
          print("banana effect");
          banana_time += Time.deltaTime;

          leftLightColor.color = Color.green;
          leftLight.SetActive(true);
          rightLightColor.color = Color.green;
          rightLight.SetActive(true);

          if(banana_time > banana_duration){
            banana_time = 0;
            Driving.banana = false;
            print("banana over");
          }
        }


        //for the shrink effect
        if(Driving.shrink){
          print("shrink effect");
          shrink_time += Time.deltaTime;

          leftLightColor.color = Color.black;
          leftLight.SetActive(true);
          rightLightColor.color = Color.black;
          rightLight.SetActive(true);
          this.gameObject.transform.localScale = new Vector3(shrink_rate, shrink_rate, shrink_rate);

          if(shrink_time > shrink_duration){
            shrink_time = 0;
            Driving.shrink = false;
            print("shrink over");
          }
        }else{
            this.gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        }


        // for the mirror effect
        if(Driving.mirror){
          print("mirror effect");
          mirror_time += Time.deltaTime;

          leftLightColor.color = Color.green;
          leftLight.SetActive(true);
          rightLightColor.color = Color.green;
          rightLight.SetActive(true);

          //effect (mirror)
          axisH = -axisH;

          if(mirror_time > mirror_duration){
            mirror_time = 0;
            Driving.mirror = false;
            print("mirror over");
          }
        }

        // for the anti-gravity effect
        if(Driving.anti_gravity){
          print("anti_gravity effect");
          anti_gravity_time += Time.deltaTime;

          leftLightColor.color = Color.green;
          leftLight.SetActive(true);
          rightLightColor.color = Color.green;
          rightLight.SetActive(true);

          if(!anti_gravity_rd){
              anti_gravity_duration = UnityEngine.Random.Range(0.5f, anti_gravity_ub);
              anti_gravity_rd = true;
              anti_gravity_or = this.gameObject.transform.rotation;
          }

          this.gameObject.transform.rotation = anti_gravity_or;
          //effect (anti_gravity)
          GetComponent<Rigidbody>().mass = new_mass;
          //GetComponent<Rigidbody>().detectCollisions = false;

          //GetComponent<Rigidbody>().isKinematic = true;
          if(anti_gravity_time > anti_gravity_duration){
            anti_gravity_time = 0;
            Driving.anti_gravity = false;
            print("anti_gravity over");
          }

        }else{
          GetComponent<Rigidbody>().mass = original_mass;
          GetComponent<Rigidbody>().detectCollisions = true;
          //GetComponent<Rigidbody>().isKinematic = false;
          anti_gravity_rd = false;
        }


        // for the freeze effect
        if(Driving.freeze){
          print("freeze effect");
          freeze_time += Time.deltaTime;

          leftLightColor.color = Color.green;
          leftLight.SetActive(true);
          rightLightColor.color = Color.green;
          rightLight.SetActive(true);

          //effect for freeze
          GetComponent<Rigidbody>().isKinematic = true;
          if(freeze_time > freeze_duration){
            freeze_time = 0;
            Driving.freeze = false;
            print("freeze over");
          }
        }else{
            GetComponent<Rigidbody>().isKinematic = false;
        }




/*** HERE END THE IMPLEMENTATION OF THE ITEMS***/




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



            //write logs
            if(!BananaEffect.item_enabled && !has_written){
                write_log(logs);
                has_written = true;
            }
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



        if (axisV < 0)
        {
            //print("Brake");
            BrakeCar();
        }
        else
        {
            if(!isPY)
            audioSource.clip = soundEngine;
            if (speed > maxSpeed)
                speed = maxSpeed;
            GetComponent<AudioSource>().pitch = maxSpeed/300 + speed / maxSpeed;
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
        // if (speed == 0)
        // {
        //     leftLight.SetActive(false);
        //     rightLight.SetActive(false);
        //     audioSource.enabled = false;
        //     GetComponent<AudioSource>().Stop();
        //     return;
        // }
        leftSmoke.emit = false;
        rightSmoke.emit = false;
        leftLight.SetActive(true);
        rightLight.SetActive(true);


        flWheelCollider.motorTorque = -maxBrakeTorque;
        frWheelCollider.motorTorque = -maxBrakeTorque;


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





    	private void write_log(List<float> logs){
        FileStream fs;
    	    if(!File.Exists(log_path)){
    	       fs = File.Create(log_path);
    	    }else{
             fs = File.Open(log_path, FileMode.Open, FileAccess.Write, FileShare.None);
          }

    	        StreamWriter sw = new StreamWriter(fs);
    	        int i = 0;
              print(logs.Count);
    					while(i < logs.Count){
    						sw.WriteLine(logs[i] + " " + logs[i+1]+ " " + logs[i+2]+" "+logs[i+3] + " " + logs[i+4] + " " + logs[i+5]);
    						i = i + 6;
    					}
          print("write over");
          sw.Close();


    	 }
}
