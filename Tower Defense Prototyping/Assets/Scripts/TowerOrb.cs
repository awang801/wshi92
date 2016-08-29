using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerOrb : Tower
{

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/OrbFireSFX", typeof(AudioClip)));
		setStats ("Orb Tower", 1f, 1.5f, 20, 15, 35);
	}


}
