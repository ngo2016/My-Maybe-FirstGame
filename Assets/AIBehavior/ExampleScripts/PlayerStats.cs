using UnityEngine;
using UnityEngine.UI;

namespace AIBehaviorExamples
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource loopAudio; //for breath scare sound
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip deadSound;
        [SerializeField] GameObject hurtImg;
        [SerializeField] Text scoreText;
        [SerializeField] private int score;

        private float health = 100.0f;

        public int Score1 { get => score; set => score = value; }

        public float Health { get => health; set => health = value; }

        private void Awake()
        {
#if UNITY_EDITOR
		    Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        public void SubtractHealth(float amount)
        {
            health -= amount;

            if (health <= 0.0f)
            {
                health = 0.0f;
                Debug.LogWarning("You're Dead!");
                //Disable loop audio
                loopAudio.enabled = false;
                //Play dead sound
                audioSource.PlayOneShot(deadSound);
                //Find Game Manager and put an end to this
                FindObjectOfType<GameManager>().GameOver();
            }
            else
            {
                Debug.Log("Health is now: " + health);
                //Play hurt sound
                audioSource.PlayOneShot(hurtSound);
                if (health <= 50)
                {
                    loopAudio.Play();
                    var color = hurtImg.GetComponent<Image>().color;
                    color.a = 0.5f;
                    hurtImg.GetComponent<Image>().color = color;
                }
            }
        }

        public void Damage(float damage)
        {
            Debug.Log("Got hit with " + damage + " damage points");
            SubtractHealth(damage);
        }

        public void Score(int score)
        {
            this.score += score;
            scoreText.text = this.score.ToString();
        }
    }
}