using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Control : MonoBehaviour {

	public Button  prefAnimation;
	public Button  nextAnimation;

	public GameObject model1;
	public GameObject model2;
	public GameObject model3;

	private MD2Model animation1;
	private MD2Model animation2;
	private MD2Model animation3;
	void OnGUI()
	{
        GUI.Label(new Rect(10, 10, 100, 20), "Animation:"+animation1.currAnimation.name);
		
			}

	void Start () 
	{
		prefAnimation.onClick.AddListener (clickPref);
		nextAnimation.onClick.AddListener(clickNext);


        animation1 = model1.GetComponent<MD2Model>();
        animation2 = model2.GetComponent<MD2Model>();
        animation3 = model3.GetComponent<MD2Model>();

		if (!animation1) 
		{
			Debug.LogError("game object dont have md2");
		}
		if (!animation2) 
		{
			Debug.LogError("game object dont have md2");
		}
		if (!animation3) 
		{
			Debug.LogError("game object dont have md2");
		}

	}

		private void clickNext()
		{
		animation1.NextAnimation ();
		animation2.NextAnimation ();
		animation3.NextAnimation ();
		}
		private void clickPref()
		{
	    animation1.BackAnimation ();
		animation2.BackAnimation ();
		animation3.BackAnimation ();

	}
		}
