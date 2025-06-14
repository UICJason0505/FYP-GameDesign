using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{
    // Start is called before the first frame update
    public int blood = 1;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chess")) TakeDamage();
    }
    public void TakeDamage()
    {
        blood--;

        if (blood <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.GameOver("King is dead! Game Over.");
        Destroy(gameObject);
    }
}
