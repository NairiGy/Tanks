using TMPro;
using UnityEngine;

namespace Tanks.UI
{
    public class UIHitResultMessage : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float lifetime;
        [SerializeField] private float speed;

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }

            transform.position += transform.up * speed * Time.deltaTime;
        }

        public void SetText(string message)
        {
            text.text = message;
        }
    }
}
