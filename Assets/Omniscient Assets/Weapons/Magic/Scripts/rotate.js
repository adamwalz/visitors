var rotationspeed : float = 30;

function Update()
{
	
	var rotationVector : Vector3 = Vector3(Random.Range(-10.0, 10.0), Random.Range(-10.0, 10.0), Random.Range(-10.0, 10.0));
	transform.Rotate(rotationVector * Time.deltaTime * rotationspeed);
}
