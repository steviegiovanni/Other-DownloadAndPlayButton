using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour {
	// enumeration of the states the button can take
	public enum STATE {
		WAITING_TO_DOWNLOAD = 0,
		DOWNLOAD_PENDING = 1,
		DOWNLOAD_PROGRESS = 2,
		READY_TO_PLAY = 3
	}

	// the state of the button
	[SerializeField]
	private STATE _state;

	// getter setter for the button state
	public STATE State{
		get{ return _state; }
		set{ _state = value; }
	}

	// state transition duration
	[SerializeField]
	private float _transitionDuration = 1.0f;

	// getter setter for the transition duration
	public float TransitionDuration{
		get{ return _transitionDuration; }
		set{ _transitionDuration = value; }
	}

	// minimum scale of the button
	[SerializeField]
	private float _minScale = 1.0f;

	// getter and setter for the minimum scale of the button
	public float MinScale{
		get{ return _minScale; }
		set{ _minScale = value; }
	}

	// maximum scale of the button
	[SerializeField]
	private float _maxScale = 1.25f;

	// getter and setter for the maximum scale of the button
	public float MaxScale{
		get{ return _maxScale;}
		set{ _maxScale = value;}
	}

	// minimum opactity during transition
	[SerializeField]
	private float _minOpacity = 0.5f;

	// getter and setter for the minimum opacity of the button during transition
	public float MinOpacity{
		get{ return _minOpacity;}
		set{ _minOpacity = value;}
	}

	// slowdown the pending texture swap 
	[SerializeField]
	private int _pendingTexSwapSlowdown = 1;

	// getter setter for pending texture swap slowdown
	public int PendingTexSwapSlowdown{
		get{ return _pendingTexSwapSlowdown;}
		set{ _pendingTexSwapSlowdown = value;}
	}

	// assets related
	public GameObject btnBackground;
	public Material bgMaterialBlue;
	public Material bgMaterialOrange;
	public Texture bgBlue;
	public Texture bgOrange;
	public GameObject btnForeground;
	public Material fgMaterial;
	public Texture fgWaitingToDownload;
	public Texture [] fgDownloadPending;
	public Texture [] fgDownloadProgress;
	public Texture fgReadyToPlay;

	// Use this for initialization
	void Start () {
		// set the initial state of the button
		State = STATE.WAITING_TO_DOWNLOAD;

		// set initial material and textures
		btnBackground.GetComponent<Renderer>().material = bgMaterialBlue;
		//bgMaterialBlue.mainTexture = bgBlue;
		//Color bgColor = bgMaterialBlue.color; 
		//bgColor.a = 1.0f;
		//bgMaterialBlue.color = bgColor;
		fgMaterial.mainTexture = fgWaitingToDownload;
		Color fgColor = fgMaterial.color;
		fgColor.a = 1.0f;
		fgMaterial.color = fgColor;

		// set initial size of the button
		this.gameObject.transform.localScale = new Vector3(MinScale,MinScale,MinScale);
	}

	// called when the button is clicked
	public void OnClicked(){
		if (State == STATE.WAITING_TO_DOWNLOAD) {
			State = STATE.DOWNLOAD_PENDING;
			StartCoroutine (ButtonLogic());
		}
	}

	// coroutine called when the button is clicked
	IEnumerator ButtonLogic(){
		while (State != STATE.READY_TO_PLAY) { // run an entire sequence until  state is ready to play
			switch (State) {
			case STATE.DOWNLOAD_PENDING:
				{
					// get parameters to control animation
					float curTime = Time.time;
					float scaleSpeed = (MaxScale - MinScale) / (TransitionDuration / 2);
					float fadeSpeed = 1.0f - MinOpacity / (TransitionDuration); 

					// first half of the transition
					while ((Time.time - curTime) < TransitionDuration / 2) {
						// enlarge the button
						Vector3 curScale = this.gameObject.transform.localScale;
						float deltaScale = Time.deltaTime * scaleSpeed;
						this.gameObject.transform.localScale = curScale + new Vector3(deltaScale,deltaScale,deltaScale);

						// fade out the foreground material
						Color fgColor = fgMaterial.color;
						fgColor.a = fgColor.a - fadeSpeed * Time.deltaTime;
						fgMaterial.color = fgColor;

						yield return null;
					}
						
					// second half of the transition
					while ((Time.time - curTime) < TransitionDuration) {
						// make button smaller
						Vector3 curScale = this.gameObject.transform.localScale;
						float deltaScale = Time.deltaTime * scaleSpeed;
						this.gameObject.transform.localScale = curScale - new Vector3(deltaScale,deltaScale,deltaScale);

						// fade out the foreground material
						Color fgColor = fgMaterial.color;
						fgColor.a = fgColor.a - fadeSpeed * Time.deltaTime;
						fgMaterial.color = fgColor;

						yield return null;
					}

					// reset opacity of foreground
					Color tempFgColor = fgMaterial.color;
					tempFgColor.a = 1.0f;
					fgMaterial.color = tempFgColor;

					// virtually wait for 3s while playing pending animation
					curTime = Time.time;
					int numPendingTex = fgDownloadPending.Length;
					int pendingTexIndex = 0;
					while ((Time.time - curTime) < 3.0f) {
						fgMaterial.mainTexture = fgDownloadPending [pendingTexIndex / PendingTexSwapSlowdown % numPendingTex];
						pendingTexIndex++;
						yield return null;
					}

					// switch state to DOWNLOAD_PROGRESS
					State = STATE.DOWNLOAD_PROGRESS;
				}
				break;
			case STATE.DOWNLOAD_PROGRESS:
				{
					// get parameters to control animation
					float curTime = Time.time;
					float scaleSpeed = (MaxScale - MinScale) / (TransitionDuration / 2);
					float fadeSpeed = 1.0f - MinOpacity / (TransitionDuration); 

					// first half of the transition
					while ((Time.time - curTime) < TransitionDuration / 2) {
						// fade out the foreground material
						Color fgColor = fgMaterial.color;
						fgColor.a = fgColor.a - fadeSpeed * Time.deltaTime;
						fgMaterial.color = fgColor;

						yield return null;
					}

					// reset opacity of foreground
					Color tempFgColor = fgMaterial.color;
					tempFgColor.a = 1.0f;
					fgMaterial.color = tempFgColor;

					// virtually wait for 3s while playing download animation
					curTime = Time.time;
					int numDownloadTex = fgDownloadProgress.Length;
					float downloadTexIndex = 0;
					while ((Time.time - curTime) < 3.0f) {
						fgMaterial.mainTexture = fgDownloadProgress [(int)((downloadTexIndex / 3.0f) * numDownloadTex) % numDownloadTex];
						downloadTexIndex += Time.deltaTime;
						yield return null;
					}

					// play transition to READY_TO_PLAY
					curTime = Time.time;
					Renderer bgRenderer = btnBackground.GetComponent<Renderer> ();
					float lerpParam = 0.0f;
					while ((Time.time - curTime) < TransitionDuration) {
						Debug.Log("Time.time - curTime: "+(Time.time - curTime));
						lerpParam += Time.deltaTime * TransitionDuration;
						Debug.Log("Lerp param: "+lerpParam);
						bgRenderer.material.Lerp(bgMaterialBlue,bgMaterialOrange,lerpParam);
						yield return null;
					}

					// switch state to READY_TO_PLAY
					State = STATE.READY_TO_PLAY;
				}
				break;
			}
			yield return null;
		}
	}
}
