using UnityEngine;
//Destroys Self After A Set Amount Of Time.
namespace AtlyssHelperUtils
{
    public class TimedDestroyBehaviour : MonoBehaviour
    {
        [Header("Condition Object Display")]
        [SerializeField]
        public float TimeToDelete = 1f;
        private float currentTime = 0f;
        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= TimeToDelete)
                Destroy(gameObject);
        }
    }
}