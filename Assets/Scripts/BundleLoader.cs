using UnityEngine;
using System.IO;
using System.Collections;

public class BundleLoader : MonoBehaviour
{
    // Nama file .unity3d yang kamu taruh di folder StreamingAssets
    public string bundleName = "modelku.unity3d"; 

    void Start()
    {
        StartCoroutine(LoadAllAssetsFromBundle());
    }

    IEnumerator LoadAllAssetsFromBundle()
    {
        // Mencari lokasi file .unity3d di dalam memori penyimpanan Android
        string filePath = Path.Combine(Application.streamingAssetsPath, bundleName);

        // Memuat AssetBundle ke dalam memori aplikasi
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(filePath);
        yield return bundleRequest;

        AssetBundle localBundle = bundleRequest.assetBundle;

        if (localBundle == null)
        {
            Debug.LogError("Nyxen Error: Gagal memuat file .unity3d! Periksa nama file kamu.");
            yield break;
        }

        // KUNCI UTAMA: Memuat SELESAI/SEMUA objek GameObjects (Game Model) yang ada di dalam file .unity3d
        AssetBundleRequest assetRequest = localBundle.LoadAllAssetsAsync<GameObject>();
        yield return assetRequest;

        // Memunculkan semua objek yang berhasil diekstrak ke layar HP
        foreach (Object obj in assetRequest.allAssets)
        {
            GameObject prefab = obj as GameObject;
            if (prefab != null)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity);
                Debug.Log("Nyxen Berhasil Memuat Aset: " + prefab.name);
            }
        }

        // Lepas memori bundle agar HP tidak lag/patah-patah
        localBundle.Unload(false);
    }
}
