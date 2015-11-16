using UnityEngine;
using System.Collections;

public class unirotate: MonoBehaviour {

	public Transform  target;
	public Transform  target1;
	public GameObject GB_LeftMove;
	public GameObject GB_RtMove;
	public GameObject GB_UpMove;
	public GameObject GB_DnMove;
	public Transform  target2;
    public float edgeBorder = 0.1f;
    public float horizontalSpeed = 360.0f;
	public float horizontalSpeedMultify = 0.1f;
    public float verticalSpeed = 120.0f;
    public float minVertical = 20.0f;
    public float maxVertical = 85.0f;
	public float cameraShiftHorizon = 2.0f;
    public float cameraShiftVertical = 2.0f;
    public bool  autoRotate = true;

	
	public float MouseWheelSensitivity = 10.0f;
	public float MouseZoomMin = 1.0f;
	public float MouseZoomMax = 7.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    private float distance = 0.0f;
	private float verticalValue=0.0f;
	private float horizontalValue=0.0f;
	private bool isEarth=false;
	private bool isMercury=false;
	private bool isVenus=false;
	private bool isMars=false;
	private bool isJupiter=false;
	private bool isSaturn=false;
	private bool isUranus=false;
	private bool isNeptune=false;
	private float dt;

	//public GUISkin New_skin2;
	//public GUISkin New_skin3;
	
	//public Texture2D backMap;
	public GUIStyle   btnStyle1;
	public GUIStyle   btnStyle2;
	public GUIStyle   btnStyle3;
	public GUIStyle   btnStyle4;
	public GUIStyle   btnStyle5;

    private Vector3 frompos;
    private Quaternion fromrot;
    private Vector3 topos;
    private Quaternion torot;
    private bool movecamera = false;
    private float springiness = 4.0f;
    private float movecamerabegintime;
    private bool isenabled = false;
	private const float movelasttime = 2f;
	private bool leftMove;
	private bool rtMove;
	private bool upMove;
	private bool dnMove;
	public UISprite m_SpriteHandLeft=null;
	public UISprite m_SpriteHandRight=null;
	public UISprite m_SpriteCursorLoading=null;

	
	// Use this for initialization
	void Start () {

		if(CKinect.KinectServices.Instance == null)
		{
			CKinect.KinectServices.Create(m_SpriteHandLeft,m_SpriteHandRight,m_SpriteCursorLoading);
		}
		else if(CKinect.CursorController.Instance != null)
		{
			CKinect.CursorController.Instance.SpriteHandLeft = m_SpriteHandLeft;
			CKinect.CursorController.Instance.SpriteHandRight = m_SpriteHandRight;
			CKinect.CursorController.Instance.SpriteCursorLoading = m_SpriteCursorLoading;
			
		}

		UIEventListener.Get(this.GB_LeftMove).onPress= (go,b) =>{

			leftMove = b;

		};
		UIEventListener.Get(this.GB_RtMove).onPress= (go,b) =>{
			
			rtMove = b;

		};
		UIEventListener.Get(this.GB_UpMove).onPress= (go,b) =>{
			
			upMove = b;
			
		};
		UIEventListener.Get(this.GB_DnMove).onPress= (go,b) =>{
			
			dnMove = b;
			
		};
		//x = transform.eulerAngles.y;
       // y = transform.eulerAngles.x;
        //distance = (transform.position - target.position).magnitude;

        MoveCamera0();
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();

		}

