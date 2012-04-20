var speed:int =1;
var xSpeed = 2.0;

var rotation = Vector3(0,0,0);

function Update() {
  
  	if (Input.GetMouseButton(0))
  	{
  		rotation = Vector3(0, -50, 0);
		transform.Rotate(rotation * Time.deltaTime*xSpeed);
	}
	
	if (Input.GetMouseButton(1))
	{
		rotation = Vector3(0, 50, 0);
		transform.Rotate(rotation * Time.deltaTime*xSpeed);
	}
}