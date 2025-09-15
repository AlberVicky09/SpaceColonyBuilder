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
            gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        //Avoid collision on shooter itself
        if (shooterGO.Equals(other.gameObject)) { return; }
        
        //If a fighter shoots and a enemy is hit (the rest of object cant get damage from allies)
        switch (shooter) {
            case PropsEnum.Fighter:
                if (other.gameObject.name.StartsWith("EnemyFighter") || other.gameObject.name.StartsWith("EnemyBase")) {
                    other.gameObject.GetComponent<PropStats>().ReduceHealthPoints(15);
                }
                break;
            case PropsEnum.EnemyFighter:
                if (other.gameObject.name.StartsWith("PlayerFighter") || other.gameObject.name.StartsWith("PlayerBase")) {
                    other.gameObject.GetComponent<PropStats>().ReduceHealthPoints(10);
                }
                break;
        }
        
        //Deactivate bullet
        gameObject.SetActive(false);
    }
}