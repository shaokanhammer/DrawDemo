using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour
{
    public GlobalController Contr;
    Vector3 lastPoint;
    float minDistance = 1;

    void OnActive()
    {
        GlobalController.Points.Clear();
        Contr = GameObject.FindGameObjectWithTag("!!!").GetComponent<GlobalController>();
        GetComponent<TrailRenderer>().enabled = false;
    }

    void Update()
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0) *0.1f;        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastPoint = transform.position;
            GetComponent<TrailRenderer>().enabled = true;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Vector3.Distance(lastPoint, transform.position) > minDistance)
            {
                GlobalController.Points.Add(transform.position);
                lastPoint = transform.position;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            float cc = Contr.Compare(Contr.PointsToBool(), Contr.memory);
            GetComponent<TrailRenderer>().enabled = false;
            GlobalController.Points.Clear();
            if (cc > 0.14f)
            {
                Contr.PlusScore();
            }
        }
    }
}
