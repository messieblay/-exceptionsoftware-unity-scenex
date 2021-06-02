using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace ExceptionSoftware.ExScenes
{
    public class FadeInOut : MonoBehaviour
    {
        public Image image;
        public Color color;
        public float timeFade;
        public AnimationCurve curve;
        float Fade
        {
            get => image.color.a;
            set => image.color = color.SetA(value);
        }
        //void SetColor(Color color) => image.color = color;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public void LoadDefaultData(Color color, float timeToFade, AnimationCurve curve)
        {
            this.color = color;
            this.timeFade = timeToFade;
            this.curve = curve;
            this.Fade = 0;
        }


        public IEnumerator FadeOut(float customTime = -1)
        {
            if (Fade != 0)
            {
                yield return FadeCombatZone(1, 0, customTime < 0 ? timeFade : customTime);
            }
        }

        public IEnumerator FadeIn(float customTime = -1)
        {
            if (Fade != 1)
            {
                yield return FadeCombatZone(0, 1, customTime < 0 ? timeFade : customTime);
            }
        }



        public IEnumerator FadeCombatZone(float fadeFrom, float fadeTo, float seconds)
        {
            Fade = fadeFrom;
            for (float x = 0; x < seconds; x += Time.deltaTime)
            {
                float fade = Mathf.Lerp(fadeFrom, fadeTo, curve.Evaluate(x / seconds));
                Fade = fade;
                yield return null;
            }
            Fade = fadeTo;

            yield return null;
        }
    }
}
