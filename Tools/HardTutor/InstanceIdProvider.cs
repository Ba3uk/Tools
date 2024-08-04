using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HardTutor
{
    internal class InstanceIdProvider : IInstanceIdProvider
    {
        public List<int> GetInstance(List<string> ids)
        {
            var gameObjects = GameObject.FindGameObjectsWithTag("HardTutor");
            if (!gameObjects.Any())
            {
                Debug.LogError($"Can't find object for Hard Tutor instance with ids '{ids}'");
                return new List<int>() {0};
            }

            var instanceIds = new List<int>();
            foreach (var gameObject in gameObjects)
            {
                var hardTutorBehaviour = gameObject.GetComponent<HardTutorBehaviour>();
                if (ids.Contains(hardTutorBehaviour.Id))
                {
                    instanceIds.Add(gameObject.GetInstanceID());
                }
            }

            return instanceIds;
        }
    }
}