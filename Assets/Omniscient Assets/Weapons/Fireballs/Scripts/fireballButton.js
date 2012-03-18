/* 	(C) 2012 Unluck Productions
	http://www.chemicalbliss.com */

//Example script on how to add fireballs to scene

var spawn:Transform;		//Place fireball prefab
var verticalSpeed:float=-200;	

function OnMouseDown (){

var newBall:Transform = transform.Instantiate(spawn, Vector3(Random.Range(-4,4),10,Random.Range(3,13)),transform.rotation);
newBall.rigidbody.AddForce(Random.Range(-100, 100),verticalSpeed,Random.Range(-100, 100));
newBall.rigidbody.AddTorque(Random.Range(-25,25),Random.Range(-25,25),Random.Range(-25,25));
}