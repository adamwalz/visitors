/* 	(C) 2012 Unluck Productions
	http://www.chemicalbliss.com */

var fragments:Transform;									//Place the fractured fireball model
var fragmentParticleMinSizeModification:float = .75;		//Change the particle minimum size for fragments
var fragmentParticleMaxSizeModification:float = .75;		//Change the particle maximum size for fragments
var fragmentParticleMaxEmissionModification:float = 5.0;	//Change the particle maximum emission for fragments
var fragmentParticleMinEmissionModification:float = 5.0;	//Change the particle minimum emission for fragments
var fragmentParticleMaxEnergyModification:float = .5;		//Change the particle maximum energy for fragments
var fragmentParticleMinEnergyModification:float = .5;		//Change the particle minimum energy for fragments
var fragmentParticleSizeGrow:float = -0.125;				//Change the particle grow for fragments
var waitForRemoveCollider:float = 1;						//Delay before removing collider
var waitForDestroy:float = 2;								//Delay before removing fireball
var explosiveForce:float = 350;								//How much random force applied to each fragment
var showFractures:boolean = true;							//Show/Hide fragments
var maxFragmentsWithParticles:int = 20;						//How many of the fragments should have particle system attached
var selfExplode:float = 100;								//Delay before explode the fireball even if it does not collide with anything 
var inheritVelocity:boolean;								//Fragments will have the velocity of the fireball when added
var fractureDelay:float = .1;								//Delay before explosion
var physicMat:PhysicMaterial;								//Physical material attached to each fragment
private var counter:int;									//Counts how many particles attached to fragments
private var exploded:boolean;								//Determines if the fireball has exploded or not

function Start () {
yield(WaitForSeconds(selfExplode));
triggerExplosion();
}

function OnCollisionEnter(collision : Collision) {   
triggerExplosion();
}

function triggerExplosion(){
yield(WaitForSeconds(fractureDelay));
transform.Destroy(transform.collider);
if(transform.FindChild("fireball") != null)
  transform.Destroy(transform.FindChild("fireball").gameObject);
  transform.Destroy(transform.collider);
  explode();
  exploded=true;

}

function explode(){
if(!exploded){
var fragmentd:Transform = gameObject.Instantiate(fragments, transform.position, transform.rotation);
for (var child: Transform in fragmentd.FindChild("fragments")) {
    counter ++;
    child.gameObject.AddComponent(MeshCollider);
    child.gameObject.AddComponent(Rigidbody);
    if(physicMat!=null)
    child.gameObject.collider.material = physicMat;
     if(counter <= maxFragmentsWithParticles&&transform.FindChild("particles")!=null){
    var fP:Transform = new gameObject.Instantiate(transform.FindChild("particles"), child.transform.position, child.transform.rotation) as Transform;
    fP.particleEmitter.minSize =  fP.particleEmitter.minSize*fragmentParticleMinSizeModification;
    fP.particleEmitter.maxSize =  fP.particleEmitter.minSize*fragmentParticleMaxSizeModification;
    fP.particleEmitter.maxEmission = fP.particleEmitter.maxEmission*fragmentParticleMaxEmissionModification;
    fP.particleEmitter.minEmission = fP.particleEmitter.minEmission*fragmentParticleMinEmissionModification;
    fP.particleEmitter.maxEnergy = fP.particleEmitter.minEnergy *fragmentParticleMaxEnergyModification;
    fP.particleEmitter.minEnergy = fP.particleEmitter.minEnergy *fragmentParticleMinEnergyModification;
    fP.GetComponent(ParticleAnimator).sizeGrow = fragmentParticleSizeGrow;
    fP.parent = child;
    if(inheritVelocity){
    child.rigidbody.velocity = transform.rigidbody.velocity;
    }
    }
    child.rigidbody.AddForce(Random.Range(-explosiveForce, explosiveForce),Random.Range(-explosiveForce, explosiveForce),Random.Range(-explosiveForce, explosiveForce));
  	child.rigidbody.AddTorque(Random.Range(-explosiveForce, explosiveForce),Random.Range(-explosiveForce, explosiveForce),Random.Range(-explosiveForce, explosiveForce));
  
  if(!showFractures)
  child.renderer.enabled = false;
  }
  if(transform.FindChild("particles") != null)
  transform.FindChild("particles").particleEmitter.emit=false;
  yield(WaitForSeconds(waitForRemoveCollider));
  for (var child: Transform in fragmentd.FindChild("fragments")) {  
    child.gameObject.Destroy(child.GetComponent(MeshCollider));
  }
  yield(WaitForSeconds(waitForDestroy));
  if(transform.FindChild("Billboard GameObjects") != null)
  GameObject.Destroy(transform.FindChild("Billboard GameObjects").gameObject);
  GameObject.Destroy(fragmentd.gameObject);
  GameObject.Destroy(transform.gameObject);  
  }   
}