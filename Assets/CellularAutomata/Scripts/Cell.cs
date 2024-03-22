using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Cell : MonoBehaviour
{
  [SerializeField] private Color traversableColor;
  [SerializeField] private Color nonTraversableColor;
  [SerializeField] private Color wallColor;
   private GameObject _object;
   private SpriteRenderer _spriteRenderer;
   private Transform _transform;

   public void Init(GameObject obj, SpriteRenderer spr, Transform tr)
   {
     _object = obj;
     _spriteRenderer = spr;
     _transform = tr;
   }
   private void ChangeColor(Color color)
   {
     _spriteRenderer.color = color;
   }

   public void SetPosition(int x, int y)
   {
     transform.position = new Vector3(x, y, 0);
   }

   public void SetTraversable(bool traversable, bool wall = false)
   {
     if (traversable)
     {
       ChangeColor(traversableColor);
     }
     else
     {
       if (wall)
       {
         ChangeColor(wallColor);
       }
       else
       {
         ChangeColor(nonTraversableColor);
       }
     }
   }

   public bool Traversable()
   {
     return _spriteRenderer.color == traversableColor;
   }
}
