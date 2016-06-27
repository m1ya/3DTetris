using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
   //壁を2、止まっているブロックを1として配列で座標を管理する
    public static int[,,] field;
    int LineChecker;

    //GameOverの処理領域
    public Text GameOverText;
    public static bool gameoverflag;

    //列表示用オブジェクト
    public GameObject LineCube;
    GameObject[,,] cube;

    //列削除用領域
    public int[,,] memorise;
    bool vanish;

    //BGM
    public AudioSource bgmSource;
    public AudioSource vanishSource;
    public AudioSource gameoverSource;

    // Use this for initialization
    void Start()
    {
        gameoverflag = false;
        LineChecker = 0;
        vanish = false;

        field = new int[12, 22, 12];
        memorise = new int[12, 22, 12];
        cube = new GameObject[12, 22, 12];

        //cubeへGameObjectの代入
        for (int i = 1; i < field.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 1; k < field.GetLength(2) - 1; k++)
                {
                    cube[i, j, k] = (GameObject)Instantiate(LineCube, new Vector3(i, j, k), transform.rotation);
                }
            }
        }

        //x軸の左右の壁を作る
        for (int i = 0; i < field.GetLength(1); i++)
        {
            for (int j = 0; j < field.GetLength(2); j++)
            {
                field[0, i, j] = 2;
                field[11, i, j] = 2;
            }
        }

        //z軸の前後の壁を作る
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                field[i, j, 0] = 2;
                field[i, j, 11] = 2;
            }
        }

        //y軸の床、天井を作る
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(2); j++)
            {
                field[i, 0, j] = 2;
                field[i, 21, j] = 2;
            }
        }

    }

    void Update()
    {
        Draw();

        if(gameoverflag == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Main");
            }
        }
    }

    //cubeを描画する
    void Draw()
    {
        for(int i = 1; i < field.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 1; k < field.GetLength(2) - 1; k++)
                {
                    if(field[i,j,k] == 1)
                    {
                        cube[i, j, k].SetActive(true);
                    }else
                    {
                        cube[i, j, k].SetActive(false);
                    }
                }
            }
        }
    }

    //列がそろっているかの判定
    void LineCheck()
    {
        //x軸に列がそろっているかの判定
        for (int i = 1; i < (field.GetLength(0) - 1); i++)
        {

            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 1; k < (field.GetLength(2) - 1); k++)
                {
                    if (field[i, j, k] == 0)
                    {
                        LineChecker = 0;
                        break;
                    }

                    LineChecker += field[i, j, k];

                    if (LineChecker == 10)
                    {
                        Down(i,j,0);
                        if(vanish == false)
                        {
                            vanishSource.Play();
                            vanish = true;
                        }
                        j--;
                        LineChecker = 0;
                    }
                }
            }
        }

        //z軸に列がそろっているかの判定
        for (int i = 1; i < (field.GetLength(2) - 1); i++)
        {
            for (int j = 1; j < field.GetLength(1) - 1; j++)
            {
                for (int k = 1; k < (field.GetLength(0) - 1); k++)
                {
                    if (field[k, j, i] == 0)
                    {
                        LineChecker = 0;
                        break;
                    }

                    LineChecker += field[k, j, i];

                    if (LineChecker == 10)
                    {
                        Down(0, j, i);
                        if (vanish == false)
                        {
                            vanishSource.Play();
                            vanish = true;
                        }
                        j--;
                        LineChecker = 0;
                    }
                }
            }
        }
        vanish = false;
        //ゲームオーバーしていないかの確認
        Gameover();
    }

    //消えたら一つ下げる処理
    void Down(int x, int y, int z)
    {
        for (int i = 1; i < 11; i++)
        {
            for (int j = y; j < 21; j++)
            {
                 if (x == 0)
                 {
                    if (j < 20)
                    {
                        field[i, j, z] = field[i, j + 1, z];
                    }else if(j == 20)
                    {
                        field[i, j, z] = 0;
                    }                    
                  }

                 if (z == 0)
                 {
                    if (j < 20)
                    {
                        field[x, j, i] = field[x, j + 1, i];
                    }else if(j == 20)
                    {
                        field[x, j, i] = 0;
                    }
                 }
            }
        }
    }

    //ゲームオーバーの判定
    void Gameover()
    {
        if (field[5, 17, 5] == 1)
        {
            GameOverText.text = "GameOver Retry to put Space";
            gameoverflag = true;
            bgmSource.Stop();
            gameoverSource.Play();

        }

        for (int i = 4; i <= 6; i++)
        {
            for (int j = 18; j <= 20; j++)
            {
                for (int k = 4; k <= 6; k++)
                {
                    if (field[i, j, k] == 1)
                    {
                        GameOverText.text = "GameOver Retry to put Space";
                        gameoverflag = true;
                        bgmSource.Stop();
                        gameoverSource.Play();
                    }
                }
            }
        }
    }
}
