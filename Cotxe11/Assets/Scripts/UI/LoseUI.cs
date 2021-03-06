﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseUI : MonoBehaviour
{

    public GameObject Ghost = null;
    public GameObject plant = null;
    public AudioSource camera_audio = null;
    public AudioSource aud_source = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartDay()
    {
        aud_source.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartNight()
    {
        aud_source.Play();
        Ghost.transform.position = plant.transform.position;
        Ghost.GetComponent<GhostController>().is_dead = false;
        Ghost.GetComponent<FlyEnergy>().progress_bar.fillAmount = PlayerPrefs.GetFloat("FlyEnergy");
        Ghost.GetComponent<FlyEnergy>().enough_energy = true;
        Ghost.GetComponent<GhostController>().ghost_animator.SetBool("is_dead", false);
        Ghost.GetComponent<GhostController>().ResetCamera();
        Ghost.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
        transform.parent.gameObject.SetActive(false);
        camera_audio.Play();
    }

}
