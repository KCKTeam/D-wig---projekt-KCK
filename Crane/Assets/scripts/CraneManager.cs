using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CraneManager : MonoBehaviour {

	public GameObject toLiftObject;
	public Transform prowadnica;
	public Transform hak;
	public Transform lina;
	float promienDzwigu=40f;
	public float speed = 5f;
	private static CraneManager instance;
	public static CraneManager Instance{ get { return instance; } }
	public Transform sektory;

	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
	}

	public Transform Crane;

	void Start(){
		//StartCoroutine (opuscHak (-10f));
	}

	public IEnumerator obroc(float rotation){
		if (rotation > 0) {
			float rotationSpeed=0.5f;
			while (rotation > 0) {
				Crane.rotation = Quaternion.Euler (Crane.rotation.eulerAngles.x, Crane.rotation.eulerAngles.y + rotationSpeed, Crane.rotation.eulerAngles.z);
				rotation -= rotationSpeed;
				yield return new WaitForSeconds (0.01f);
			}
		} else {
			float rotationSpeed=-0.5f;
			while (rotation < 0) {
				Crane.rotation = Quaternion.Euler (Crane.rotation.eulerAngles.x, Crane.rotation.eulerAngles.y + rotationSpeed, Crane.rotation.eulerAngles.z);
				rotation -= rotationSpeed;
				yield return new WaitForSeconds (0.01f);
			}
		}
	}

	public IEnumerator podniesObiekt(GameObject obiekt){
		yield return StartCoroutine (obroc(Angle(obiekt)));
		yield return StartCoroutine (przesunProwadnice (Distance (obiekt)));
		yield return StartCoroutine (lineDown(obiekt));
		yield return StartCoroutine (lineUp ());
	}

	public IEnumerator podniesIodloz(GameObject obiekt){
		yield return StartCoroutine (obroc(Angle(obiekt)));
		yield return StartCoroutine (przesunProwadnice (Distance (obiekt)));
		yield return StartCoroutine (lineDown(obiekt));
		yield return StartCoroutine (lineUp ());
		yield return new WaitForSeconds (2);
		yield return StartCoroutine (putObjectDown ());
		yield return lineUp ();
	}

	public IEnumerator odlozIpodnies(GameObject obiekt){
		yield return StartCoroutine (putObjectDown ());
		yield return lineUp ();
		yield return StartCoroutine (obroc(Angle(obiekt)));
		yield return StartCoroutine (przesunProwadnice (Distance (obiekt)));
		yield return StartCoroutine (lineDown(obiekt));
		yield return StartCoroutine (lineUp ());
	}

	public IEnumerator putDown(){
		yield return StartCoroutine (putObjectDown());
		yield return StartCoroutine (lineUp ());

	}

	public IEnumerator odlozIpodnies(GameObject obiekt, string sektor){
		yield return polozWsektorze (sektor);
		yield return podniesObiekt (obiekt);

	}

	public IEnumerator polozTrzymanyObok(GameObject stojacy){
		Vector2 lokalizacja=znajdzLokalizacje(trzymanyObiekt(), stojacy);
		yield return StartCoroutine (obroc(Angle(lokalizacja)));
		yield return StartCoroutine (przesunProwadnice (Distance (lokalizacja)));
		yield return StartCoroutine (putObjectDown());
		yield return StartCoroutine (lineUp ());
	}

	public IEnumerator polozObok(GameObject odkladany, GameObject stojacy){
		yield return StartCoroutine (podniesObiekt (odkladany));

		Vector2 lokalizacja=znajdzLokalizacje(odkladany, stojacy);
		yield return StartCoroutine (obroc(Angle(lokalizacja)));
		yield return StartCoroutine (przesunProwadnice (Distance (lokalizacja)));
		yield return StartCoroutine (putObjectDown());
		yield return StartCoroutine (lineUp ());
	}

	public IEnumerator polozWsektorzeIpodnies(string sektor, GameObject obiekt){
		yield return polozWsektorze (sektor);
		yield return podniesObiekt (obiekt);
	}
	public IEnumerator polozWsektorze(string sektor){
		Vector2 lokalizacja = Vector2.zero;
		if (sektory.FindChild (sektor)) {
		 lokalizacja = new Vector2 (sektory.FindChild (sektor).transform.position.x, sektory.FindChild (sektor).transform.position.z);
		}
		if (lokalizacja != Vector2.zero) {
			craneText ("Umieszczam "+ trzymanyObiekt().GetComponent<objectProperties>().kolor+ " " + trzymanyObiekt().GetComponent<objectProperties>().rodzaj+ " w sektorze " + sektor.ToUpper());
			yield return StartCoroutine (obroc (Angle (lokalizacja)));
			yield return StartCoroutine (przesunProwadnice (Distance (lokalizacja)));
			yield return StartCoroutine (putObjectDown ());
			yield return StartCoroutine (lineUp ());
		} else {
			craneText ("Nie znalazłem podanego sektora lub nie jest on w moim zasięgu.");
		}
	}

	public IEnumerator podniesIpolozWsektorze(GameObject obiekt, string sektor){
		yield return podniesObiekt (obiekt);
		craneText ("Podnoszę "+obiekt.GetComponent<objectProperties>().kolor+" "+obiekt.GetComponent<objectProperties>().rodzaj);
		yield return polozWsektorze (sektor);
	}

	public IEnumerator podniesZsektora(string sektor){
		GameObject doPodniesienia = sektory.FindChild (sektor).GetComponent<detekcjaObiektu> ().pobierzObiekt ();
		Debug.Log (sektory.FindChild (sektor).GetComponent<detekcjaObiektu> ().pobierzObiekt().GetComponent<objectProperties>().rodzaj);
		if (doPodniesienia != null) {
			craneText ("Podnoszę "+doPodniesienia.GetComponent<objectProperties>().kolor+ " " + doPodniesienia.GetComponent<objectProperties>().rodzaj+" z sektora "+sektor);
			yield return StartCoroutine (podniesObiekt (doPodniesienia));
		} else {
			craneText ("Nie znalazłem obiektu w sektorze " + sektor.ToUpper());
		}

	}

	public IEnumerator putObjectDown(){
		GameObject obiekt = hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody.gameObject;
		Debug.Log (obiekt.GetComponent<objectProperties> ().rodzaj + " " + obiekt.GetComponent<objectProperties> ().kolor);
		if (obiekt != null) {
			float przesun = obiekt.transform.GetChild (1).transform.position.y;
			float time = 0.01f;
			if (przesun < 0) {
				float moveDistance = -0.1f;
				while (przesun < 0f) {
					lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
					przesun += moveDistance;
					yield return new WaitForSeconds (time);
				}
			} else {
				float moveDistance = 0.1f;
				while (przesun > 0f) {
					lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
					przesun -= moveDistance;
					yield return new WaitForSeconds (time);
				}
			}
			hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody = null;
		}
	}

	public IEnumerator opuscHak(float distance){
		float przesun = distance;
		float time = 0.01f;
		if (przesun < 0) {
			float moveDistance = 0.1f;
			while (przesun < 0f) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z - moveDistance);
				przesun += moveDistance;
				yield return new WaitForSeconds (time);
			}
		}else {
			float moveDistance = 0.1f;
			while (przesun > 0f) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z+moveDistance);
				przesun -= moveDistance;
				yield return new WaitForSeconds (time);
			}
		}
	}

	IEnumerator lineDown(GameObject obiekt){
		float yObiekt = obiekt.transform.GetChild(0).transform.position.y;
		float yHak = hak.position.y;
		float przesun = yObiekt - yHak+1.4f;
		float time = 0.01f/speed;

		if (przesun < 0) {
			float moveDistance = 0.1f;
			while (przesun < 0f) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
				przesun += moveDistance;
				yield return new WaitForSeconds (time);
			}
			hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody = obiekt.GetComponent<Rigidbody> ();
		}else {
			float moveDistance = -0.1f;
			while (przesun > 0f) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z+moveDistance);
				przesun -= moveDistance;
				yield return new WaitForSeconds (time);
			}
			hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody = obiekt.GetComponent<Rigidbody> ();
		}
	}

	IEnumerator lineUp(){
		float przesun = 20f;
		float time = 0.01f/speed;
		float moveDistance = 0.1f;
		while (przesun > 0f) {
			lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z - moveDistance);
			przesun -= moveDistance;
			yield return new WaitForSeconds (time);
		}
	}
		
	float Angle(GameObject obiekt){
		Vector2 vec1 = new Vector2 (transform.position.x, transform.position.z);
		Vector2 vec2 = new Vector2 (obiekt.transform.GetChild(0).position.x, obiekt.transform.GetChild(0).position.z);
		Vector2 vec3 = new Vector2 (hak.position.x, hak.position.z);
		float x = vec2.x - vec1.x;
		float y = vec2.y - vec1.y;
		float tan = Mathf.Atan(y/x);
		float angle = tan * (180 / Mathf.PI);

		float x1 = vec3.x - vec1.x;
		float y1 = vec3.y - vec1.y;
		float tan1 = Mathf.Atan(y1/x1);
		float angle1 = tan1 * (180 / Mathf.PI);

		if (vec2.x < 0 && vec2.y > 0) {
			angle = 180 + angle;
		}
		if (vec2.x < 0 && vec2.y < 0) {
			angle = -180 + angle;
		}
		if (vec3.x < 0 && vec3.y > 0) {
			angle1 = 180 + angle1;
		}
		if (vec3.x < 0 && vec3.y < 0) {
			angle1 = -180 + angle1;
		}

		float angle2=angle - angle1;
		return angle2*(-1);
	}

	float Angle(Vector2 lokalizacja){
		Vector2 vec1 = new Vector2 (transform.position.x, transform.position.z);
		Vector2 vec2 = new Vector2 (lokalizacja.x, lokalizacja.y);
		Vector2 vec3 = new Vector2 (hak.position.x, hak.position.z);
		float x = vec2.x - vec1.x;
		float y = vec2.y - vec1.y;
		float tan = Mathf.Atan(y/x);
		float angle = tan * (180 / Mathf.PI);

		float x1 = vec3.x - vec1.x;
		float y1 = vec3.y - vec1.y;
		float tan1 = Mathf.Atan(y1/x1);
		float angle1 = tan1 * (180 / Mathf.PI);

		if (vec2.x < 0 && vec2.y > 0) {
			angle = 180 + angle;
		}
		if (vec2.x < 0 && vec2.y < 0) {
			angle = -180 + angle;
		}
		if (vec3.x < 0 && vec3.y > 0) {
			angle1 = 180 + angle1;
		}
		if (vec3.x < 0 && vec3.y < 0) {
			angle1 = -180 + angle1;
		}

		float angle2=angle - angle1;
		Debug.Log (angle2);
		return angle2*-1f;
	}

	public IEnumerator przesunProwadnice(float distance){
		float time = 0.01f;
		if (distance > 0) {
			float moveDistance = 0.1f;
			while (distance > 0f) {
				prowadnica.transform.localPosition = new Vector3 (prowadnica.transform.localPosition.x, prowadnica.transform.localPosition.y, prowadnica.transform.localPosition.z + moveDistance);
				distance -= moveDistance;
				yield return new WaitForSeconds (time);
			}
		} else {
			float moveDistance = -0.1f;
			while (distance < 0f) {
				prowadnica.transform.localPosition = new Vector3 (prowadnica.transform.localPosition.x, prowadnica.transform.localPosition.y, prowadnica.transform.localPosition.z + moveDistance);
				distance -= moveDistance;
				yield return new WaitForSeconds (time);
			}
		}
	}



	Vector2 znajdzLokalizacje(GameObject trzymany, GameObject docelowy){
		Vector3 trzymanyWymiary = trzymany.GetComponent<BoxCollider> ().size;
		Vector3 docelowyWymiary = docelowy.GetComponent<BoxCollider> ().size;
		float odkladanyNajdluzsza=najdluzszaKrawedz(trzymanyWymiary);
		float stojacyNajdluzsza=najdluzszaKrawedz(docelowyWymiary);
		float odlegloscPostawienia = (odkladanyNajdluzsza + stojacyNajdluzsza) / 2+0.5f;

		float odleglosc;
		List <Vector2> mozliweLokalizacje=new List<Vector2>();

		Vector2 vec1 = new Vector2 (docelowy.transform.position.x+odlegloscPostawienia, docelowy.transform.position.z);
		Vector2 vec2 = Vector2.zero;
		odleglosc = Vector2.Distance (vec1,vec2);
		if (odleglosc < promienDzwigu) {
			mozliweLokalizacje.Add (vec1);
		};

		vec1 = new Vector2 (docelowy.transform.position.x-odlegloscPostawienia, docelowy.transform.position.z);
		odleglosc = Vector2.Distance (vec1,vec2);
		if (odleglosc < promienDzwigu) {
			mozliweLokalizacje.Add (vec1);
		};

		vec1 = new Vector2 (docelowy.transform.position.x, docelowy.transform.position.z+odlegloscPostawienia);
		odleglosc = Vector2.Distance (vec1,vec2);
		if (odleglosc < promienDzwigu) {
			mozliweLokalizacje.Add (vec1);
		};

		vec1 = new Vector2 (docelowy.transform.position.x, docelowy.transform.position.z-odlegloscPostawienia);
		odleglosc = Vector2.Distance (vec1,vec2);
		if (odleglosc < promienDzwigu) {
			mozliweLokalizacje.Add (vec1);
		};

		int lokalizacja=Random.Range (0,mozliweLokalizacje.Count);

		return mozliweLokalizacje [lokalizacja];
	}

	float najdluzszaKrawedz(Vector3 wektor){
		float najdluzsza = 0f;
		if (najdluzsza < wektor.x)
			najdluzsza = wektor.x;
		if (najdluzsza < wektor.y)
			najdluzsza = wektor.y;
		if (najdluzsza < wektor.z)
			najdluzsza = wektor.z;
		return najdluzsza;
	}

	float Distance (GameObject obiekt){
		Vector2 vec1 = new Vector2 (obiekt.transform.GetChild(0).position.x, obiekt.transform.GetChild(0).position.z);
		Vector2 vec2 = new Vector2 (prowadnica.position.x, prowadnica.position.z);
		float distance = Vector2.Distance (vec1, vec2);

		if (vec2.y >= 0) {
			if(vec1.y<=vec2.y)
				distance=distance*(-1);
		}
		if (vec2.y < 0) {
			if(vec1.y>=vec2.y)
				distance=distance*(-1);
		}

		return distance;
	}

	float Distance (Vector2 lokalizacja){
		Vector2 vec1 = new Vector2 (lokalizacja.x, lokalizacja.y);
		Vector2 vec2 = new Vector2 (prowadnica.position.x, prowadnica.position.z);
		float distance = Vector2.Distance (vec1, vec2);

		if (vec2.y >= 0) {
			if(vec1.y<=vec2.y)
				distance=distance*(-1);
		}
		if (vec2.y < 0) {
			if(vec1.y>=vec2.y)
				distance=distance*(-1);
		}

		Debug.Log (distance);
		return distance;
	}

	public bool checkJoint(){
		bool joint = !(hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody == null);
		return joint;
	}

	public GameObject trzymanyObiekt(){
		if (checkJoint()) {
			GameObject trzymany=hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody.gameObject;
			return trzymany;
		} else
			return null;
	}

	public Text craneResponse;
	public Text playerText;
	public Transform TextContainer;

	public void craneText(string text){
		Text newText=Instantiate (craneResponse, TextContainer, worldPositionStays:false) as Text;
		newText.text = text;
	}

	public void myText(string text){
		Text newText=Instantiate (playerText, TextContainer, worldPositionStays:false) as Text;
		newText.text = text;
	}

	public float prowadnicaPosition(){
		return prowadnica.localPosition.z;
	}

	public float linaPosition(){
		return lina.localScale.z;
	}
}