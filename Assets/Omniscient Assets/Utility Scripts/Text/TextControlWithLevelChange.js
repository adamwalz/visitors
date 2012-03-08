function OnMouseEnter()
{
	renderer.material.color = Color.green;
}

function OnMouseExit()
{
	renderer.material.color = Color.white;
}

function OnMouseUp()
{
	Application.LoadLevel("start Scene");
}