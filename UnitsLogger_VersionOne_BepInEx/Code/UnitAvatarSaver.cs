using System.IO;
using UnityEngine;

namespace UnitsLogger_BepInEx
{

    // Сохранение спрайта юнита
    public class UnitAvatarSaver : MonoBehaviour
    {
        public void SaveAvatarImage(Actor pActor, string filePath)
        {
            // Получаем спрайт для рендера
            Sprite spriteToRender = (Sprite)Reflection.CallMethod(pActor, "getSpriteToRender");

            // Проверка на то, можно ли сохранить текстуру
            if (spriteToRender.texture.isReadable)
            {
                // Создаём новую Text2D с теми же размерами, что и спрайт
                Texture2D texture = new Texture2D((int)spriteToRender.rect.width, (int)spriteToRender.rect.height);

                // Копируем пиксели спрайта в текстуру
                Color[] pixels = spriteToRender.texture.GetPixels((int)spriteToRender.textureRect.x,
                                                                  (int)spriteToRender.textureRect.y,
                                                                  (int)spriteToRender.textureRect.width,
                                                                  (int)spriteToRender.textureRect.height);
                texture.SetPixels(pixels);
                texture.Apply();

                // Кодируем текстуру в PNG
                byte[] bytes = texture.EncodeToPNG();

                // Сохраняем PNG-файл
                File.WriteAllBytes(filePath, bytes);

                // Очистка
                Destroy(texture);
            }
        }

        public void SaveAvatarImage(Texture2D texture, string filePath)
        {
            // Проверка на то, можно ли сохранить текстуру
            if (texture != null)
            {
                // Кодируем текстуру в PNG
                byte[] bytes = texture.EncodeToPNG();

                // Сохраняем PNG-файл
                File.WriteAllBytes(filePath, bytes);

                // Очистка
                Destroy(texture);
            }
        }

        public static Texture2D SaveInitialAvatar(Actor pActor)
        {
            // Получаем спрайт для рендера
            Sprite spriteToRender = (Sprite)Reflection.CallMethod(pActor, "getSpriteToRender");

            // Проверка на то, можно ли сохранить текстуру
            if (spriteToRender.texture.isReadable)
            {
                // Создаём новую Text2D с теми же размерами, что и спрайт
                Texture2D texture = new Texture2D((int)spriteToRender.rect.width, (int)spriteToRender.rect.height);

                // Копируем пиксели спрайта в текстуру
                Color[] pixels = spriteToRender.texture.GetPixels((int)spriteToRender.textureRect.x,
                                                                  (int)spriteToRender.textureRect.y,
                                                                  (int)spriteToRender.textureRect.width,
                                                                  (int)spriteToRender.textureRect.height);
                texture.SetPixels(pixels);
                texture.Apply();

                return texture;
            }

            else
            {
                return null;
            }
        }
    }
}
