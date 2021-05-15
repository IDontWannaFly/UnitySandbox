using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyDetector : MonoBehaviour
{
    public LayerMask enemyLayer;

    public Transform player;

    private EnemyDetectorCallback callback;

    public void attackCallback(EnemyDetectorCallback callback){
        this.callback = callback;
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"Object enter: {other.transform.name}");
        var hitResults = new RaycastHit2D[2];
        var hit = Physics2D.Raycast(
            transform.position, 
            new Vector3(
                other.transform.position.x, 
                other.transform.position.y + other.transform.lossyScale.y / 2) - transform.position, 
            new ContactFilter2D(){
                useLayerMask = true,
                layerMask = enemyLayer | LayerMask.GetMask("Ground")
            }, 
            hitResults, Vector2.Distance(transform.position, other.transform.position));
        Debug.Log($"Hit info: {string.Join(", ", hitResults.Select(r => r.transform != null ? r.transform.name : ""))}");
        if(hit > 0 && hit < 2){
            if((enemyLayer.value & 1<<other.gameObject.layer) != 0)
                if(callback != null)
                    callback.OnDetected(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Debug.Log($"Object exit: {other.gameObject.layer} : {enemyLayer.value}");
        if((enemyLayer.value & 1<<other.gameObject.layer) != 0){
            if(callback != null)
                callback.OnLost(other);
        }
    }

    private void OnDrawGizmosSelected() {
        if(player != null)
            Debug.DrawRay(transform.position, new Vector3(
                player.position.x, 
                player.position.y + player.lossyScale.y / 2) - transform.position, Color.green);
    }

    public interface EnemyDetectorCallback{
        void OnDetected(Collider2D collider);
        void OnLost(Collider2D collider);
    }
}
