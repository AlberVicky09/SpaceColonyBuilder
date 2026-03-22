using UnityEngine;

public class BulletBehaviour : MonoBehaviour {
    
    private float speed = 1.5f;
    private Vector3 direction;
    private PropsEnum shooter;
    private GameObject shooterGO, objectiveGO;

    public void SetShooter(PropsEnum shooter, GameObject shooterGO, GameObject objectiveGO) {
        this.shooter = shooter;
        this.shooterGO = shooterGO;
        this.objectiveGO = objectiveGO;
    }

    private void Update() {
        //Calculate distance with origin position, if too far away, destroy
        if (shooterGO == null || Vector3.Distance(transform.position, shooterGO.transform.position) >= Constants.MAX_BULLET_TRAVEL_DISTANCE) {
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        //Avoid collision on shooter itself
        if (shooterGO.Equals(other.gameObject)) { return; }
        
        //If a fighter shoots and a enemy is hit (the rest of object cant get damage from allies)
        if (other.gameObject.Equals(objectiveGO)) {
            var damageQuantity = PropsEnum.Fighter.Equals(shooter) ? 15 : 10;
            other.gameObject.GetComponent<PropStats>().ReduceHealthPoints(damageQuantity);
            //Deactivate bullet
            gameObject.SetActive(false);
        }
    }
}