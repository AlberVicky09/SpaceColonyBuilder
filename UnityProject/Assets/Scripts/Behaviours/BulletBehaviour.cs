using UnityEngine;

public class BulletBehaviour : MonoBehaviour {
    
    private float speed = 1.5f;
    private Vector3 direction;
    private PropsEnum shooter;
    private GameObject shooterGO;

    public void SetShooter(PropsEnum shooter, GameObject shooterGO) {
        this.shooter = shooter;
        this.shooterGO = shooterGO;
    }

    private void Update() {
        //Calculate distance with origin position, if too far away, destroy
        if (Vector3.Distance(transform.position, shooterGO.transform.position) >= Constants.MAX_BULLET_TRAVEL_DISTANCE) {
            Debug.Log("Bullet destroyed due to too much distance");
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        //Avoid collision on shooter itself
        if (shooterGO.Equals(other.gameObject)) { return; }
        
        //If a fighter shoots and a enemy is hit (the rest of object cant get damage from allies)
        if ((PropsEnum.BasicFighter.Equals(shooter) && other.gameObject.tag.Equals("Enemy")) 
            //Or if a enemy shoots and anything hittable is hit
            || (PropsEnum.BasicEnemy.Equals(shooter)
                && other.gameObject.tag.Equals("MainBuilding") && !other.gameObject.tag.Equals("Fighter"))){
            other.gameObject.GetComponent<PropStats>().ReduceHealthPoints(20);
        }
        
        //Deactivate bullet
        gameObject.SetActive(false);
    }
}