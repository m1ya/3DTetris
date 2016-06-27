using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

    //床や壁、ブロックに接触したかの判定
    public bool downflag;
    public int rightflag;
    public int leftflag;
    public int forwardflag;
    public int backflag;

    //フィールド外にいたら立てる
    public bool outflag;

    //機能停止のフラグ
    public bool stopflag;

    //座標を書き換えたか否かの判定
    public int posflag = 0;

    //ブロック落下の時間・速度管理
    public float timekeeper;
    public float timer;

    //床に着いたときにブロックを生成したかどうかの判定
    bool geneflag = false;

    //GameManagerを取得する
    GameObject gamemanager;

    // Use this for initialization
    void Start () {

        //接触フラグの初期化
        downflag = false;
        rightflag = 0;
        leftflag = 0;
        forwardflag = 0;
        backflag = 0;

        //機能停止フラグの初期化
        stopflag = false;

        //落下速度の初期化
        timer = 1;

        //フィールド外フラグの初期化
        outflag = false;

        //GameManagerを取得する
        gamemanager = GameObject.Find("GameManager");
    }
	
	// Update is called once per frame
	void Update () {

        //床orブロックの上に接触していなければ
        if (downflag == false)
        {
            //Blockの落下処理
            timekeeper += Time.deltaTime;
            if (timekeeper >= timer)
            {
                transform.position += Vector3.down;
                timekeeper = 0;
            }
        }

        if(stopflag == false) {

            //Blockの操作処理

            if (Input.GetKeyDown(KeyCode.D))
            {
                timer = 0.05f;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //左側に何もなければ
                if (leftflag == 0)
                {
                    transform.position += Vector3.left;
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                //右側に何もなければ
                if (rightflag == 0)
                {
                    transform.position += Vector3.right;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //前側に何もなければ
                if (forwardflag == 0)
                {
                    transform.position += Vector3.forward;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //後側に何もなければ
                if (backflag == 0)
                {
                    transform.position += Vector3.back;
                }
            }

            //ブロックの回転処理

            if (Input.GetKeyDown(KeyCode.Z))
            {
                transform.eulerAngles += new Vector3(0, 90, 0);
                Invoke("reZ", 0.001f);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                transform.eulerAngles += new Vector3(90, 0, 0);
                Invoke("reX", 0.001f);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                transform.eulerAngles += new Vector3(0, 0, 90);
                Invoke("reY", 0.001f);
            }

        }
        else if(geneflag == false)
        {
            if (posflag == 0)
            {
                //揃っている列がないかチェックする
                gamemanager.SendMessage("LineCheck");

                //ゲームオーバーしていなければ
                if (GameManagerScript.gameoverflag == false)
                {
                    //次のブロックを生成する
                    geneflag = true;
                    BlockGeneratorScript.geneflag = true;
                }

                //オブジェクトを消す
                Destroy(this.gameObject);
            }
        }
    }

    void reX()
    {
        //もしフィールド外に出たら
        if (outflag == true)
        {
            transform.eulerAngles -= new Vector3(90, 0, 0);
            outflag = false;
        }
    }

    void reZ()
    {
        //もしフィールド外に出たら
        if (outflag == true)
        {
            transform.eulerAngles -= new Vector3(0, 90, 0);
            outflag = false;
        }
    }

    void reY()
    {
        //もしフィールド外に出たら
        if (outflag == true)
        {
            transform.eulerAngles -= new Vector3(0, 0, 90);
            outflag = false;
        }
    }
}
