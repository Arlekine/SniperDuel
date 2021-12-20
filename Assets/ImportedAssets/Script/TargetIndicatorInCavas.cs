using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicatorInCavas : MonoBehaviour {
	//Guarda uma referência para o script que instanciou
	[HideInInspector] public AddTargetInticatorCanvas objectWorld;
	//Guarda uma referência do transform que instanciou
	[HideInInspector] public Transform Target;
	//Vai recer o modo atual de Render
	[HideInInspector] public RenderMode renderMode;
	//Transform do objeto dentro do Canvas
	[HideInInspector] public RectTransform thisObject;
	//Guarda uma referência para o canvas da cena
	[HideInInspector] public RectTransform mainCanvas;


	//Recebe o valor para espalo interno
	[HideInInspector] public float         padding;

	
	void Update () {
		UpdateInfoIndicatorCanvas ();
	}

	void UpdateInfoIndicatorCanvas(){
		UpdateIndicatorPosition ();
		//Demais ações...
	}

	private void UpdateIndicatorPosition(){
		switch(renderMode){

			case RenderMode.ScreenSpaceOverlay:{
					Vector3 convertedPosition = Camera.main.WorldToViewportPoint (Target.position);
					
					if(convertedPosition.z < 0){
						
						convertedPosition = Vector3Invert (convertedPosition);
						convertedPosition = Vector3FixEdge (convertedPosition);
					}
					convertedPosition = Camera.main.ViewportToScreenPoint (convertedPosition);
					KeepCameraInside (convertedPosition);
			}
			break;
		case RenderMode.ScreenSpaceCamera:{
				
				Debug.LogWarning ("Render Mode - Screen Space Camera não suportado!");
			}
			break;
		case RenderMode.WorldSpace:{
				
			Debug.LogWarning ("Render Mode - Worl Space não suportado!");
		}
		break;
	 }
	}
	private void KeepCameraInside(Vector2 reference){
		//Mantendo o objeto dentro do Tela somando com o padding
		reference.x = Mathf.Clamp(reference.x, padding, Screen.width - padding);
		reference.y = Mathf.Clamp(reference.y, padding, Screen.height - padding);
		thisObject.transform.position = reference;

	}


	Vector3 Vector3Invert(Vector3 viewport_position){
		Vector3 invertedVector = viewport_position;
		//Inverte position com base na dimensão da tela
		invertedVector.x = 1f - invertedVector.x;
		invertedVector.y = 1f - invertedVector.y;
		invertedVector.z = 0;
		return invertedVector;
	}
	public Vector3 Vector3FixEdge(Vector3 vector)
	{
		Vector3 vectorFixed = vector;

		//Obtendo maior valor do Vetor, maior valor sempre maior que 0
		float highestValue = Vector3Max(vectorFixed);

		//Obtendo menor valor do Vetor, menor valor sempre menor que 0
		float lowerValue = Vector3Min(vectorFixed);

		//Obento o lado mais próximo do alvo para o indicador ficar [0,0] ou  [1,1]

		float highestValueBetween = DirectionPreference (lowerValue, highestValue);

		Debug.Log ("Vetor:"+vector+" Maior : "+highestValue+" Menor : "+lowerValue+" Melhor:"+highestValueBetween);


		/**Fixando vetor na borda [1,1]**/

		//Se highest for o melhor para olhar
		if (highestValueBetween == highestValue) {

			vectorFixed.x = vectorFixed.x == highestValue ? 1 :vectorFixed.x;
			vectorFixed.y = vectorFixed.y == highestValue ? 1 : vectorFixed.y;

			//Se menor valor for o melhor olhar
		}

		/*Fixando vetor na bora [0,0]*/

		//Se lowerValue for o melhor para olhar
		if(highestValueBetween == lowerValue){

			vectorFixed.x = Mathf.Abs(vectorFixed.x) == lowerValue ? 0 : Mathf.Abs(vectorFixed.x);
			vectorFixed.y = Mathf.Abs(vectorFixed.y) == lowerValue ? 0 :  Mathf.Abs(vectorFixed.y);
		}
		Debug.Log ("Vetor:"+vectorFixed);
		return vectorFixed;
	}
	float Vector3Max(Vector3 vector){
		
		float highestValue = Mathf.Max (vector.x, vector.y);
		return highestValue;
	}
	float Vector3Min(Vector3 vector){
		float lowerValue = 0f;
		lowerValue = vector.x <= lowerValue ? vector.x : lowerValue;
		lowerValue = vector.y <= lowerValue ? vector.y : lowerValue;

		return lowerValue;
	}

	float DirectionPreference(float lowerValue, float highestValue){
		
		//Convertendo valores para positivo para ver qual é maior;
		lowerValue = Mathf.Abs (lowerValue);
		highestValue = Mathf.Abs (highestValue);


		//Obetendo maior valor entre os dois
		float highestValueBetween = Mathf.Max(lowerValue, highestValue);

		return highestValueBetween;
	}
}
