using UnityEngine;
using System.Collections;

public class BlockGeneratorScript : MonoBehaviour {

    //ブロックの取得
    public GameObject Block1;
    public GameObject Block2;
    public GameObject Block3;
    public GameObject Block4;
    public GameObject Block5;
    public GameObject Block6;
    public GameObject Block7;

    //一度に二個生まれないように
    public static bool geneflag = true;

    //ブロックをランダムに生成するための変数
    int random;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        if (geneflag == true)
        {
            //生成したので次に生成しない
            geneflag = false;

            //ランダムにブロックを生成する
            random = Random.Range(1, 7);
            switch (random)
            {
                case 1:
                    Instantiate(Block1, transform.position, transform.rotation);
                    break;
                case 2:
                    Instantiate(Block2, transform.position, transform.rotation);
                    break;
                case 3:
                    Instantiate(Block3, transform.position, transform.rotation);
                    break;
                case 4:
                    Instantiate(Block4, transform.position, transform.rotation);
                    break;
                case 5:
                    Instantiate(Block5, transform.position, transform.rotation);
                    break;
                case 6:
                    Instantiate(Block6, transform.position, transform.rotation);
                    break;
                case 7:
                    Instantiate(Block7, transform.position, transform.rotation);
                    break;
            }
        }
    }

}
