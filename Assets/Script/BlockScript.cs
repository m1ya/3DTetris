using UnityEngine;
using System.Collections;

public class BlockScript : MonoBehaviour {

    //床や壁、ブロックに接触したかの判定
    bool rightflag;
    bool leftflag;
    bool forwardflag;
    bool backflag;

    //消えた際の処理
    bool Down;

    //座標を既に書き換えたかどうかの判定
    bool positionflag;
    bool posflag;

    //ブロックの座標を保管する。
    Vector3 position;

    //親オブジェクトを取得する変数。
    GameObject Parent;

    //親オブジェクトのMoveScriptを取得する。
    MoveScript ms;

    // Use this for initialization
    void Start () {

        //接触フラグの初期化
        rightflag = false;
        leftflag = false;
        forwardflag = false;
        backflag = false;

        //消える際の処理
        Down = false;

        //座標を書き換えたかどうかの判定の初期化
        positionflag = false;
        posflag = false;

        //親オブジェクト、スクリプトを取得する。
        Parent = transform.root.gameObject;
        ms = Parent.GetComponent<MoveScript>();

        //ブロックの座標を格納する。
        position = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        if (posflag == false)
        {
            ms.posflag++;
            posflag = true;
        }

        //親が止まっていたら自分も止まる
        if (ms.stopflag == true && positionflag == false)
        {
            //自分の座標の配列の中身を書き換える。
            GameManagerScript.field[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z)] = 1;
            positionflag = true;
            ms.posflag--;
        }

        //格納されている座標と現在の座標が違ったら
        if (position != transform.position)
            {
                //一時的に座標を格納
                Vector3 TemporaryPosition;
                TemporaryPosition = transform.position;

                //フィールドの外にブロックがいたら
                if (TemporaryPosition.x <= 0 || TemporaryPosition.x >= 11 || TemporaryPosition.z <= 0 || TemporaryPosition.z >= 11 || TemporaryPosition.y >= 21)
                {
                    ms.outflag = true;
                }

                //現在の座標を格納する。
                position = transform.position;
            }

            //一つ左のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[Mathf.RoundToInt(position.x) - 1, Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z)] >= 1)
            {
            if (leftflag == false)
            {
                //親オブジェクトの左移動をストップ
                ms.leftflag++;
                leftflag = true;
            }
             }else if(leftflag == true)
        {
            ms.leftflag--;
            leftflag = false;
        }

            //一つ右のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[Mathf.RoundToInt(position.x) + 1, Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z)] >= 1)
            {
            if (rightflag == false)
            {
                //親オブジェクトの右移動をストップ
                ms.rightflag++;
                rightflag = true;
            }
            }else if(rightflag == true)
        {
            ms.rightflag--;
            rightflag = false;
        }

            //一つ前のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z) - 1] >= 1)
            {
            if (backflag == false)
            {
                //親オブジェクトの前移動をストップ
                ms.backflag++;
                backflag = true;
            }
            }else if(backflag == true)
        {
            ms.backflag--;
            backflag = false;
        }

            //一つ後のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z) + 1] >= 1)
            {
            if (forwardflag == false)
            {
                //親オブジェクトの後移動をストップ
                ms.forwardflag++;
                forwardflag = true;
            }
            }else if(forwardflag == true)
        {
            ms.forwardflag--;
            forwardflag = false;
        }

            //一つ下のマスに床又はブロックがあった場合
            if (GameManagerScript.field[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y) - 1, Mathf.RoundToInt(position.z)] >= 1)
            {
                //親オブジェクトの落下をストップ
                ms.downflag = true;

                //1秒後に操作できないようにする
                Invoke("Stop", 1f);
            }
    }

    void Stop()
    {
        if (GameManagerScript.field[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y) - 1, Mathf.RoundToInt(position.z)] >= 1)
        {
            //操作不能にする
            ms.stopflag = true;
            //Debug.Log("Under is " + position.x + "," + (position.y - 1) + "," + position.z + " " + GameManagerScript.field[(int)position.x, (int)position.y - 1, (int)position.z]);

            //自分の座標の配列の中身を書き換える。
            if (positionflag == false)
            {
                GameManagerScript.field[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z)] = 1;
                positionflag = true;
                ms.posflag--;
            }
        }
        else
        {
            ms.timer = 1;
            ms.downflag = false;
            ms.timekeeper = 0.9f;
        }
    }

    void Position()
    {
        if (Down == true)
        {
            //列消滅時に一列さげる
            transform.position += Vector3.down;
            Down = false;
        }
     }

    void Flag()
    {
        Down = true;
    }


}
