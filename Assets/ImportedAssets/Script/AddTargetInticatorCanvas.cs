using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTargetInticatorCanvas : MonoBehaviour {
	public GameObject indicatorCanvas;
	public GameObject canvas;
	// Use this for initialization
	void Start () {
		CreateTargetInCanvas ();
	}
	void CreateTargetInCanvas (){
		//Instanciando indicador
		TargetIndicatorInCavas targetCanvasLocal = Instantiate (indicatorCanvas).GetComponent<TargetIndicatorInCavas>();
		targetCanvasLocal.objectWorld = this;
		//Colocando indicador como filho do Canvas
		targetCanvasLocal.transform.SetParent (canvas.transform);
		//Definindo uma posição inicial para o indicador (0,0)
		targetCanvasLocal.transform.localPosition = Vector3.zero;
		targetCanvasLocal.Target =  this.transform;

		//Pegando o tipo de Render atual
		Canvas canvasScript = canvas.GetComponent<Canvas> ();
		targetCanvasLocal.renderMode = canvasScript.renderMode;

		targetCanvasLocal.padding = 30f ;

		targetCanvasLocal.thisObject = targetCanvasLocal.GetComponent<RectTransform> ();
		targetCanvasLocal.mainCanvas = canvas.GetComponent<RectTransform> ();

		//Definindo um tamanho para o indicador
		targetCanvasLocal.thisObject.localScale = new Vector3( 0.5f, 0.5f, 0.5f);

	}
}
