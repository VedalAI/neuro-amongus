using System;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp]
public sealed class Uploader : MonoBehaviour
{
    public const string FILE_SERVER_URL = "http://amongus.neurosama.com";

    public static Uploader Instance { get; private set; }

    public Uploader(IntPtr ptr) : base(ptr)
    {
    }

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
    }

    [HideFromIl2Cpp]
    public void SendFileToServer(string fileName, byte[] fileBytes)
    {
        this.StartCoroutine(CoSendFileToServer(fileName, fileBytes));
    }

    [HideFromIl2Cpp]
    private IEnumerator CoSendFileToServer(string fileName, byte[] fileBytes)
    {
        HttpClient client = new();

        client.DefaultRequestHeaders.Add("User-Agent", "Neuro/1.0");

        // send file as multipart form data
        MultipartFormDataContent form = new();
        form.Add(new ByteArrayContent(fileBytes), "file", fileName);

        Task<HttpResponseMessage> task = client.PostAsync(FILE_SERVER_URL, form);
        yield return new WaitForTask(task);

        if (task.IsCompletedSuccessfully)
        {
            Info("Data file sent to the server.");
            Info("Server returned status code: " + task.Result.StatusCode);
        }
        else
        {
            Error("Could not send data file to the server.'");
        }
    }
}
