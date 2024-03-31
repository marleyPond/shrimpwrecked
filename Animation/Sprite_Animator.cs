using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//collection of sprite layers
public class Sprite_Animator : MonoBehaviour {

	public List<Sprite_Layer> sprite_layers;

	public void Awake(){
		sprite_layers = new List<Sprite_Layer>();
	}

    public void SetSortingLayerOrderForAll(int layer)
    {
        for (int i = 0; i < sprite_layers.Count; i++)
        {
            sprite_layers[i].SetSortingLayerOrder(layer);
        }
    }

    public void SetCullLayer(int cull_layer)
    {
        for(int i = 0; i < sprite_layers.Count; i++)
        {
            sprite_layers[i].gameObject.layer = cull_layer;
        }
    }

    public void AddLayer(List<Sprite> layer, Color c){
		Sprite_Layer temp = (Sprite_Layer)Instantiate (
            Resources.Load("Prefabs\\Animation\\Sprite_Layer", typeof(Sprite_Layer) ),
                transform.position, Quaternion.identity
            );
		temp.transform.parent = transform;
		if (sprite_layers == null)
			sprite_layers = new List<Sprite_Layer> ();
		temp.SetSprites (layer, c);
		sprite_layers.Add ( temp );
	}

	public void SetIteratorAll(int iter, int min, int max){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].SetIterator( iter, min, max);
		}
	}

	public void SetIterator(int iter, int min, int max, int layer){
		sprite_layers [layer].SetIterator (iter, min, max);
	}

	public void SetLayer(int l, List<Sprite> sl, Color c){
		sprite_layers [l].SetSprites (sl, c);
	}

	public void Iterate(){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].Iterate ();
		}
	}

	public void UpdateIterate(){
		for( int i = 0; i < sprite_layers.Count; i++ ){
			sprite_layers[i].UpdateIterate ();
		}
	}

	public void UpdateIterate(float x, float y, float z){
		for( int i = 0; i < sprite_layers.Count; i++ ){
			sprite_layers[i].UpdateIterate (x, y, z);
		}
	}

	public void UpdateSprite(){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].UpdateSprite ();
		}
	}

	public void UpdateSprite(float x, float y, float z){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].UpdateSprite (x, y, z);
		}
	}

    public void SetLayerOrder(float p)
    {
        for (int i = 0; i < sprite_layers.Count; i++)
        {
            sprite_layers[i].SetLayerOrder(p);
        }
    }

	public void SetLayerOrder(int index, float i){
		sprite_layers [index].SetLayerOrder (i);
	}

	public float GetLayerOrder(int index, int i){
		return sprite_layers [index].GetLayerOrder(i);
	}

	public void SetLocalScale(float scale){
		for( int i = 0; i < sprite_layers.Count; i++ ){
			sprite_layers[i].SetLocalScale(scale);
		}
	}

	public void SetLocalScale(int index, float scale){
		sprite_layers [index].SetLocalScale (scale);
	}
	
	public void SetLayerTitle(int index, string title){
		sprite_layers [index].SetTitle (title);
	}

	public void SetAllToMin(){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].SetToMin();
			sprite_layers[i].UpdateSprite();
		}
	}

    public void ChangeNormalColor(Color c)
    {
        for (int i = 0; i < sprite_layers.Count; i++)
        {
            sprite_layers[i].SetColorNormal(c);
        }
    }

	public void TurnColorAll(Color c){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].TurnColor (c);
		}
	}

	public void TurnColorNormalAll(){
		for (int i = 0; i < sprite_layers.Count; i++) {
			sprite_layers[i].TurnNormalColor ();
		}
	}

    public void SetAnimatorRenderLayer(string s)
    {
        for (int i = 0; i < sprite_layers.Count; i++)
        {
            sprite_layers[i].SetRenderLayer(s);
        }
    }

    public void SetAnimatorRenderLayer(int l)
    {
        for (int i = 0; i < sprite_layers.Count; i++)
        {
            sprite_layers[i].SetRenderLayer(l);
        }
    }

}//eof Sprite_Animator
