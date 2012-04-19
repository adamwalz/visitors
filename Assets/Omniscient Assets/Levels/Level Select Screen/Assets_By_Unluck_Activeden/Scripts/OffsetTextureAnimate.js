// Scroll main texture based on time
var scrollSpeedX = 0.015;
var scrollSpeedY = 0.015;
var scrollSpeedXMaterial2 = 0.015;
var scrollSpeedYMaterial2 = 0.015;
function Update () {
    var offsetX = Time.time * scrollSpeedX;
    var offsetY = Time.time * scrollSpeedY;
    var offset2X = Time.time * scrollSpeedXMaterial2;
    var offset2Y = Time.time * scrollSpeedYMaterial2;
    renderer.material.SetTextureOffset ("_BumpMap", Vector2(offsetX,offsetY));
    renderer.material.SetTextureOffset ("_MainTex", Vector2(offsetX,offsetY));
    renderer.materials[1].SetTextureOffset ("_MainTex", Vector2(offset2X,offset2Y));
    renderer.materials[1].SetTextureOffset ("_BumpMap", Vector2(offset2X,offset2Y));
}