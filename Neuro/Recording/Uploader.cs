using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.Attributes;
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
        using HttpClient client = new();

        client.DefaultRequestHeaders.Add("User-Agent", "Neuro/1.0");

        // send file as multipart form data
        using MultipartFormDataContent form = new();
        form.Add(new ByteArrayContent(fileBytes), "file", fileName);

        Task<HttpResponseMessage> task = client.PostAsync(FILE_SERVER_URL, form);
        yield return new WaitForTask(task);

        if (task.IsCompletedSuccessfully && task.Result.IsSuccessStatusCode)
        {
            Info("Data file sent to the server.");
        }
        else
        {
            Error("Could not send data file to the server.'");
        }

        if (task.IsCompletedSuccessfully)
        {
            Info("Server returned: " + task.Result.ReasonPhrase + ", code: " + task.Result.StatusCode);
        }
    }

    private class WaitForTask : IEnumerator
    {
        private readonly Task _task;

        object IEnumerator.Current => null;

        public WaitForTask(Task task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
        }

        bool IEnumerator.MoveNext()
        {
            return !_task.IsCompleted;
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }
    }
}