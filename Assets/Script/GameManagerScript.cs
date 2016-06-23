using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{

    //Debug用
    public Text text1;
    public Text text2;
    public Text text3;

    //壁を2、止まっているブロックを1として配列で座標を管理する
    public static int[,,] field;
    int LineChecker = 0;
    public GameObject DeleteCube;
    public GameObject DownCube;

    //列削除用のメモ領域
    public static int[,,] memorise;

    // Use this for initialization
    void Start()
    {

        field = new int[12, 22, 12];
        memorise = new int[12, 22, 12];

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
                //y軸の床、天井を作る
                field[i, 0, j] = 2;
                field[i, 21, j] = 2;
            }
        }

    }

    void Update()
    {
        text1.text = field[1, 1, 10].ToString() + " " + field[2, 1, 10].ToString() + " " + field[3, 1, 10].ToString() + " " + field[4, 1, 10].ToString() + " " + field[5, 1, 10].ToString() + " " + field[6, 1, 10].ToString() + " " + field[7, 1, 10].ToString() + " " + field[8, 1, 10].ToString() + " " + field[9, 1, 10].ToString() + " " + field[10, 1, 10].ToString();
        text2.text = field[1, 2, 10].ToString() + " " + field[2, 2, 10].ToString() + " " + field[3, 2, 10].ToString() + " " + field[4, 2, 10].ToString() + " " + field[5, 2, 10].ToString() + " " + field[6, 2, 10].ToString() + " " + field[7, 2, 10].ToString() + " " + field[8, 2, 10].ToString() + " " + field[9, 2, 10].ToString() + " " + field[10, 2, 10].ToString();
        text3.text = field[1, 3, 10].ToString() + " " + field[2, 3, 10].ToString() + " " + field[3, 3, 10].ToString() + " " + field[4, 3, 10].ToString() + " " + field[5, 3, 10].ToString() + " " + field[6, 3, 10].ToString() + " " + field[7, 3, 10].ToString() + " " + field[8, 3, 10].ToString() + " " + field[9, 3, 10].ToString() + " " + field[10, 3, 10].ToString();
    }

    //列がそろっているかの判定
    void LineCheck()
    {
        //x軸に列がそろっているかの判定
        for (int i = 1; i < (field.GetLength(0) - 1); i++)
        {

            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                LineChecker = 0;

                for (int k = 1; k < (field.GetLength(2) - 1); k++)
                {

                    if (field[i, j, k] == 0)
                    {
                        break;
                    }

                    LineChecker += field[i, j, k];
                    if (LineChecker == 10)
                    {                 
                        memorise[i, j, 0] = 1;
                        Debug.Log("memorise " + i + "," + j + "," + "0 " + memorise[i, j, 0]);
                    }
                }
            }
        }

        //z軸に列がそろっているかの判定
        for (int i = 1; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                LineChecker = 0;

                for (int k = 1; k < (field.GetLength(0) - 1); k++)
                {
                    if (field[k, j, i] == 0)
                    {
                        break;
                    }

                    LineChecker += field[k, j, i];
                    if (LineChecker == 10)
                    {
                        memorise[0, j, i] = 1;
                        Debug.Log("memorise " + "0" + "," + j + "," + i + " " + memorise[0, j, i]);
                    }
                }
            }
        }

        StepDelete();
    }

    void StepDelete()
    {
        for (int i = 1; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 1; k < (field.GetLength(0) - 1); k++)
                {
                    if (memorise[i, j, k] == 1)
                    {
                        Debug.Log("Delete");
                        Delete(i, j, k);
                    }
                }
            }
        }

        for (int i = 1; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 1; k < (field.GetLength(0) - 1); k++)
                {
                    if (memorise[i, j, k] == 1)
                    {
                        Debug.Log("Down");
                        Down(i, j, k);
                        memorise[i, j, k] = 0;
                    }
                }
            }
        }

    }

    //列がそろったら消える処理
    void Delete(int x, int y, int z)
    {
        //xが0ならxの座標1~10を消す、zが0ならzの座標1~10を消す。
        //消したブロックがあった座標の配列を0にする
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
    }

    //消えたら一つ下げる処理
    void Down(int x, int y, int z)
    {
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 21; j++)
            {
                if (j != y)
                {
                    if (x == 0)
                    {
                        Instantiate(DownCube, new Vector3(i, j, z), transform.rotation);
                    }

                    if (z == 0)
                    {
                        Instantiate(DownCube, new Vector3(x, j, i), transform.rotation);
                    }
                }
            }
        }
    }
}