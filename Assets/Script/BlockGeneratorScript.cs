using UnityEngine;
using System.Collections;

public class BlockGeneratorScript : MonoBehaviour {

    //ブロックの取得
    public GameObject[] Block;

    //一度に二個生まれないように
    public static bool geneflag;

    //ブロックをランダムに生成するための変数
    int random;

    // Use this for initialization
    void Start () {
        geneflag = true;
    }

    // Update is called once per frame
    void Update() {
        if (geneflag == true)
        {
            //生成したので次に生成しない
            geneflag = false;

            //ランダムにブロックを生成する
            Instantiate(Block[1], transform.position, transform.rotation);
            /*Random.Range(0, Block.Length)*/
        }
    }
    }

