/*==============================================================================
            Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;

public class VirtualButtonEventHandler : MonoBehaviour,
                                         IVirtualButtonEventHandler
{

    public Transform spawn;
	public float verticalSpeed = -200;


    // Called when the virtual button has just been pressed:
    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {

    }


    // Called when the virtual button has just been released:
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        //Transform newBall = transform.Instantiate(spawn, Vector3(Random.Range(-4,4),10,Random.Range(3,13)),transform.rotation);
		//Transform newB = Instantiate(spawn, Vector3
		
		//newBall.rigidbody.AddForce(Random.Range(-100, 100),verticalSpeed,Random.Range(-100, 100));
		//newBall.rigidbody.AddTorque(Random.Range(-25,25),Random.Range(-25,25),Random.Range(-25,25));
    }
}
