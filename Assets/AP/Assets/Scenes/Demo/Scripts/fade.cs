using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
	[SerializeField] private CanvasGroup FadeGroup;

	// Start is called before the first frame update
	void Start()
	{
		// Initialization code here if needed
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FadeGroup.alpha = 0;
		}
	}
}
