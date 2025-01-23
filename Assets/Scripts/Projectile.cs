// <copyright file="Projectile.cs" company="DIS Copenhagen">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Benno Lueders</author>
// <date>07/14/2017</date>

using UnityEngine;
using System.Collections;

/// <summary>
/// Projectile handles the movement of the fired lasers. The script needs any 2D Collider (set to trigger) and a Rigidbody2D (set to kinematic) on the same GameObject.
/// The script moves the GameObject constantly upwards using speed. After the lifeTime the projectile is
/// </summary>
public class Projectile : MonoBehaviour
{

	[Tooltip ("How fast is the projectile moving upwards")]
	public float speed = 2;
	[Tooltip ("After how many seconds is the projectile destroyed")]
	public float lifeTime = 3;
	[Tooltip ("The direction the laster travels")]
	public Vector2 direction = new Vector2 (0, 1);

	/// <summary>
	/// This is called by Unity. It starts the coroutine that destroyes the projectile after the lifetime.
	/// </summary>
	void Start ()
	{
		// normalize direction so it does not impact the travel speed
		direction.Normalize ();
		// make the projectile rotate into the direction it is moving, math will be addressed in lecture 2
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	/// <summary>
	/// Update is called by Unity each frame. This moves the GameObject upwards at speed and kills the object after lifeTime expires
	/// </summary>
	void Update ()
	{
		transform.position += new Vector3 (direction.x, direction.y, 0) * speed * Time.deltaTime;
		
		lifeTime -= Time.deltaTime;
		if(lifeTime <= 0){
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// This is called by Unity when the object overlaps with another object. It is only called when the following conditions are true:
	/// 1. Both objects have any 2D Collider attached, at least them is a trigger.
	/// 2. At least of of the two colliding GameObjects has a Rigidbody2D attached.
	/// OnTriggerEnter2D is for overlap checking (Projectile overlapping with Asteroid) OnCollisionEnter2D is used for solid body collisions with newtonian force exhange.
	/// </summary>
	void OnTriggerEnter2D (Collider2D other)
	{
		Asteroid asteroid = other.GetComponent<Asteroid>(); // Only asteroid have the "Asteroid" script, if we hit one, we will find the script on it.
		if (asteroid != null) { // This checks if we hit an asteroid.
			asteroid.OnHit (); // notify the asteroid it got hit
			Destroy (gameObject); // Destory this projectile
		}
	}
}
