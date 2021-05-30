using UnityEngine;
using UnityEngine.UI;
namespace ExceptionSoftware.ExScenes
{
    //[RequireComponent(typeof(Image))]
    public class LoadingProgress : MonoBehaviour
    {

        [SerializeField] float _speed = .5f;

        Image[] _image = null;

        private void OnEnable()
        {
            _image = GetComponentsInChildren<Image>();
            FillClockwise = true;
        }

        void Update()
        {
            if (_image == null || _image.Length == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            if (FillClockwise)
            {
                FillAmount += Time.deltaTime * _speed;
            }
            else
            {
                FillAmount -= Time.deltaTime * _speed;
            }

            FillAmount = Mathf.Clamp(FillAmount, 0, 1);

            if (FillAmount == 0)
            {
                FillClockwise = true;
            }

            if (FillAmount == 1)
            {
                FillClockwise = false;
            }
        }

        bool FillClockwise
        {
            get => _image[0].fillClockwise;
            set => _image.ForEach(s => s.fillClockwise = value);
        }
        float FillAmount
        {
            get => _image[0].fillAmount;
            set => _image.ForEach(s => s.fillAmount = value);
        }
    }
}
