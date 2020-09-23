using UnityEngine;


namespace AIBehaviorExamples
{
	public class PlayerStats : MonoBehaviour
	{
        private AudioSource _audioSource;
		public float health = 100.0f;
		[SerializeField] AudioClip hurtSound;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
		}

		public void SubtractHealth(float amount)
		{
			health -= amount;

			if ( health <= 0.0f )
			{
				health = 0.0f;
				Debug.LogWarning("You're Dead!");
			}
			else
			{
				Debug.Log("Health is now: " + health);
				//Play hurt sound
				_audioSource.PlayOneShot(hurtSound);
			}
		}

        public void Damage(float damage)
		{
			Debug.Log("Got hit with " + damage + " damage points");
			SubtractHealth(damage);
		}
	}
}