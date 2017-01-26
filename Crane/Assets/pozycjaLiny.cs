using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class pozycjaLiny : MonoBehaviour {
	Slider slider;

	void Start(){
		slider = transform.GetComponent<Slider> ();
	}
	// Update is called once per frame
	void Update () {
		slider.value = CraneManager.Instance.linaPosition ();
	}
}
