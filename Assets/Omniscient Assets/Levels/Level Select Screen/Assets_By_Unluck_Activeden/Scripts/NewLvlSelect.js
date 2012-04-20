#pragma strict
var scene = "";
function Start () {

}

function Update () {

	if (Input.GetMouseButtonDown(0)) // check for left-mouse
	{
    	var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    	var hit : RaycastHit;
    	if (collider && collider.Raycast (ray, hit, 100.0))
    	{
    		if (collider.tag == "LevelLight")
    		{
    			Application.LoadLevel(scene);
     	  	}
    	}
}
}