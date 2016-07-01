using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //ヘルプの管理
    public GameObject Help;
    public static bool HelpStop;

    //壁を2、止まっているブロックを1として配列で座標を管理する
    public static int[,,] field;
    int LineChecker;

    //Scoreの管理
    public static int Score;
    public Text ScoreText;
    public Text CombText;
    int SameTimeVanish;
    int Comb;
    bool CombFlag;
    bool vanishflag;

    //GameOverの処理領域
    public GameObject GameoverCanvas;
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
        Help.SetActive(false);
        GameoverCanvas.SetActive(false);
        HelpStop = false;

        Score = 0;
        gameoverflag = false;
        LineChecker = 0;
        vanish = false;
        SameTimeVanish = 0;
        CombFlag = false;
        vanishflag = false;

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

        //Scoreの表示
        ScoreText.text = "SCORE : " + Score.ToString();
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
                        vanishflag = true;
                        SameTimeVanish++;
                        Down(i,j,0);
                        if(CombFlag == false)
                        {
                            Comb++;
                            CombFlag = true;
                        }
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
                        vanishflag = true;
                        SameTimeVanish++;
                        Down(0, j, i);
                        if (CombFlag == false)
                        {
                            Comb++;
                            CombFlag = true;
                        }
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
        CombFlag = false;
        
        if(vanishflag == false)
        {
            CombText.text = "";
            Comb = 0;
        }else
        {
            CombText.text = Comb.ToString() + " Comb";
            vanishflag = false;
        }

        //Scoreの計算
        Score += 100 * SameTimeVanish * SameTimeVanish;
        Score += 100 * Comb * Comb;
        SameTimeVanish = 0;

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
            GameoverCanvas.SetActive(true);
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
                        GameoverCanvas.SetActive(true);
                        gameoverflag = true;
                        bgmSource.Stop();
                        gameoverSource.Play();
                    }
                }
            }
        }
    }

    void Tweet()
    {
        string tweet = "私のスコアは" + Score + "でした！ https://unityroom.com/games/3dtetris #3DTetris_Rinaya";
        Application.ExternalEval(string.Format("window.open('{0}','_blank')", "http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(tweet)));
    }

    void HelpButton()
    {
        Help.SetActive(true);
        HelpStop = true;
    }

    void HelpBack()
    {
        Help.SetActive(false);
        HelpStop = false;
    }
}
