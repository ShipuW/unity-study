using UnityEngine;
using System.Collections;

public enum BreakoutGameState { playing, win, lost };//定义游戏状态.

public class BreakoutGame : MonoBehaviour
{
	//音乐文件.
	public AudioSource music;
	//音量.
	public float musicVolume;

    public static BreakoutGame SP;
	public Transform ballPrefab;//珠球.
	private int totalBlocks;//总的方块数.
	private int blocksHit;//已经击打的方块数.
    private BreakoutGameState gameState;//游戏状态.
	
	public float ZoomSpeed = 10;
	public float MovingSpeed = 0.5f;//移动速度.
	public float RotateSpeed = 1;//旋转速度.
	public float distance = 5;

	void Start(){
		//设置默认音量.
		musicVolume = 0.5f;
	}
    void Awake()
    {
        SP = this;
        blocksHit = 0;
        gameState = BreakoutGameState.playing;
        totalBlocks = GameObject.FindGameObjectsWithTag("Pickup").Length;//得到所有的方块数.
        Time.timeScale = 1.0f;//设置传递时间为1 .
		//SpawnBall();
    }

	void Update(){
		Quaternion rotation = Quaternion.identity;
		Vector3 position;
		float delta_x, delta_y, delta_z;
		float delta_rotation_x, delta_rotation_y;
		
		//按下鼠标左键.
		if(Input.GetMouseButton(0)){
			delta_x = Input.GetAxis("Mouse X") * MovingSpeed;//获取x轴方向的鼠标运动增量,乘以相应的移动速度.
			delta_y = Input.GetAxis("Mouse Y") * MovingSpeed;//获取y轴方向的鼠标运动增量,乘以相应的移动速度.
			//rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);//设置旋转的角度，存储在Quaternion中.
			rotation.eulerAngles = new Vector3(0,transform.rotation.eulerAngles.y,0);
			transform.position = rotation * new Vector3(-delta_x,0,-delta_y)+transform.position;
			//Debug.Log(rotation);
		}
		//按下鼠标右键.
		if(Input.GetMouseButton(1)){
			delta_rotation_x = Input.GetAxis("Mouse X") * RotateSpeed;
			delta_rotation_y = Input.GetAxis("Mouse Y") * RotateSpeed;
			position = transform.rotation*new Vector3(0,0,distance)+transform.position;
			transform.Rotate(0,delta_rotation_x,0,Space.World);
			transform.Rotate(delta_rotation_y,0,0);
			transform.position = transform.rotation * new Vector3(0,0,-distance)+position;
		}
		//滑动鼠标滚动条.
		if(Input.GetAxis("Mouse ScrollWheel")!=0){
			delta_z = -Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
			transform.Translate(0,0,-delta_z);
			distance += delta_z;
		}
	
	} 
	/// <summary>
	/// 初始化珠球
	/// </summary>
    void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(10f, 1.8f , 5f), Quaternion.identity);//实例化珠球.
    }
	/// <summary>
	/// 界面的渲染	
	/// </summary>
    void OnGUI(){
		//GUILayout.Label("作者：朱俊璋、王士溥");
        GUILayout.Space(10);//添加空格.
        GUI.Label(new Rect(10,120,80,40),"己击打: " + blocksHit + "/" + totalBlocks);


		if (GUI.Button (new Rect (10, 300, 80, 40), "开关音乐")) {
			//没有播放中.
			if(!music.isPlaying){
				music.Play();
			}else{
				music.Pause();
			}		
		}

 		//游戏开始.

		if (GUI.Button(new Rect(10,250,80,40),"开始游戏")) {
			SpawnBall();

		}

		//if (GUI.Button(new Rect(100,200,80,30),"游戏暂停")) {
		//	Time.timeScale = 0.0f;	
		//}
        if (gameState == BreakoutGameState.lost)
        {
            GUI.Label(new Rect(10,150,80,40),"你输了！");
            if (GUI.Button(new Rect(10,200,80,40),"重新加载"))
            {
				Application.LoadLevel(Application.loadedLevel);//重新加载关卡.
            }
        }
        else if (gameState == BreakoutGameState.win)
        {
			GUI.Label(new Rect(10,150,80,40),"你赢了！");
			if (GUI.Button(new Rect(10,200,80,40),"重新加载"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }
	/// <summary>
	/// 击打砖块
	/// </summary>
    public void HitBlock()
    {
        blocksHit++;
        if (blocksHit%10 == 0) //每击打十个砖块生成新的珠球.
        {
            //SpawnBall();
        }
        if (blocksHit >= totalBlocks)//游戏胜利.
        {
            WinGame();
        }
    }
	/// <summary>
	/// 珠球掉落
	/// </summary>
	public void LostBall()
	{
		int ballsLeft = GameObject.FindGameObjectsWithTag("Player").Length;//获得剩余珠球数.
		if(ballsLeft<=1){
			SetGameOver();//游戏结束.
		}
	}
	/// <summary>
	/// 游戏胜利
	/// </summary>
    public void WinGame()
    {
        Time.timeScale = 0.0f; //设置游戏暂停.
        gameState = BreakoutGameState.win;
    }
	/// <summary>
	/// 游戏失败 
	/// </summary>
    public void SetGameOver()
    {
        Time.timeScale = 0.0f; //设置游戏暂停.
        gameState = BreakoutGameState.lost;
    }
}
