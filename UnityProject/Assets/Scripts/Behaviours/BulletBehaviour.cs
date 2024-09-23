using UnityEngine;

public class BulletBehaviour : MonoBehaviour {
    
    private float speed = 1.5f;
    private Vector3 direction;
    private PropsEnum shooter;
    private Vector3 shotPosition;

    private void OnEnable() {
        shotPosition = transform.position;
    }

    private void Update() {
        //Calculate distance with origin position, if too far away, destroy
        if (Vector3.Distance(transform.position, shotPosition) >= Constants.MAX_BULLET_TRAVEL_DISTANCE) {
            gameObject.SetActive(false);
        }
    }
    
    private void OnCollisionEnter(Collision other) {
        //If a fighter shoots and a enemy is hit (the rest of object cant get damage from allies)
        if ((PropsEnum.BasicFighter.Equals(shooter) || PropsEnum.ImprovedFighter.Equals(shooter))
                && other.gameObject.tag.Equals("Enemy")) {
            other.gameObject.GetComponent<PropStats>().ReduceHealthPoints(20);
        //If enemy shoots and a prop with healthPoints is hit (not an enemy)
        } else if ((PropsEnum.BasicEnemy.Equals(shooter) || PropsEnum.ImprovedEnemy.Equals(shooter))
                && !other.gameObject.tag.Equals("Enemy") && !other.gameObject.tag.Equals("Ore") && !other.gameObject.tag.Equals("Obstacle")){
            other.gameObject.GetComponent<PropStats>().ReduceHealthPoints(20);
        }
        //Deactivate bullet
        gameObject.SetActive(false);
    }
}