using UnityEngine;
using System.Collections;

public static class TransformExtensions  {
	public static IEnumerator Move(this Transform t, Vector3 target, float duration){
		var diffVector = (target - t.position);
		var diffLength = diffVector.magnitude;
		diffVector.Normalize ();
		float counter = 0;
		while (counter < duration) {
			var movAmount = (Time.deltaTime *diffLength)/duration;
			t.position += diffVector * movAmount;
			counter += Time.deltaTime;
			yield return null;
		}

		t.position = target;
	}
}
