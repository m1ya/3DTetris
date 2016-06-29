using UnityEngine;
using System.Collections;

public class BlockGeneratorScript : MonoBehaviour {

    //ブロックの取得
    public GameObject[] Block;

    //一度に二個生まれないように
    public static bool geneflag;

    //ブロックをランダムに生成するための
    bool nextblock;
    int random;
    GameObject[] cube;

    // Use this for initialization
    void Start () {
        cube = new GameObject[7];
        geneflag = true;
        random = Random.Range(0, Block.Length);
        nextblock = false;

        cube[0] = GameObject.Find("Block1");
        cube[1] = GameObject.Find("Block2");
        cube[2] = GameObject.Find("Block3");
        cube[3] = GameObject.Find("Block4");
        cube[4] = GameObject.Find("Block5");
        cube[5] = GameObject.Find("Block6");
        cube[6] = GameObject.Find("Block7");

        for(int i = 0; i < 7; i++)
        {
            cube[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update() {
        if (geneflag == true)
        {
            //生成したので次に生成しない
            geneflag = false;
            nextblock = false;

            //ランダムにブロックを生成する
            Instantiate(Block[random], transform.position, transform.rotation);
        }

        if(nextblock == false)
        {
            cube[random].SetActive(false);
            random = Random.Range(0, Block.Length);
            cube[random].SetActive(true);
            nextblock = true;
        }
    }
}

