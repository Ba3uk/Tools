using UnityEngine;

namespace HardTutor
{
    public class HardTutorBehaviour : MonoBehaviour
    {
        [field: SerializeField] public string Id { get; set; }

        private void OnValidate()
        {
            gameObject.tag = "HardTutor";
        }
    }
}