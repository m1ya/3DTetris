using UnityEngine;
using System.Collections;

public class BlockScript : MonoBehaviour {

    //ブロックの座標を保管する。
    Vector3 position;

    //親オブジェクトを取得する変数。
    GameObject Parent;

    //親オブジェクトのMoveScriptを取得する。
    MoveScript ms;

    // Use this for initialization
    void Start () {

        //親オブジェクト、スクリプトを取得する。
        Parent = transform.root.gameObject;
        ms = Parent.GetComponent<MoveScript>();

        //ブロックの座標を格納する。
        position = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

            //格納されている座標と現在の座標が違ったら
            if (position != transform.position)
            {
                //一時的に座標を格納
                Vector3 TemporaryPosition;
                TemporaryPosition = transform.position;

                //フィールドの外にブロックがいたら
                if (TemporaryPosition.x <= 0 || TemporaryPosition.x >= 11 || TemporaryPosition.z <= 0 || TemporaryPosition.z >= 11)
                {
                     ms.outflag = true;
                }
                
                //現在の座標を格納する。
                position = transform.position;
            }

            //一つ左のマスに壁又はブロックがあった場合
            if(GameManagerScript.field[(int)position.x - 1, (int)position.y, (int)position.z] >= 1)
            {
                //親オブジェクトの左移動をストップ
                ms.leftflag = true;
            }

            //一つ右のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[(int)position.x + 1, (int)position.y, (int)position.z] >= 1)
            {
                //親オブジェクトの右移動をストップ
                ms.rightflag = true;
            }

            //一つ前のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[(int)position.x, (int)position.y, (int)position.z - 1] >= 1)
            {
                //親オブジェクトの前移動をストップ
                ms.backflag = true;
            }

            //一つ後のマスに壁又はブロックがあった場合
            if (GameManagerScript.field[(int)position.x, (int)position.y, (int)position.z + 1] >= 1)
            {
                //親オブジェクトの後移動をストップ
                ms.forwardflag = true;
            }

            //一つ下のマスに床又はブロックがあった場合
            if (GameManagerScript.field[(int)position.x, (int)position.y - 1, (int)position.z] >= 1)
            {
                //親オブジェクトの落下をストップ
                ms.downflag = true;

                //1秒後に操作できないようにする
                Invoke("Stop", 1f);
            }
    }

    void Stop()
    {
        if (GameManagerScript.field[(int)position.x, (int)position.y - 1, (int)position.z] >= 1)
        {
            //操作不能にする
            ms.stopflag = true;

            //自分の座標の配列の中身を書き換える。
            GameManagerScript.field[(int)position.x, (int)position.y, (int)position.z] = 1;
        }
        else
        {
            ms.downflag = false;
            ms.timekeeper = 0.9f;
        }
    }

}
