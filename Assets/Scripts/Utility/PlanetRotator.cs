using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotator
{
    Texture2D planetTexture;
    public Sprite[] planetTextures;

    void CreateRotateSprites() {
        for (int i = 0; i < planetTextures.Length; i++) {
            if (i == 0) {
                OffsetPixel(planetTexture);
            } else {
                OffsetPixel(planetTextures[i-1].texture);
            }
        }
    }

    Sprite OffsetPixel(Texture2D planet) {
        Texture2D OffsetTexture = planet;
        for (int y = 0; y < planet.height; y++) {
            for (int x = 0; x < planet.width; x++) {
                Color Pixel = planet.GetPixel(x,y);
                if (Pixel == Color.black) {
                    continue;
                } else {
                    
                }
            }
        }
        return Sprite.Create(OffsetTexture, new Rect (0, 0, 512, 512), Vector2.zero);
    }

    public PlanetRotator(Texture2D _planetTexture) {
        
        planetTexture = _planetTexture;
        planetTextures = new Sprite[512];
        CreateRotateSprites();
    }
}
