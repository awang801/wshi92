using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerCannon : Tower
{

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/CannonFireSFX", typeof(AudioClip)));
		setStats ("Cannon", 2f, 2f, 40, 25, 60);
	}


}
