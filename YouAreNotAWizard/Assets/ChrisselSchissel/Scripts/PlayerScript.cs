using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerScript : MonoBehaviour
{
    //Collision Detection
        //Shock
    public WaveAttackCollision waveRealOne;
    private WaveAttackCollision waveClone;
    [SerializeField]
    private GameObject waveHitBox;
        //Fire
    public FireAttackCollision fireRealOne;
    private FireAttackCollision fireClone;
    [SerializeField]
    private GameObject fireHitBox;


    //VisualEffects
    public VisualEffect flamespell;
    public VisualEffect wavespell;

    //Raycast Variablen
    Camera cam; //Unsere Spielkamera
    [SerializeField]
    private float maxDistance = 1000f; //Raycastl�nge
    [SerializeField]
    private LayerMask layerMask; //GroundLayer, damit nur RayCasts auf dem Boden gemacht werden

    //Timer Variablen
    public float flameSpawnTimer;
    private float currentTimer = 0f;

    void Start()
    {
        cam = Camera.main;
        flamespell.Stop();
        wavespell.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        FlameHitBox();
        WaveHitBox();

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
            //F�r das Normalisieren ist es wichtig, dass nur x und z Achse beachtet wird
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
        }
        if (Input.GetKey(KeyCode.Mouse0)){
            if (FlameHitBoxTimer())
            {
                fireClone = Instantiate(fireRealOne, transform.GetChild(0).position + direction * 2, directionAngle);
                fireClone.setMovementDirection(direction);
            }
            flamespell.transform.position = transform.GetChild(0).position + direction * 2;
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
        bool spawn = FlameHitBoxTimer();

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Vector3.up;
        Quaternion directionAngle = Quaternion.LookRotation(direction);

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            //Richtungsvektor
            direction = (hit.point - transform.position);
            //F�r das Normalisieren ist es wichtig, dass nur x und z Achse beachtet wird
            direction = new Vector3(direction.x, 0.0f, direction.z);
            //direction wird normalisiert
            direction = direction.normalized;
            //Richtung der Attacke als Winkel
            directionAngle = Quaternion.LookRotation(direction);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //VFX Attacke starten
            wavespell.Play();
            waveClone = Instantiate(waveRealOne, transform.GetChild(0).position + direction * 2, directionAngle);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            
            wavespell.transform.position = transform.GetChild(0).position + direction * 2;
            wavespell.transform.rotation = directionAngle;

            waveClone.transform.rotation = directionAngle;
            waveClone.transform.position = transform.GetChild(0).position + direction  * 0.6f * waveHitBox.transform.localScale.z;

        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            wavespell.Stop();
            waveClone.DestroyWaveSpell();
        }
    }

}
