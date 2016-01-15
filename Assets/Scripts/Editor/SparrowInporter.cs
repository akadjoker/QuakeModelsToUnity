/*
* 
* Copyright (c) 2015 Luis Santos AKA DJOKER
 * 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Globalization;

public static class SparrowInporter
{
    [MenuItem("Assets/Djoker Tools/Sprite Sheet Packer/SparrowXMLImport")]
    static void ProcessToSprite()
    {
        TextAsset txt = (TextAsset)Selection.activeObject;

        string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(txt));
        string path = rootPath + "/" + txt.name + ".PNG";

    
        Texture2D tmpTexture = new Texture2D(1, 1);
        byte[] tmpBytes = File.ReadAllBytes(path);
        tmpTexture.LoadImage(tmpBytes);

  
        List<SpriteMetaData> sprites = Parse(txt, tmpTexture.height);
        TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

        texImp.spritesheet = sprites.ToArray();
        texImp.textureType = TextureImporterType.Sprite;
        texImp.spriteImportMode = SpriteImportMode.Multiple;

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

     

      //  string nm = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(txt));
      //  string materialAssetPath = AssetDatabase.GenerateUniqueAssetPath(rootPath + "/" + txt.name + ".asset");
      //  AssetDatabase.CreateAsset(texImp, materialAssetPath);


     //   AssetDatabase.Refresh();

    }
    static List<SpriteMetaData> Parse(TextAsset text, int height)
    {

            List<SpriteMetaData> Sprites = new List<SpriteMetaData>();
           XDocument xmlDocument = XDocument.Parse(text.text);

        
            if (xmlDocument.Root.Name.LocalName != "TextureAtlas")
                return null;



         
            var sprites = xmlDocument.Descendants("SubTexture");

       

            foreach (var sprite in sprites)
            {
                string n = sprite.Attribute("name").Value;
                Debug.Log("name :" + n);

                int x = int.Parse(sprite.Attribute("x").Value, CultureInfo.InvariantCulture);
                int y = int.Parse(sprite.Attribute("y").Value, CultureInfo.InvariantCulture);
                int w = int.Parse(sprite.Attribute("width").Value, CultureInfo.InvariantCulture);
                int h = int.Parse(sprite.Attribute("height").Value, CultureInfo.InvariantCulture);

                Rect r = new Rect(x-1, y-1, w+2, h+2);

     

                int oX = (sprite.Attribute("frameX") == null) ? 0 : int.Parse(sprite.Attribute("frameX").Value, CultureInfo.InvariantCulture) * -1;
                int oY = (sprite.Attribute("frameY") == null) ? 0 : int.Parse(sprite.Attribute("frameY").Value, CultureInfo.InvariantCulture) * -1;
                int oW = (sprite.Attribute("frameWidth") == null) ? w : int.Parse(sprite.Attribute("frameWidth").Value, CultureInfo.InvariantCulture);
                int oH = (sprite.Attribute("frameHeight") == null) ? h : int.Parse(sprite.Attribute("frameHeight").Value, CultureInfo.InvariantCulture);


                bool trim =
                    (sprite.Attribute("frameX") != null) ||
                    (sprite.Attribute("frameY") != null) ||
                    (sprite.Attribute("frameWidth") != null) ||
                    (sprite.Attribute("frameHeight") != null);



                SpriteMetaData spriteMetaData = new SpriteMetaData();
                spriteMetaData.alignment = trim ? 9 : 0;
                spriteMetaData.name = n;
                spriteMetaData.pivot = new Vector2(((oW / 2.0f) - (oX)) / (float)w, 1.0f - ((oH / 2.0f) - (oY)) / (float)h);
                spriteMetaData.rect = new Rect(r.x, height - r.y - r.height, r.width, r.height); 
        

                Sprites.Add(spriteMetaData);
            }
            
            return Sprites;
        }
    
}