		if (leftMove) {
			horizontalValue += dt*cameraShiftHorizon;
		}
		if (rtMove) {
			horizontalValue -= dt*cameraShiftHorizon;
		}
		if (upMove) {
			verticalValue -= dt*cameraShiftVertical;
		}
		if (dnMove) {
			verticalValue += dt*cameraShiftVertical;
		}
		
	}
	
	void LateUpdate()
   {
		//onClick ();
	    //float dt = Time.deltaTime;
	   dt = Time.deltaTime;
		if (isEarth) {
			topos = new Vector3(34f, 0.65f, 39.33f);
		}
       if (movecamera)
       {
           float ratio;
           ratio = (Time.time - movecamerabegintime) / movelasttime;
           if (ratio > 1) ratio = 1f;
           //transform.position = Vector3.Lerp(transform.position, topos, Time.deltaTime * springiness);
           transform.position = Vector3.Lerp(frompos, topos, ratio);
          // transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, Time.time * speed);
           transform.rotation = Quaternion.Lerp(fromrot, torot, ratio);

           x = transform.eulerAngles.y;
           y = transform.eulerAngles.x;
           distance = (transform.position - target.position).magnitude;

           //print((transform.position - topos).magnitude);

           //if ((transform.position - topos).magnitude < 0.001f)
           if (Time.time - movecamerabegintime > movelasttime + 0.5f)
           {
               x = transform.eulerAngles.y;
               y = transform.eulerAngles.x;
               distance = (transform.position - target.position).magnitude;
               horizontalValue = 0f;
               verticalValue = 0f;

               //transform.position = topos;
               //transform.rotation = torot;
               movecamera = false;
               isenabled = true;
              // print(movecamera);
               
           }

       }

       if (Input.GetMouseButton(1))
       {

           horizontalValue -= dt * cameraShiftHorizon * Input.GetAxis("Mouse X") * 1;
           verticalValue -= dt * cameraShiftVertical * Input.GetAxis("Mouse Y") * 1;
       }
	   
	    if(Input.GetKey(KeyCode.V))
		{
			horizontalValue += dt*cameraShiftHorizon;
		}
		else if(Input.GetKey(KeyCode.B))
		{
			
			horizontalValue -= dt*cameraShiftHorizon;
	
		}
		else if(Input.GetKey(KeyCode.F))
		{
			
			verticalValue -= dt*cameraShiftVertical;
	
		}
		else if(Input.GetKey(KeyCode.G))
		{
			
			verticalValue += dt*cameraShiftVertical;
	
		}
		else if(Input.GetKey(KeyCode.R))
		{
			
			autoRotate = !autoRotate;

		}
		else if(Input.GetAxis("Mouse ScrollWheel")!=0.0f)
		{
			 distance += Input.GetAxis("Mouse ScrollWheel")*MouseWheelSensitivity;
		}
		
		if(autoRotate)
			x -= horizontalSpeedMultify * horizontalSpeed * dt;
		else
		    x -= Input.GetAxis("Horizontal") * horizontalSpeed * dt;
		
	    y += Input.GetAxis("Vertical") * verticalSpeed * dt;
		
		if(Input.GetMouseButton (0))
		{
			if(!autoRotate) 
			   x+=Input.GetAxis("Mouse X")* horizontalSpeed*dt;
			   y -= Input.GetAxis("Mouse Y") * verticalSpeed * dt;
		}

        if (!movecamera)
        {
			if(isEarth==true){
				transform.position = target1.position+new Vector3(1,1,1);
				transform.LookAt(target1);
			}
			else if(isMercury==true){
				transform.position = target2.position+new Vector3(1,1,1);
				transform.LookAt(target2);
			}
			else{
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            transform.position = rotation * new Vector3(horizontalValue, verticalValue, -distance) + target.position;

            transform.rotation = rotation;
				//onClick();
			}
        }

        string str;
        str = transform.position.x + "," + transform.position.y + "," + transform.position.z + "|||" + transform.rotation.x + "," + transform.rotation.y + "," + transform.rotation.z + "," + transform.rotation.w;
        //Debug.Log(str);
	   
    }
	
	static float ClampAngle ( float  angle,  float min,  float  max ) {
	    if (angle < -360)
		    angle += 360;
	    if (angle > 360)
		    angle -= 360;
	    return Mathf.Clamp (angle, min, max);
    }
	
	void OnGUI()
	{
		//GUI.skin = New_skin2;
       // GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backMap);
		
//		if(GUI.RepeatButton(new Rect(10,20,64,64),"左移"))   
//		{
//			horizontalValue += dt*cameraShiftHorizon;
//		}
		
		//GUI.skin = New_skin3;
		
//		if(GUI.RepeatButton(new Rect(10,100,64,64),"右移"))   
//		{
//			horizontalValue -= dt*cameraShiftHorizon;
//		}
		
//		if(GUI.RepeatButton(new Rect(10,180,64,64),"上移"))  
//		{
//			verticalValue -= dt*cameraShiftVertical;
//		}
		
//		if(GUI.RepeatButton(new Rect(10,260,64,64),"下移"))  
//		{
//			verticalValue += dt*cameraShiftVertical;
//		}

//		if(GUI.RepeatButton(new Rect(10,340,70,70),"开始旋转"))   
//		{
//			autoRotate = true;
//		}
//		if(GUI.RepeatButton(new Rect(10,420,70,70),"停止旋转"))   
//		{
//			autoRotate = false;
//		}
//		#region
//		if(GUI.Button(new Rect(1200,100,70,70),"Earth"))
//		{
//			//MoveCameraTo(target1);
//			//transform.LookAt(target1);
////			topos = target1.position;
////			transform.position = Vector3.Lerp(transform.position, topos, Time.deltaTime * springiness);
//			isEarth = !isEarth;
//		}
//		if(GUI.Button(new Rect(1200,200,70,70),"Mercury"))
//		{
//			isMercury=!isMercury;
//		}
//		#endregion
//		if (GUI.RepeatButton (new Rect (1400, 10, 70, 70), "next scene")) {
//			//Application.LoadLevel("");
//		}
	}

    void MoveCamera0()
    {

        topos = new Vector3(27f, 5f, 39.33f);
        torot = new Quaternion(0.0077f, -0.9548f, 0.0048f, 0.2960f);
        frompos = transform.position;
        fromrot = transform.rotation;
        movecamerabegintime = Time.time;
        movecamera = true;
	}

	void MoveCameraTo(Transform t){
		frompos = transform.position;
		fromrot = transform.rotation;
		topos = t.position;
		dt = Time.deltaTime;
		bool movecamra = true;
		if (movecamera) {
			float ratio;
			ratio = (Time.time - movecamerabegintime) / movelasttime;
			if (ratio > 1)
				ratio = 1f;
			transform.position = Vector3.Lerp (transform.position, topos, Time.deltaTime * springiness);
			transform.rotation = Quaternion.Lerp (fromrot, torot, ratio);

			x = transform.eulerAngles.y;
			y = transform.eulerAngles.x;
			distance = (transform.position - t.position).magnitude;

			if (Time.time - movecamerabegintime > movelasttime + 0.5f) 
			{
				x = transform.eulerAngles.y;
				y = transform.eulerAngles.x;
				distance = (transform.position - t.position).magnitude;
				horizontalValue = 0f;
				verticalValue = 0f;
				
				transform.position = topos;
				//transform.rotation = torot;
				movecamera = false;
				isenabled = true;

			}
		}
	}

	public void stopRotate(){
		autoRotate = false;
	}
	public void Rotate(){
		autoRotate = true;
	}	

	//UIEventTrigger


	//}
}

