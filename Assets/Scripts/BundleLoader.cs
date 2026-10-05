using UnityEngine;
using System.IO;
using System.Collections;
using System.Threading.Tasks;

public class BundleLoader : MonoBehaviour
{
    private GameObject currentSpawnedObject = null;

    // Fungsi bawaan Unity untuk menampilkan tombol UI di layar secara instan
    void OnGUI()
    {
        // Membuat tombol besar di tengah atas layar HP
        if (GUI.Button(new Rect(Screen.width / 2 - 150, 50, 300, 100), "Pilih File .unity3d"))
        {
            OpenFilePicker();
        }
    }

    // Fungsi untuk membuka penyimpanan Android
    private void OpenFilePicker()
    {
        // Menentukan format file yang boleh dipilih (.unity3d atau semua file)
        string[] fileTypes = new string[] { "*/*" }; 

        // Membuka file picker bawaan Android
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("Pemilihan file dibatalkan.");
                return;
            }

            Debug.Log("File dipilih: " + path);
            // Mulai memuat file yang dipilih dari memori HP
            StartCoroutine(LoadRequestedBundle(path));
        }, fileTypes);

        Debug.Log("Status Izin Penyimpanan: " + permission);
    }

    IEnumerator LoadRequestedBundle(string absolutePath)
    {
        // Hapus objek lama yang sedang tampil sebelum memuat objek baru
        if (currentSpawnedObject != null)
        {
            Destroy(currentSpawnedObject);
        }

        // Memuat file AssetBundle dari lokasi penyimpanan HP yang dipilih
        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(absolutePath);
        yield return bundleRequest;

        AssetBundle localBundle = bundleRequest.assetBundle;

        if (localBundle == null)
        {
            Debug.LogError("Nyxen Error: Gagal membaca file .unity3d. Pastikan filenya valid!");
            yield break;
        }

        // Memuat semua GameObjects di dalam file bundle
        AssetBundleRequest assetRequest = localBundle.LoadAllAssetsAsync<GameObject>();
        yield return assetRequest;

        foreach (Object obj in assetRequest.allAssets)
        {
            GameObject prefab = obj as GameObject;
            if (prefab != null)
            {
                // Memunculkan objek di tengah layar
                currentSpawnedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                Debug.Log("Nyxen Sukses Merender: " + prefab.name);
            }
        }

        // Unload asset bundle untuk menghemat RAM Android
        localBundle.Unload(false);
    }
}
