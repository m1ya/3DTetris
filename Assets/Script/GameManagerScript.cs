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

    //列削除用オブジェクト
    public GameObject DeleteCube;
    public GameObject DownCube;

    //列削除用のメモ領域
    public static int[,,] downmemo;
    public static int[,,] memorise;
    bool deleteflag;

    //BGM
    public AudioSource bgmSource;
    public AudioSource vanishSource;

    // Use this for initialization
    void Start()
    {

        field = new int[12, 22, 12];
        memorise = new int[12, 22, 12];
        downmemo = new int[12, 22, 12];

        deleteflag = false;

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
                        deleteflag = true;
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
                        deleteflag = true;
                    }
                }
            }
        }

        if (deleteflag == true)
        {
            StepDelete();
            deleteflag = false;
        }
    }

    void StepDelete()
    {
        //列を消す
        for (int i = 0; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 0; k < (field.GetLength(0) - 1); k++)
                {
                    if (memorise[i, j, k] == 1)
                    {
                        Delete(i, j, k);
                        //vanishSource.Play();
                    }
                }
            }
        }

        //ブロックを下げる
        for (int i = 0; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 0; k < (field.GetLength(0) - 1); k++)
                {
                    if (memorise[i, j, k] == 1)
                    {
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
                field[i, y, z] = 0;
            }

            if (z == 0)
            {
                Instantiate(DeleteCube, new Vector3(x, y, i), transform.rotation);
                field[x, y, i] = 0;
            }
        }
    }

    //消えたら一つ下げる処理
    void Down(int x, int y, int z)
    {
        Debug.Log(x + "," + y + "," + z);
        for (int i = 1; i < 11; i++)
        {
            for (int j = y+1; j < 21; j++)
            {
                 if (x == 0)
                 {
                    if (j < 20)
                    {
                        field[i, j - 1, z] = field[i, j, z];
                    }else if(j == 20)
                    {
                        field[i, j, z] = 0;
                    }

                    Instantiate(DownCube, new Vector3(i, j, z), transform.rotation);
                    
                  }

                 if (z == 0)
                 {
                    if (j < 20)
                    {
                        field[x, j - 1, i] = field[x, j, i];
                    }else if(j == 20)
                    {
                        field[x, j, i] = 0;
                    }

                    Instantiate(DownCube, new Vector3(x, j, i), transform.rotation);
                    
                 }
            }
        }
    }

}