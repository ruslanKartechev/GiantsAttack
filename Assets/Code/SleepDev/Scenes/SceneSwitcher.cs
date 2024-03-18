using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace SleepDev.Scenes
{
    public class SceneSwitcher : MonoBehaviour, ISceneSwitcher
    {
        private Queue<string> _loadedScenes = new Queue<string>();
        

        public async void OpenSceneAdditive(string name, Action<bool> onLoaded)
        {
            var process = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            process.allowSceneActivation = true;
            while (process.isDone == false && Application.isPlaying)
            {
                await Task.Yield();
            }
            _loadedScenes.Enqueue(name);
            onLoaded.Invoke(true);
        }

        public async void OpenScene(string name, Action<bool> onLoaded)
        {
            var process = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
            process.allowSceneActivation = true;
            while (process.isDone == false && Application.isPlaying)
            {
                await Task.Yield();
            }
            _loadedScenes.Enqueue(name);
            onLoaded.Invoke(true);   
        }

        public void ReloadCurrent()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ClosePrevAdditiveScene()
        {
            if(_loadedScenes.Count > 0)
                SceneManager.UnloadSceneAsync(_loadedScenes.Dequeue());
        }
        
        public void CloseAllPrevAdditiveScene()
        {
            
            while(_loadedScenes.Count > 0)
                SceneManager.UnloadSceneAsync(_loadedScenes.Dequeue());
        }
    }
}