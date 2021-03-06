using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public CharacterController CC;
    //Collision Detection
        //Shock
    public WaveAttackCollision waveRealOne;
    private WaveAttackCollision waveClone;
    [SerializeField]
    private GameObject waveHitBox;
    private bool waveUse = false;

        //Fire
    public FireAttackCollision fireRealOne;
    private FireAttackCollision fireClone;
    [SerializeField]
    //private GameObject fireHitBox;
    private bool fireUse = false;

    [SerializeField]
    private float attackHeight = 0.8f;

    //#####-Mana-######
    public float mana = 100f;
    public float manaPerSecond = 2.0f;
    private bool manaAvailable = true;

    //#####-Life-######
    public int life = 100;
    public int lifePerSecond = 1;
    bool notFullLife = false;
    int lifeRegCounter = 0;


    //#####-Death-######
    public GameObject Canvas1;
    public GameObject Canvas2;
    float deathTimer = 0f;
    public bool isDead = false;

    //#####-Slider-######
    public Slider manaSlider;
    public Slider lifeSlider;

    //#####-AudioManager-######
    private PlayerAudioScript audioManager;



    //VisualEffects
    public VisualEffect flamespell;
    public VisualEffect wavespell;

    //Raycast Variablen
    Camera cam; //Unsere Spielkamera
    [SerializeField]
    private float maxDistance = 1000f; //Raycastlänge
    [SerializeField]
    private LayerMask layerMask; //GroundLayer, damit nur RayCasts auf dem Boden gemacht werden

    //Timer Variablen
    public float flameSpawnTimer;
    private float currentTimer = 0f;

    void Start()
    {
        audioManager = GetComponent<PlayerAudioScript>();
        cam = Camera.main;
        flamespell.Stop();
        wavespell.Stop();
    }

    void manaBar()
    {
        int manaInt = (int)mana;
        manaSlider.value = manaInt;
    }
    void lifeBar()
    {
        lifeSlider.value = life;
    }
    public void TakeDamage(int damage)
    {
        Debug.Log("jetzt");
        life -= damage;
        notFullLife = true;
    }


    public int GetLife()
    {
        return life;
    }

    public void CheckDeath()
    {
        
        if (life <= 0)
        {
            isDead = true;
            CC.enabled = false;
            Canvas1.SetActive(false);
            Canvas2.SetActive(false);
            deathTimer += Time.deltaTime;
            int seconds = (int)deathTimer % 60;
            if (seconds >= 5)
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
              
            }
            Time.timeScale = 1f;
            
        }
    }


    void regenerate()
    {
        if(life < 100 && life >=1)
        {
            life += lifePerSecond;
        }
        else
        {
            notFullLife = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (manaAvailable && !isDead)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !waveUse && Time.timeScale > 0.5f)
            {
                FlameHitBox();
                audioManager.PlayFireSound();

            }
            else if (Input.GetKeyUp(KeyCode.Mouse0) && fireUse)
            {
                FlameHitBox();
                audioManager.StopFireSound();
                fireUse = false;
            }
            else if (Input.GetKey(KeyCode.Mouse1) && !fireUse && Time.timeScale > 0.5f)
            {
                WaveHitBox();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1) && waveUse)
            {
                WaveHitBox();
                
                waveUse = false;
            }
            if (!fireUse)
            {
                audioManager.StopFireSound();
            }
            
        }
        else
        {
            audioManager.StopFireSound();
            fireUse = false;
            waveUse = false;
            flamespell.Stop();
            wavespell.Stop();
            if(waveClone != null)
            {
                waveClone.DestroyWaveSpell();
            }
            
        }
        

        manaBar();
        lifeBar();
        CheckDeath();
        

    }



    void FixedUpdate()
    {
        if(waveUse || fireUse)
        {
            mana -= manaPerSecond;
            if(mana <= 0)
            {
                manaAvailable = false;
            }
        }
        else if (mana <= 100)
        {
            mana += (manaPerSecond * 0.6f);
            if(mana > 20.0f)
            {
                manaAvailable = true;
            }
        }
        if (notFullLife)
        {
            lifeRegCounter++;
            if(lifeRegCounter%25 == 0)
            {
                regenerate();
                lifeRegCounter = 0;
            }

        }
    }


    void FlameHitBox()
    {
        bool spawn = FlameHitBoxTimer();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Vector3.up;
        Quaternion directionAngle = Quaternion.LookRotation(direction);

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            //Richtungsvektor
            direction = (hit.point - transform.position);
            //Für das Normalisieren ist es wichtig, dass nur x und z Achse beachtet wird
            direction = new Vector3(direction.x, 0.0f, direction.z);
            //direction wird normalisiert
            direction = direction.normalized;
            //Richtung der Attacke als Winkel
            directionAngle = Quaternion.LookRotation(direction);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {         
            //VFX Attacke starten
            flamespell.Play();
            fireUse = true;
        }
        if (Input.GetKey(KeyCode.Mouse0)){
            if (FlameHitBoxTimer())
            {
                //fireClone = Instantiate(fireRealOne, transform.GetChild(0).position + new Vector3(0.0f, attackHeight, 0.0f) + direction * 2, directionAngle);
                fireClone = Instantiate(fireRealOne, transform.GetChild(0).position + new Vector3(0.0f, attackHeight, 0.0f) + direction * 2, directionAngle);

                fireClone.setMovementDirection(direction);
            }
            flamespell.transform.position = transform.GetChild(0).position + new Vector3(0.0f, attackHeight, 0.0f) + direction * 2;
            flamespell.transform.rotation = directionAngle;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            flamespell.Stop();
        }
        
    }
    bool FlameHitBoxTimer()
    {
        currentTimer += Time.deltaTime;
        if (currentTimer > flameSpawnTimer)
        {
            currentTimer = 0f;
            return true;
        }
        else
        {
            return false;
        }
        
    }

    void WaveHitBox()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Vector3.up;
        Quaternion directionAngle = Quaternion.LookRotation(direction);

        Vector3 transWeapon = GameObject.Find("/Player/Weapon").transform.position;

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            //Richtungsvektor
            direction = (hit.point - transform.position);
            //Für das Normalisieren ist es wichtig, dass nur x und z Achse beachtet wird
            direction = new Vector3(direction.x, 0.0f, direction.z);
            //direction wird normalisiert
            direction = direction.normalized;
            //Richtung der Attacke als Winkel
            directionAngle = Quaternion.LookRotation(direction);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //VFX Attacke starten
            waveClone = Instantiate(waveRealOne, transWeapon + direction * 0.6f * waveHitBox.transform.localScale.z, directionAngle);
            waveUse = true;
            wavespell.Play();
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            
            wavespell.transform.position = transWeapon + new Vector3(0.0f, attackHeight, 0.0f)  + direction * 2;
            wavespell.transform.rotation = directionAngle;
            if(waveClone != null)
            {
                waveClone.transform.position = transWeapon + new Vector3(0.0f, attackHeight, 0.0f) + direction * 0.6f * waveHitBox.transform.localScale.z;
                waveClone.transform.rotation = directionAngle;
            }
            
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            wavespell.Stop();
            waveClone.DestroyWaveSpell();
        }
    }

}
