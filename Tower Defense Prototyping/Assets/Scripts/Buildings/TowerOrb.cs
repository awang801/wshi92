using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerOrb : Tower
{

	protected override void Awake()
	{
		base.Awake ();
		shootSFX = (AudioClip)(Resources.Load("Sounds/OrbFireSFX", typeof(AudioClip)));
		//setStats(string _name, float _adamage, float _adelay, int _cost, int _sellvalue, int _upcost)
		setStats ("Orb", 1f, 0.75f, 20, 15, 35);
	}


}
