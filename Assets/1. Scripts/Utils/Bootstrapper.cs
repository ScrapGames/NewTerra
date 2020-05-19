using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Buildings;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class Bootstrapper : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private GameObject logo, controls;
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private AssetReference scene;

    private void Start()
    {
        LoadDatabasesAsync();
    }


    private async void LoadDatabasesAsync()
    {
        // Load Databases - this will only be called when loading the game        
        await DataManager.Instance.LoadAllDatabasesAsync();

        // Change scene        
        var operation = Addressables.LoadSceneAsync(scene, activateOnLoad: false);

        await operation.Task;
        var sceneResult = operation.Task.Result;



        Debug.Log("Scene Loaded!");
        logo.SetActive(false);
        controls.SetActive(true);


        button.onClick.AddListener(() =>
        {
            // Set as active scene
            sceneResult.Activate();

            GameManager.Instance.gameObject.AddComponent<UIManager>();

            // Remove bootstrapper            
            Destroy(this);
        });

    }
}
