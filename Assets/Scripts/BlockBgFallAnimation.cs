using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBgFallAnimation : MonoBehaviour
{
	[SerializeField] AnimationCurve _fallAnimationCurve;
	[Range(.1f,2f)]
	[SerializeField] float _duration;
	Transform _myTransform;
	Vector3 _from, _to;
	void Awake()
	{
		_myTransform = transform;
	}

	public void PlayAnimation(Vector3 fromPosition, Vector3 toPosition)
	{
		_from = fromPosition;
		_to = toPosition;
		StopCoroutine("FallAnimationLoop");
		StartCoroutine("FallAnimationLoop");
	}

	IEnumerator FallAnimationLoop()
	{
		float t0 = Time.time;
		float r = 0;
		do
		{
			r = (Time.time - t0) / _duration;
			_myTransform.position = Vector3.LerpUnclamped(_from,_to,1f-_fallAnimationCurve.Evaluate(r));
			yield return null;
		} while (r < 1);
	}
}
