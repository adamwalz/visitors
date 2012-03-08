var colCounter : int = 0;
function Update () {

	if (colCounter == 0)
	{
		renderer.material.color = Color.magenta;
	}
	
	else
	{
		renderer.material.color = Color.green;
	}
}

function ChangeColor(){
	colCounter++;
}