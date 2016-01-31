using UnityEngine;
using System.Collections;

public class RemoveKnockout : MonoBehaviour {

	void RemoveKO()
    {
        transform.parent.GetComponent<Controls>().knockedOut = false;
    }
}
