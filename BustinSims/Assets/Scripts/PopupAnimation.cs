using UnityEngine;
using System.Collections;

public class PopupAnimation : MonoBehaviour {

	private float animationSpeed = 8;
	private float delay;
	private Vector3 startScale;
	private Vector3 endScale;

	public void SetInitialScales(Vector3 normalScale, float delay) {
		startScale = Vector3.one * 0.1f;
		endScale = normalScale;
		this.delay = delay / animationSpeed;
		transform.localScale = startScale;

		StartCoroutine("FadeIn");
	}

	private IEnumerator FadeIn() {
		yield return new WaitForSeconds(delay);

		float time = 0;
		float journeyLength = Vector3.Distance(startScale, endScale);
		float fracJourney = 0;
        while (fracJourney < 1) {
			float distCovered = time * animationSpeed;
			fracJourney = distCovered / journeyLength;
			transform.localScale = Vector3.Lerp(startScale, endScale, fracJourney);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Destroy(this);
	}
}
