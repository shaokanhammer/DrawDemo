using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GlobalController : MonoBehaviour
{
    public Text textField;
    public GameObject starter;
    public GameObject mouse;
    public static List<Vector3> Points = new List<Vector3>();
    public Texture2D[] images;
    public bool[,] memory = new bool[20, 20];

    int num;
    float timeMax = 20;
    float time;
    int score = 0;

    void OnEnable()
    {
        NewTask();
    }

    void Update()
    {
        for (int i = 1; i < Points.Count; i++)
        {
            Debug.DrawLine(Points[i - 1], Points[i], Color.green);
        }

        time -= Time.deltaTime;
        textField.text = "Time left - " + time + "\nScore - " + score;
        if (time < 0)
        {
            textField.text = "Time over!!!" + "\nScore - " + score;
            starter.SetActive(true);
            timeMax = 20;
            score = 0;
            starter.GetComponentInChildren<Text>().text = "Начать заново";
            gameObject.SetActive(false);
            mouse.SetActive(false);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 20), "Time left - " + time + "\nScore - " + score);
    }

    void NewTask()
    {
        Debug.Log("New Task!");
        do
        {
            num = Random.Range(0, images.Length);
        } while (images[num] == null);
        Debug.Log("Task №" + num + " initialization...");
        InitImages(num);
        renderer.material.SetTexture(0, images[num]);
        time = timeMax;
    }
    void InitImages(int im)
    {
        bool[,] map = new bool[80, 80];
        #region Image cheking
        float yStep = images[im].height/ 80.0f;
        float xStep = images[im].width / 80.0f;
        int pCount = 0;
        for (int i = 0; i < images[im].width; i++)
            for (int j = 0; j < images[im].height; j++)
            {
                map[Mathf.FloorToInt(i / xStep), Mathf.FloorToInt(j / yStep)] = false;
                Color pix = images[im].GetPixel(i, j);
                if (pix.a > 0.2f) //if colored
                {
                    #region neiber
                    for (int a = -1; a <= 1; a++)
                        for (int b = -1; b <= 1; b++)
                            try
                            {
                                map[Mathf.FloorToInt(i / 16) + a, Mathf.FloorToInt(j / 16) + b] = true;                         
                            }
                            catch
                            {
                            }                 
                    #endregion
                    pCount++;
                }
            }
        Debug.Log(pCount + " pixels found || " + pCount / (images[im].height * images[im].width * 1.0f) * 100 + "%");
        #endregion
        #region Building vectors
        int counter = 0;
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                if (map[i, j])
                {
                    counter++;
                }
        Debug.Log(counter + " points found || " + counter / 1600.0f * 100 + "%");
        Debug.Log("Building vectors...");
        Vector2[] vect = new Vector2[counter];
        counter = 0;
        string vects = "";
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                if (map[i, j])
                {
                    vect[counter] = new Vector2(i, j);
                    counter++;
                    vects += vect[counter-1].x + " " + vect[counter-1].y + "\n";
                }
        Debug.Log(vects);
        Debug.Log(vect.Length + " vectors constructed");
        #endregion
        Normalize(ref vect);
        foreach (Vector2 pos in vect)
        {
            try
            {
                int x = Mathf.Max(0, Mathf.FloorToInt(pos.x * 20) - 1);
                int y = Mathf.Max(0, Mathf.FloorToInt(pos.y * 20) - 1);
                memory[x,y] = true;
            }
            catch (System.IndexOutOfRangeException)
            {               
            }
        }
    }

    public float Compare(byte[,] drawed, bool[,] mem)
    {
        float cor = 0, inc = 0, bas = 0;
        for (int i = 0; i < 20; i++)
            for (int j = 0; j < 20; j++)
            {
                if (!mem[i, j])
                    bas++;
                if ((drawed[i, j] > 0) && mem[i, j])
                {
                    cor++;
                    Debug.Log("+");
                }
                else
                    if ((drawed[i, j] > 0 && !mem[i, j]) || (drawed[i, j] == 0 && mem[i, j]))
                    {
                        inc++;
                        Debug.Log("-");
                    }
            }
        float rez = ((float)(cor)) / ((float)(cor + inc));
        Debug.Log("Compare result: Cor-" + cor + " Inc-" + inc + " Rez =" + rez+ "BASE-" + bas);
        if (inc*1.5f > bas )
        {
            return -1;
        }
        return rez;
    }

    void Normalize(ref Vector2[] XX)
    {
        Debug.Log("Normalizing 2d");
        float xmin = XX[0].x, ymin = XX[0].y, xmax = XX[0].x, ymax = XX[0].y, norm;
        for (int i = 0; i < XX.Length; i++)
        {
            xmin = Mathf.Min(xmin, XX[i].x);
            ymin = Mathf.Min(ymin, XX[i].y);
            xmax = Mathf.Max(xmax, XX[i].x);
            ymax = Mathf.Max(ymax, XX[i].y);
        }
        norm = Mathf.Max(xmax - xmin, ymax - ymin);
        string vects = "Normalized vector: \n";
        for (int i = 0; i < XX.Length; i++)
        {
            XX[i] = new Vector2(Mathf.Floor((XX[i].x - xmin) / norm * 20) / 20.0f, Mathf.Floor((XX[i].y - ymin) / norm * 20) / 20.0f);
            vects += XX[i].x + " " + XX[i].y + "\n";
        }
        Debug.Log(vects);

    }
    void Normalize(ref Vector3[] XX)
    {
        float xmin = XX[0].x, ymin = XX[0].y, xmax = XX[0].x, ymax = XX[0].y, norm;
        for (int i = 0; i < XX.Length; i++)
        {
            xmin = Mathf.Min(xmin, XX[i].x);
            ymin = Mathf.Min(ymin, XX[i].y);
            xmax = Mathf.Max(xmax, XX[i].x);
            ymax = Mathf.Max(ymax, XX[i].y);
        }
        norm = Mathf.Max(xmax - xmin, ymax - ymin);
        Debug.Log("Vectors norm: Xmin-" + xmin + "Ymin-" + ymin + "norm-" + norm );
        string vects = "Normalized vector: \n";
        for (int i = 0; i < XX.Length; i++)
        {
            XX[i] = new Vector3(Mathf.Floor((XX[i].x - xmin) / norm * 20) / 20.0f, Mathf.Floor((XX[i].y - ymin) / norm * 20) / 20.0f, 0);
            vects += XX[i].x + " " + XX[i].y + "\n";
        }
        Debug.Log(vects);
    }

    public byte[,] PointsToBool()
    {
        byte[,] mat = new byte[20,20];
        Debug.Log("Inpit converting...");
        Vector3[] inpt = Points.ToArray();
        Normalize(ref inpt);
        foreach (Vector3 pos in inpt)
        {
            int x = Mathf.Max(0, Mathf.Min(20, Mathf.FloorToInt(pos.x * 20 - 1))),
                y = Mathf.Max(0, Mathf.Min(20, Mathf.FloorToInt(pos.y * 20 - 1)));
            mat[x, y] = 2;
            for (int a = -1; a <= 1; a++)
                for (int b = -1; b <= 1; b++)
                {
                    try
                    {
                        if (mat[x + a, y + b] != 2)
                            mat[x + a, y + b] = 1;
                    }
                    catch
                    {
                    }
                }
        }
        return mat;
    }
    public void PlusScore()
    {
        timeMax -= Mathf.Pow(timeMax, 1/3.0f);
        time = timeMax;
        score++;
        NewTask();
    }
}
