﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Incinerator : MonoBehaviour {

	public float CooldownCounter { get; private set; }	
	public GUIBar bar;
	public Canvas canvas;

	[Header("Sound Effects")]
	public AudioClip sfxDestroy;

    private void Awake()
    {
        Assert.IsNotNull(bar);
        Assert.IsNotNull(canvas);
    }

	void Update ()
	{
		if(CooldownCounter > 0)
		{
			CooldownCounter -= Time.deltaTime;
			canvas.gameObject.SetActive (true);
			bar.UpdateCooldownBar (CooldownCounter);
			if(CooldownCounter < 0) CooldownCounter = 0;
		}

		if(CooldownCounter > 0 && canvas.gameObject.activeSelf == false) canvas.gameObject.SetActive (true);
	}

	private void OnTriggerStay(Collider other) 
	{
		if (CooldownCounter > 0) 
		{
			canvas.gameObject.SetActive (false);
			return;
		}

        IGrabable grabable = other.GetComponentInParent<IGrabable>();

        //if (other.CompareTag("Debri"))
        if (grabable != null)
        {
            //Debug.Log("DECREMENTO: " + grabable.GetMassValue());

		    Debri debri = other.GetComponentInParent<Debri>();
			if(debri.IsGrabbed) { return; }

            CooldownCounter = debri.IncineratorCooldown;		    
            GameManager.Instance.AddScore(debri.Score);
            GameManager.Instance.DecrementMessValue(grabable.GetMassValue());
			canvas.gameObject.SetActive (true);

            // TODO play with random pitch - and maybe with a particle animation
			SoundEffects.Instance.Play(sfxDestroy);

            Destroy(debri.gameObject);
		}
	}
}
