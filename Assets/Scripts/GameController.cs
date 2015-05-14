/*
Author: Trevor Richardson
GameController.cs
04-01-2015

	Controls the main game flow. Skeletons are spawned in random locations and various
	animation transitions are programatically triggered by certain events. These events 
	include correctly entering the text over their head and the player getting hit by
	the skeleton, the latter of which will trigger a camera shake.
	
 */
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
		// Used to spawn the skeleton and refer to its various components
		public GameObject skeletonPrefab;
		public GameObject skeleton;

		// Text above skeleton's head
		private TextMesh skellyText;

		// Flags for tracking animation transitions and text
		public bool attacked;
		public bool easyWordSet;
		public bool easyWordGot;
		public bool hardWordSet;
		public bool hardWordGot;
		public bool deathFlag;

		// Dictionary of words
		public string[] easy;
		public string[] hard;

		// Where enetered text is displayed
		public GUIText enteredText;
		private int index;

		// Animation variables
		Animator anim;
		static int walking = Animator.StringToHash ("Base.Walking");
		static int crawling = Animator.StringToHash ("Base.Crawling");
		static int attacking = Animator.StringToHash ("Base.Attacking");
		static int deading = Animator.StringToHash ("Base.Dead");

		// Camera shake controls and flag
		float shakeTime = 0;
		float shakeAmount = 0.05f;
		float shakeDampening = 1.0f;
		public bool shaken;

		// Fills the dictionary and spawns a skeleton
		void Start ()
		{
				easy = new string[5] {"easy", "unity", "vector", "yield", "state"};
				hard = new string[5] {
						"instantiate",
						"skeleton",
						"animation",
						"orientation",
						"getcurrentanimatorstateinfo"
				};
				enteredText = GameObject.Find ("EnteredText").GetComponent<GUIText> ();
				Spawn ();
		}

		// Instatiates skeleton and resets variables
		void Spawn ()
		{
				skeleton = (GameObject)Instantiate (
			skeletonPrefab, new Vector3 (Random.Range (-4.0f, 4.0f), 0, -2.5f), Quaternion.identity);
				anim = skeleton.GetComponent<Animator> ();
				skellyText = skeleton.GetComponentInChildren<TextMesh> ();
				attacked = easyWordSet = easyWordGot = hardWordSet = hardWordGot = deathFlag = shaken = false;
		}

		// Removes skeleton from game after 2s
		IEnumerator Death ()
		{
				yield return new WaitForSeconds (2.0f);
				Destroy (skeleton);
				Spawn ();
				yield return null;
		}

		// Shakes the camera corresponding to strikes from the attack animation
		IEnumerator Shake ()
		{
				yield return new WaitForSeconds (0.5f);		
				shakeTime = 1.0f;
				yield return new WaitForSeconds (1.5f);
				shakeTime = 1.0f;
				yield return new WaitForSeconds (1.5f);
				shakeTime = 1.0f;
				yield return new WaitForSeconds (1.5f);
		}
	
		void Update ()
		{
				// Track skeleton animation state
				AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo (0);

				// Captures and displays lowercase alpha text
				foreach (char c in Input.inputString) {
						if (c >= 'a' && c <= 'z')
								enteredText.text += c;
			// Backspace
						else if (c == '\b') {
								if (enteredText.text.Length != 0)
										enteredText.text = enteredText.text.Substring (0, enteredText.text.Length - 1);
								// Checks if the correct word was input and triggers the corresponding animation transition and gun shot if so
						} else if (c == '\n' || c == '\r' || c == ' ') {
								if (!easyWordGot && enteredText.text.Equals (easy [index])) {
										easyWordGot = true;
										skellyText.text = "";
										GetComponent<AudioSource> ().Play ();
										anim.SetTrigger ("easyTrigger");
								}
								if (!hardWordGot && enteredText.text.Equals (hard [index])) {
										hardWordGot = true;
										skellyText.text = "";
										GetComponent<AudioSource> ().Play ();
										anim.SetTrigger ("hardTrigger");
								}
								// Reset text
								enteredText.text = "";
						}
				}
				// If walking, generate a word and move towards player
				if (state.nameHash == walking) {
						skeleton.transform.position += skeleton.transform.forward * Time.deltaTime;
						if (!easyWordSet) {
								index = Mathf.FloorToInt (Random.Range (0f, 5f));
								skellyText.text = easy [index];
								easyWordSet = true;
						}
						// If walking and in range of player, begin attacking
						if (skeleton.transform.position.z > 3.75f && attacked == false) {
								skellyText.text = "";
								anim.SetTrigger ("attackTrigger");
								attacked = true;
						}
				}

				// If crawling, move slower and generate a harder word
				if (state.nameHash == crawling) {
						skeleton.transform.position += skeleton.transform.forward * Time.deltaTime * 0.75f;
						if (!hardWordSet) {
								index = Mathf.FloorToInt (Random.Range (0f, 5f));
								skellyText.text = hard [index];
								hardWordSet = true;
						}
				}

				// Programmatic camera shake when being attacked
				if (state.nameHash == attacking && !shaken) {
						shaken = true;
						StartCoroutine (Shake ());
				}
				// If dead, start the death coroutine and sink the double corpse into the ground
				if (state.nameHash == deading) {
						if (!deathFlag) {
								StartCoroutine (Death ());
								deathFlag = true;
						}
						skeleton.transform.position -= skeleton.transform.up * Time.deltaTime * 0.25f;
				}

				// Despawn if skeleton reaches camera and spawn a new one
				if (skeleton.transform.position.z > 5) {
						Destroy (skeleton);
						Spawn ();
				}
				// Simple programmatic camera shake that moves it around its parent object
				if (shakeTime > 0) {
						Camera.main.transform.localPosition = Random.insideUnitSphere * shakeAmount;
						shakeTime -= Time.deltaTime * shakeDampening;
			
				} else {
						shakeTime = 0.0f;
				}
		
		}
}
