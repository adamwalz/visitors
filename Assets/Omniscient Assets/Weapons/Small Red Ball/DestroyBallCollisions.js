//var scriptName : GameObject;
function Update()
{
	renderer.material.color = Color.red;
}

function OnCollisionEnter(collision : Collision)
{
    if (collision.gameObject.name == "Cube")
    {
    	Destroy(collision.collider.gameObject);	
    }

}