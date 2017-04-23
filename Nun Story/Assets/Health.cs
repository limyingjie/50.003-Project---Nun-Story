using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComponentHealth : MonoBehaviour
{
    [SerializeField]
    protected int m_MaxHP = 10;

    protected int m_CurrentHP;

    // Getter
    public int CurrentHP { get { return m_CurrentHP; } }
    public int MaxHP { get { return m_MaxHP; } }
    public float FractionHP { get { return (float)(m_CurrentHP) / (float)(m_MaxHP); } } //returns currentHP as a fraction of maxHP

    void Start() //you start with full HP
    {
        m_CurrentHP = m_MaxHP;
    }

    public void Modify(int amount) //use this function when object receives damage/healing
    {
        m_CurrentHP += amount;

        if (m_CurrentHP > m_MaxHP) //you cant have more HP then your maxHP
        {
            m_CurrentHP = m_MaxHP;
        }
        else if (m_CurrentHP <= 0) //you die if your HP reaches 0
        {
            Die();
        }
    }

    public void Set(int amount) //sets current HP to amount specified
    {
        m_CurrentHP = amount;
        Modify(0); // run thru the checks in Modify()
    }

    public void Die() //when game object dies, remove it from play
    {
        //if (this.CompareTag("Enemy")) { GameObject.Find("EnemiesRemainingText").GetComponent<EnemyCounter>().enemyDown(); }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            //Modify(-other.GetComponent<ComponentBullet>().m_bulletDamage);
        } //take damage equal to bullets damage value

        else if (other.gameObject.CompareTag("Pickup"))
        {
            Modify(1);
            other.gameObject.SetActive(false);
        }
    }
}
