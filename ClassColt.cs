using UnityEngine;
using TMPro;

public class Colt : MonoBehaviour
{
    //Parametrs
    public float damage = 5f;
    public float range = 100f;
    public float fireRate = 7f;
    public float impactForce = 30f;
    public float spread;
    private bool isShooting = true;
    //Effects
    public Camera fpscamera;
    public ParticleSystem muzzleflash;
    public GameObject impactEffect;
    public GameObject impactEnemyEffect;
    private Animator _animator;
    private float nextTimeToFire = 0f;
    //Ammo
    public int currentAmmo = 8;
    public int allAmmo = 0;
    public int fullAmmo = 24;
    public TextMeshProUGUI bullets1;
    //Audio
    private AudioSource _audioSourse;
    public AudioClip ammoSound;
    public AudioClip shotSound;
    public AudioClip reloadSound;
    public AudioClip zatvorSound;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSourse = GetComponent<AudioSource>();
        _audioSourse.clip = zatvorSound;
    }

    void Update()
    {
        bullets1.text = currentAmmo + "|" + allAmmo;
    }

    public void Shoot()
    {
        if (Time.time >= nextTimeToFire && currentAmmo >= 1 && gameObject.activeInHierarchy && isShooting)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            muzzleflash.Play();
            _animator.SetTrigger("isShoot");
            _audioSourse.PlayOneShot(shotSound);
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            //Calculate Direction with Spread
            Vector3 direction = fpscamera.transform.forward + new Vector3(x, y, 0);
            RaycastHit hit;
            if (Physics.Raycast(fpscamera.transform.position, direction, out hit, range))
            {
                EnemyTakeDamage target = hit.transform.GetComponent<EnemyTakeDamage>();
                //didn't hit the enemy
                if (target != null)
                {
                    target.TakeDamage(damage);
                    GameObject impactEE = Instantiate(impactEnemyEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactEE, 0.1f);
                }
                //hit the enemy
                if (target == null)                                     
                {
                    GameObject impactE = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactE, 2f);
                }
                //hit force
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
            }
            currentAmmo -= 1;
        }
    }

    public void Reload()
    {
        if (gameObject.activeInHierarchy)
        {
            if (allAmmo > 0 && currentAmmo < 8)
            {
                _animator.SetTrigger("isReload");
                isShooting = false;
                int reason = 8 - currentAmmo;
                if (allAmmo >= reason)
                {
                    allAmmo = allAmmo - reason;
                    currentAmmo = 8;
                }
                else
                {
                    currentAmmo = currentAmmo + allAmmo;
                    allAmmo = 0;
                }
                _audioSourse.PlayOneShot(reloadSound);
                _audioSourse.PlayDelayed(1.8f);
                Invoke("ReloadFinish", 2.1f);
            }
        }
    }

    private void ReloadFinish()
    {
        isShooting = true;
    }
    //Magazin 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ColtAmmo") && allAmmo < fullAmmo)
        {
            int result = 24 - allAmmo;
            if (result >= 8)
            {
                allAmmo += 8;
                Destroy(other.gameObject);
                _audioSourse.PlayOneShot(ammoSound);
            }
            else
            {
                allAmmo += result;
                Destroy(other.gameObject);
                _audioSourse.PlayOneShot(ammoSound);
            }
        }
    }
}
