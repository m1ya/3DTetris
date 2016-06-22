using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

    //壁を2、止まっているブロックを1として配列で座標を管理する
    public static int[,,] field;
    int LineChecker = 0;
    public GameObject DeleteCube;

	// Use this for initialization
	void Start () {

        field = new int[12,21,12];

        for (int i = 0; i < field.GetLength(1); i++)
        {
            for (int j = 0; j < field.GetLength(2); j++)
            {
                //x軸の左右の壁を作る
                field[0, i, j] = 2;
                field[11, i, j] = 2;
            }
        }

        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                //z軸の前後の壁を作る
                field[i, j, 0] = 2;
                field[i, j, 11] = 2;
            }
        }

        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(2); j++)
            {
                //y軸の床を作る
                field[i, 0, j] = 2;
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //列がそろっているかの判定
    void LineCheck()
    {
        Debug.Log("Start LineCheck!");

        //x軸に列がそろっているかの判定
        for (int i = 1; i < (field.GetLength(0) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1); j++)
            {
                LineChecker = 0;

                for (int k = 1; k < ( field.GetLength(2) - 1); k++)
                {
                    if(field[i,j,k] == 0)
                    {
                        break;
                    }

                    LineChecker += field[i, j, k];
                    if(LineChecker == 10)
                    {
                        Delete(i,j,0);
                    }
                }
            }
        }

        //z軸に列がそろっているかの判定
        for (int i = 1; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1); j++)
            {
                LineChecker = 0;

                for (int k = 1; k < (field.GetLength(0) - 1); k++)
                {
                    LineChecker += field[k, j, i];
                    if (LineChecker == 10)
                    {
                        Delete(0,j,i);
                    }
                }
            }
        }

    }

    //列がそろったら消える処理
    void Delete(int x, int y, int z)
    {
        //xが0ならxの座標1~10を消す、zが0ならzの座標1~10を消す。
        for (int i = 1; i < 11; i++)
        {
            if (x == 0)
            {
                Instantiate(DeleteCube, new Vector3(i, y, z), transform.rotation);
            }

            if (z == 0)
            {
                Instantiate(DeleteCube, new Vector3(x, y, i), transform.rotation);
            }
        }

        //上の段のブロックを一列ずらす。

    }
}
