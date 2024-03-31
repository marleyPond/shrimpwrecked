using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//sprite layer in animator
public class Sprite_Layer : MonoBehaviour {

	public List<Sprite> sprites;
	public int iterator = 0;
	int iter_min = 0;
	int iter_max = 0;
	public SpriteRenderer sr;
	float base_depth = 0;
	Color color;
	string title;

	public void Awake(){
		sr = GetComponent<SpriteRenderer> ();
	}

	public void SetTitle(string new_title){
		title = new_title;
		transform.gameObject.name = title;
	}

	public string GetTitle(){
		transform.gameObject.name = title;
		return title;
	}

	public void SetSprites( List<Sprite> sprite_strip, Color col){
		sprites = sprite_strip;
		color = col;
	}

	public void SetSprites( List<Sprite> sprite_strip){
		sprites = sprite_strip;
	}

	public int GetSpriteNumber(){
		if( sprites != null ){
			return sprites.Count;
		}
		return 0;
	}

	public void SetIterator(int n, int min, int max){
		iterator = n;
		iter_min = min;
		iter_max = max;
	}

	public int GetIterator(){
		return iterator;
	}

	public int GetIteratorMin(){
		return iter_min;
	}

	public int GetIteratorMax(){
		return iter_max;
	}

	public void Iterate(){
		if (iterator < iter_max)
			iterator++;
		else
			iterator = iter_min;
	}

	//iterates sprite and updates
	public void UpdateIterate(){
		Iterate ();
		UpdateSprite ();
	}

	//iterates sprite and updates
	public void UpdateIterate(float x, float y, float z){
		Iterate ();
		UpdateSprite (x, y, z);
	}

	public void SetColor(Color col){
        sr.color = col;
	}

	public Color GetColor(){
		return sr.color;
	}

	public void UpdateSprite(){
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();
		sr.sprite = sprites [iterator];
        SetColor(color);
	}

	public void UpdateSprite(float x, float y, float z){
		if (sr == null)
			sr = GetComponent<SpriteRenderer> ();
		sr.sprite = sprites [iterator];
        SetColor(color);
		SetDepth (x, y, z);
	}

	public void SetLayerOrder(float i){
		base_depth = i;
		transform.position = new Vector3 (transform.position.x, transform.position.y, base_depth);
	}

	public float GetLayerOrder(int i){
		return base_depth;
	}

    public void SetSortingLayerOrder(int i)//call from anim<-crew<-sub
    {
        sr.sortingOrder = i;
    }

	public void SetLocalScale(float scale){
		transform.localScale = new Vector3 (scale, scale, 1f);
	}

	public void SetDepth(float x, float y, float z){
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		if (sr != null)
			sr.sortingOrder = (int)(x*2.5f - y*2.51f + (z*10))+1000;
	}

	public void SetToMin(){
		sr.sprite = sprites [iter_min];
		iterator = iter_min;
	}

	public void TurnColor(Color c){
		sr.color = c;
	}

    public void SetColorNormal(Color c)
    {
        color = c;
    }

	public void TurnNormalColor(){
		sr.color = color;
	}

    public void SetRenderLayer(string s)
    {
        sr.sortingLayerName = s;
    }

    public void SetRenderLayer(int i)
    {
        sr.sortingLayerID = i;
    }

}//eof Sprite_Layer
