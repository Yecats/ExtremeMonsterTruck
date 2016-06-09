using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public GameObject MonsterTruck;

    void LateUpdate () {

        if (GameManager.Instance.ShouldCameraFollowCar())
        {
            this.transform.position = MonsterTruck.transform.position - MonsterTruck.transform.forward * 5 + Vector3.up * 3;
            this.transform.LookAt(MonsterTruck.transform.position + Vector3.up * 2);
        }
	}
}
