//var scriptName : GameObject;


function OnCollisionEnter(collision : Collision)
{
    if (collision.gameObject.name == "Temple1")
    {
    	collision.collider.gameObject.GetComponent(RedColor).SendMessage("ChangeColor");	
    }

}