using System;
using System.IO;
//using System.Collections.Generic;
using System.Security.Cryptography;  
using Google.Protobuf;
using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Neuro.Recording;

// TODO: ReportFindings was removed, we need to implement separate communication with language model
[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    public Recorder(IntPtr ptr) : base(ptr) { }

    private int _fixedUpdateCalls;
    private FileStream _fileStream;

    public string fileServerURL = "http://localhost:5000/";

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

    private void Start()
    {
        string recordingsDirectory = Path.Combine(BepInEx.Paths.PluginPath, "NeuroRecordings");
        if (!Directory.Exists(recordingsDirectory)) Directory.CreateDirectory(recordingsDirectory);
        _fileStream = new FileStream(Path.Combine(recordingsDirectory, $"{DateTime.Now.ToFileTime()}.gymbag2"), FileMode.Create);

        WriteAndFlush(Frame.Now(true));
    }

    private void FixedUpdate()
    {
        // TODO: We should record meeting data!
        if (MeetingHud.Instance || Minigame.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;

        if (CommunicationHandler.Instance && CommunicationHandler.Instance.IsConnected)
        {
            Warning("Connected to socket, stopping Recorder");
            Destroy(this);
            return;
        }

        // TODO: Record local impostor data: kill cooldown, venting stuff, etc
        // TODO: Record local player interactions data: opened task, opened door

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 5) return;
        _fixedUpdateCalls = 0;

        WriteAndFlush(Frame.Now());
    }

    private void OnDestroy()
    {
        _fileStream.Dispose();

        // Send file to server
        AmongUsClient.Instance.StartCoroutine(SendFileToServer());
    }

    IEnumerator SendFileToServer()
    {
        HttpClient client = new();

        // Get request
        Task<HttpResponseMessage> getTask = client.GetAsync(fileServerURL);
        while (!getTask.IsCompleted && !getTask.IsCanceled && !getTask.IsFaulted && !getTask.IsCompletedSuccessfully) yield return null;
        if (!getTask.IsCompletedSuccessfully)
        {
            Info("Could not fetch token.");
            yield break;
        }

        // Get token
        Task<string> result = getTask.Result.Content.ReadAsStringAsync();
        while (!result.IsCompleted && !result.IsCanceled && !result.IsFaulted && !result.IsCompletedSuccessfully) yield return null;
        string token = result.Result;
        Info(token);

        // SHA256 hash of token
        SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

        byte[] fileBytes = File.ReadAllBytes(_fileStream.Name);

        // add hash to header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", hashString);

        // send file as multipart form data
        MultipartFormDataContent form = new();

        // Get file name from _fileStream.Name
        string fileName = Path.GetFileName(_fileStream.Name);

        form.Add(new ByteArrayContent(fileBytes), "file", fileName);
        
        Task<HttpResponseMessage> task = client.PostAsync(fileServerURL, form);
        while (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted && !task.IsCompletedSuccessfully) yield return null;
        if (!task.IsCompletedSuccessfully)
        {
            Info("Could not send logs to the server.'");
        }  else {
            Info("Logs sent to the server.");
        }
    }

    [HideFromIl2Cpp]
    private void WriteAndFlush(IMessage message)
    {
        _fileStream.Write(BitConverter.GetBytes(message.CalculateSize()), 0, 4);
        message.WriteTo(_fileStream);
        // Warning($"Recorded: {message}");
        _fileStream.Flush();
    }
}
