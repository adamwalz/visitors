var thePrefab: GameObject;


function Update () {

	var cam : GameObject = GameObject.Find("ARCamera");
	if(Input.GetMouseButtonUp(0))
	{
		//Debug.Log(cam.transform);
		//var instance : GameObject = Network.Instantiate(thePrefab, cam.transform.position, cam.transform.rotation, 0);
		var instance : GameObject = Instantiate(thePrefab, transform.position, transform.rotation);

		var fwd = transform.forward * 50000;
		instance.rigidbody.AddForce(fwd);
		
		
	}
}