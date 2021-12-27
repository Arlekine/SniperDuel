using UnityEngine;

public class CarStopTrigger : MonoBehaviour
{
    [SerializeField] private MovingObject _movingObject;
    
    public void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Bullet>() == null)
            _movingObject.enabled = false;
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Bullet>() == null)
            _movingObject.enabled = true;
    }
}