// <copyright file="SpaceShip.cs" company="DIS Copenhagen">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Benno Lueders</author>
// <date>18/06/2018</date>

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Numerics;

/// <summary>
/// This script handles the spaceships functionality, including movement and shooting. The spaceship needs any Collider2D attached that has the trigger flag checked, so it can be hit
/// by the asteroids and die.
/// </summary>
public class SpaceShip : MonoBehaviour {

	[Tooltip("How fast is the spaceship moving left to right")]
    public float speed = 1;
	[Tooltip("How fast is the spaceship shooting")]
    public float rateOfFire = 1;
    [Tooltip("Prefab to be instantiated when shooting (Projectile)")]
    public GameObject projectilePrefab;
    [Tooltip("Prefab to be instantiated when dying (explosioin)")]
    public GameObject explosionPrefab;
    [Tooltip("An Audioclip that is played when the ship shoots")]
    public AudioClip laserSound;

    private float lastTimeFired = 0;
	private bool isDead = false;

    /// <summary>
    /// This is called by Unity every frame. It handles the ships movement and checks if it should fire
    /// </summary>
    void Update() {

		if(isDead) return;

        // move the ship left and right, depending on the horizontal input
        // math: Direction(right) * input * speed * 1/fps(deltaTime)
        transform.position += UnityEngine.Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        // if the fire button is pressed and we waited long enough since the last shot was fired, FIRE!
        if (Input.GetKey(KeyCode.Space) && (lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            FireTheLasers();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            Bomb();
        }
    }

    /// <summary>
    /// This is called by Unity when the object overlaps with another object. It is only called when the following conditions are true:
    /// 1. Both objects have any 2D Collider attached, at least them is a trigger.
    /// 2. At least of of the two colliding GameObjects has a Rigidbody2D attached.
    /// OnTriggerEnter2D is for overlap checking (Asteroid is overlapping with space ship) OnCollisionEnter2D is used for solid body collisions with newtonian force exhange.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other){

        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        if(other.GetComponent<Asteroid>() != null)
        {
            Instantiate(explosionPrefab, transform.position, UnityEngine.Quaternion.identity);
			// load the active scene again, to restard the game. The GameManager will handle this for us. We use a slight delay to see the explosion.
			StartCoroutine(RestartTheGameAfterSeconds(1));
			// we can not destroy the spaceship since it needs to run the coroutine to restart the game.
			// instead, disable update (isDead = true) and remove the renderer to "hide" the object while we reload.
			isDead = true;
			Destroy(GetComponent<SpriteRenderer>());
        }
    }

	/// <summary>
	/// Helper function to include the shooting behavior.
	/// </summary>
    void FireTheLasers(){
        AudioSource.PlayClipAtPoint(laserSound, transform.position);

		// Shooting up
		Instantiate(projectilePrefab, transform.position + UnityEngine.Vector3.up, UnityEngine.Quaternion.identity);

        GameObject laser2 = Instantiate<GameObject>(projectilePrefab, transform.position + UnityEngine.Vector3.up, UnityEngine.Quaternion.identity);
        Projectile projScript = laser2.GetComponent<Projectile>();
        projScript.direction = new UnityEngine.Vector3(-1, 1, 0); //left

        GameObject laser3 = Instantiate<GameObject>(projectilePrefab, transform.position + UnityEngine.Vector3.up, UnityEngine.Quaternion.identity);
        Projectile projScript2 = laser3.GetComponent<Projectile>();
        projScript2.direction = new UnityEngine.Vector3(1, 1, 0); //right


		// Instantiate(projectilePrefab, transform.position + Vector3.left, Quaternion.identity);
		// Instantiate(projectilePrefab, transform.position + Vector3.right, Quaternion.identity);


        // Implement more projectiles as a bonus objectve

        // Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        // Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        // Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);


        // for (int i = 0; i < 10; i++) {
        //     Vector3 randomDirection = new Vector3 (Random.Range(-2, 2), 1, 0) * speed * Time.deltaTime;
        //     Instantiate(projectilePrefab, transform.position + randomDirection, Quaternion.identity);
        // }

    }

	/// <summary>
	/// Kill all asteroids in the scene
	/// </summary>
	void Bomb(){
        // Implement as a bonus objective
        // Tip: You can find all asteroids for example by using: FindObjectsByType<Asteroid>()
        
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");

        // Asteroid[] asteroids = FindObjectsByType<Asteroid>();
        
        for (int i = 0; i < asteroids.Length; i++) {
            Asteroid script = asteroids[i].GetComponent<Asteroid>();
            script.OnHit();
        }

        // foreach(var asteroid in asteroids) {
        //     // asteroidComp = asteroid.GetComponent<Asteroid>();

        //     // asteroidComp.OnHit();

        //     Asteroid script = asteroid.GetComponent<Asteroid>();

        //     scirpt.OnHit();

        //     // OnHit(script);

        //     // asteroidScript

        //     // asteroid.GetComponent<Asteroid>().OnHit();
        // }
    }

	/// <summary>
	/// Wait seconds and reload current scene.
	/// </summary>
	IEnumerator RestartTheGameAfterSeconds(float seconds){
		yield return new WaitForSeconds (seconds);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
