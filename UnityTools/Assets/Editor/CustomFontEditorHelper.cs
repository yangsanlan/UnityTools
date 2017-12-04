using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomFontEditorHelper : MonoBehaviour
{

    private static CharacterInfo[] nci;
    [MenuItem("Tools/CustomFont/GetTextureUVAndVert")]
    static void CustomFontDemo()
    {
        if (Selection.objects.Length <= 0) return;
        //选择图片
        var path = AssetDatabase.GetAssetPath(Selection.objects[0]);
        Texture sprite = null;
        TextureImporter setting = null;
        try
        {
            sprite = (Texture)Selection.objects[0];
            setting = (TextureImporter)TextureImporter.GetAtPath(path);
            Debug.Log(setting.spritesheet.Length + ", width: " + sprite.width + ",height: " + sprite.height);
        }
        catch
        {
            Debug.Log("请选择图片及确认图片是否正确");
        }
        nci = new CharacterInfo[setting.spritesheet.Length];
        for (int i = 0; i < setting.spritesheet.Length; i++)
        {
            var item = setting.spritesheet[i];
            string objname = setting.spritesheet[i].name;
            string[] split = objname.Split('_');
            int index = int.Parse(split[1]);
            CharacterInfo ci = new CharacterInfo();
            ci.index = index;
            float uvx = item.rect.x / sprite.width;
            float uvy = item.rect.y / sprite.height;
            float uvw = item.rect.width / sprite.width;
            float uvh = item.rect.height / sprite.height;
            int vertx = 0;
            float verty = item.rect.height / 2;
            float vertwidth = item.rect.width;
            float vertheight = -item.rect.height;

            ci.uvBottomLeft = new Vector2(uvx, uvy);
            ci.uvBottomRight = new Vector2(uvx + uvw, uvy);
            ci.uvTopLeft = new Vector2(uvx, uvy + uvh);
            ci.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);
            //Rect uvRect = new Rect(uvx, uvy, uvw, uvh);
            Rect vertRect = new Rect(vertx, verty, vertwidth, vertheight);
            ci.minX = (int)vertRect.xMin;
            ci.minY = (int)vertRect.yMax;
            ci.maxX = (int)vertRect.xMax;
            ci.maxY = (int)vertRect.yMin;
            ci.advance = (int)vertwidth;

            //以下代码已经是Obsolete
            //ci.width = item.rect.width;
            //ci.uv.x = item.rect.x / sprite.width;
            //ci.uv.y = item.rect.y / sprite.height;
            //ci.uv.width = item.rect.width / sprite.width;
            //ci.uv.height = item.rect.height / sprite.height;
            //ci.vert.x = 0;
            //ci.vert.y = item.rect.height / 2;
            //ci.vert.width = item.rect.width;
            //ci.vert.height = -item.rect.height;
            nci[i] = ci;
            //Debug.Log(ci.uv.ToString() + "," + ci.vert.ToString());
        }
    }

    [MenuItem("Tools/CustomFont/ApplyCustomFont")]
    static void ApplyCustomFont()
    {
        if (nci == null) return;
        try
        {
            Font font = (Font)Selection.objects[0];
            font.characterInfo = nci;
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }
}
