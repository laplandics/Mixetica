using System.Collections;
using R3;
using Space;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Boot
{
    public class Boot
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() => _ = new Boot();

        private Boot()
        {
            G.Register(new UI());
            G.Register(new Inputs());
            G.Register(new Scenes());
            G.Register(new Services());
            G.Register(new Coroutines());
            
            G.Resolve<Coroutines>().Start(LoadProject());
        }
        
        private IEnumerator LoadProject()
        {
            yield return Resources.UnloadUnusedAssets();
            
            yield return G.Resolve<Services>().OnProjectBeginLoad();
            
            yield return G.Resolve<Scenes>().ToBoot();
            
            yield return G.Resolve<Services>().OnProjectEndLoad();
            G.Resolve<Coroutines>().Start(LoadScene("Game"));
        }
        
        private IEnumerator LoadScene(string sceneName)
        {
            yield return G.Resolve<Services>().OnSceneBeginLoad();
            
            yield return G.Resolve<Scenes>().ToScene(sceneName);
            
            var sceneBoot = Object.FindAnyObjectByType<SceneBoot>();
            if (sceneBoot == null) { Debug.LogWarning(NoSceneBootWarning); yield break; }

            var exitSubject = new Subject<string>();
            exitSubject.Subscribe(next => G.Resolve<Coroutines>().Start(UnloadScene(next)));
            
            yield return G.Resolve<Services>().OnSceneEndLoad();
            yield return null;
            
            yield return G.Resolve<Services>().OnSceneBeginBoot();
            yield return G.Resolve<Coroutines>().Start(sceneBoot.Boot(exitSubject));
            yield return G.Resolve<Services>().OnSceneEndBoot();
        }
        
        private IEnumerator UnloadScene(string nextScene)
        {
            yield return Resources.UnloadUnusedAssets();
            
            yield return G.Resolve<Services>().OnSceneBeginUnload();
            
            yield return G.Resolve<Scenes>().ToBoot();
            
            yield return G.Resolve<Services>().OnSceneEndUnload();
            G.Resolve<Coroutines>().Start(LoadScene(nextScene));
        }
        
        private string NoSceneBootWarning => $"Couldn't find SceneBoot on this scene: {SceneManager.GetActiveScene().name}";
    }
}