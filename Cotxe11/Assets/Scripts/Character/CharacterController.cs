﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    public float speed = 1.0f;
    public float jump_force = 200.0f;
    public float max_speed = 5.0f;
    public float climb_speed = 2.0f;
    public float climb_horizontal_impulse = 0.1f;


    private bool facing_right = true;
    public bool on_air = false;
    public bool can_move = true;
    public bool can_climb = false;
    private bool is_dead = false;

    public int current_jumps = 0;

    public AudioClip jump_audio = null;
    public AudioClip bounce_audio = null;
    public AudioClip land_audio = null;
    public AudioClip double_jump_audio = null;
    public AudioClip death_audio = null;

    private Rigidbody2D character_rb = null;
    private Vector2 default_gravity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        character_rb = GetComponent<Rigidbody2D>();
        default_gravity = Physics2D.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        //Inputs

        if (is_dead == true)
        {
            if (GetComponent<AudioSource>().isPlaying == false)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            if (Input.GetKey("d"))
            {
                if (facing_right == false)
                {
                    Flip();
                    facing_right = true;
                }
                if (can_move == true)
                {
                    Vector3 movement = new Vector3(1.0f, 0.0f, 0.0f);
                    transform.position += movement * speed * Time.deltaTime;
                }
            }

            if (Input.GetKey("a"))
            {
                if (facing_right == true)
                {
                    Flip();
                    facing_right = false;
                }
                if (can_move == true)
                {
                    Vector3 movement = new Vector3(-1.0f, 0.0f, 0.0f);
                    transform.position += movement * speed * Time.deltaTime;
                }
            }

            if (Input.GetButtonDown("Jump") && current_jumps < 2 && !can_climb)
            {
                on_air = true;
                character_rb.velocity = new Vector2(0.0f, 0.0f);
                character_rb.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);
                if (current_jumps == 0)
                {
                    GetComponent<AudioSource>().clip = jump_audio;
                    GetComponent<AudioSource>().Play();
                }
                else
                {
                    GetComponent<AudioSource>().clip = double_jump_audio;
                    GetComponent<AudioSource>().Play();
                }
                current_jumps++;
            }

            if (can_climb)
            {

                current_jumps = 1;
                on_air = true;

                if (Input.GetKey("w"))
                {
                    Vector3 movement = new Vector3(0.0f, 1.0f, 0.0f);
                    transform.position += (movement * climb_speed * Time.deltaTime);
                }

                if (Input.GetKey("s"))
                {
                    Vector3 movement = new Vector3(0.0f, -1.0f, 0.0f);
                    transform.position += movement * climb_speed * Time.deltaTime;
                }

                if ((Input.GetKeyDown("d") && facing_right) || (Input.GetKeyDown("a") && !facing_right))
                {

                    character_rb.velocity = new Vector2(0.0f, 0.0f);

                    if (facing_right)
                    {
                        character_rb.AddForce(new Vector2(-climb_horizontal_impulse, 1f) * jump_force, ForceMode2D.Impulse);
                        facing_right = false;
                        Flip();
                    }

                    else
                    {
                        character_rb.AddForce(new Vector2(climb_horizontal_impulse, 1f) * jump_force, ForceMode2D.Impulse);
                        facing_right = true;
                        Flip();
                    }
                }
            }
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            current_jumps = 0;
            on_air = false;
            if (is_dead == false) {
                GetComponent<AudioSource>().clip = land_audio;
                GetComponent<AudioSource>().Play();
            }
        }


        if (collision.gameObject.tag == "Bounce")
        {
            on_air = true;
            character_rb.velocity = new Vector2(0.0f, 0.0f);

            //Get the bounce multiplier from the Bounce platform object
            float bounce_multiplier = 3.0f;
            bounce_multiplier = collision.gameObject.GetComponent<BouncePlatform>().bounce_multiplier;

            character_rb.AddForce(Vector2.up * jump_force * bounce_multiplier, ForceMode2D.Impulse);
            GetComponent<AudioSource>().clip = bounce_audio;
            GetComponent<AudioSource>().Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Death")
        {
            GetComponent<AudioSource>().clip = death_audio;
            GetComponent<AudioSource>().Play();
            Physics2D.gravity = default_gravity;
            is_dead = true;
        }
    }

}
