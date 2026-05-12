using System.Collections;
using R3;
using UnityEngine;

namespace Boot
{
    public class SceneBoot : MonoBehaviour
    {
        public IEnumerator Boot(Subject<string> onExit)
        {
            yield return null;
        }
    }
}