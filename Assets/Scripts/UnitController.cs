using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class UnitController : MonoBehaviour
{
    public bool isSelected = false;
    private Chess chess;
    public void Select()
    {
        isSelected = true;
        Debug.Log("Unit Selected: " + gameObject.name);
        // TODO: 播放选中动画
    }

    public void Deselect()
    {
        isSelected = false;
        // TODO: 清除选中状态显示
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chess"))
        {
            if (!other.CompareTag("Chess") || other.gameObject == gameObject) return;
            if (GetInstanceID() > other.GetInstanceID()) return;
            Chess self = GetComponent<Chess>();
            Chess enemy = other.GetComponent<Chess>();
            if (self.number > enemy.number)
            {
                self.number -= enemy.number;
                Destroy(enemy.gameObject);
            }
            else if(self.number < enemy.number)
            {
                enemy.number -= self.number;
                Destroy(self.gameObject);
            }
            else
            {
                Destroy(self.gameObject);
                Destroy(enemy.gameObject);

            }
        }
    }
}
