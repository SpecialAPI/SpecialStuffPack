using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace SpecialStuffPack.Feedback
{
    public static class TrelloAPI
    {
        public static void PostToTrello(string name, string description, FeedbackType type, List<Label> labels)
        {
            WWWForm www = new();
            www.AddField("key", "0c27d3ce204b5334c3fb854496f19098");
            using (var strem = Assembly.GetExecutingAssembly().GetManifestResourceStream("SpecialStuffPack.GITIGNORE.TrelloToken.txt"))
            {
                byte[] ba = new byte[strem.Length];
                strem.Read(ba, 0, ba.Length);
                www.AddField("token", Encoding.UTF8.GetString(ba));
            }
            www.AddField("name", name);
            if (description.Length > 16384)
            {
                description = description.Remove(16383);
            }
            www.AddField("desc", description);
            www.AddField("idLabels", string.Join(",", labels.Select(x => x == Label.LowPriority ? "63652d7effc44e0170d72252" : x == Label.MediumPriority ? "63652d8a2e589505d5fa7da6" : "63652d9c0c2b1100d733aef1").ToArray()));
            www.AddField("idList", type == FeedbackType.BugReport ? "6364049ed8f8d700990b7913" : "6364049a923e3e02a7b13c85");
            var filesToAttach = new List<Tuple<string, byte[]>>();
            if (type == FeedbackType.BugReport)
            {
                filesToAttach.Add(new("OutputLog.txt", Encoding.UTF8.GetBytes(EraseUserName(string.Join("\n", LogRecorder.lines.ToArray())))));
                var savedataPath = Path.Combine(SaveManager.SavePath, string.Format(SaveAPIManager.AdvancedGameSave.filePattern, SaveManager.CurrentSaveSlot));
                if (File.Exists(savedataPath))
                {
                    filesToAttach.Add(new("SpecialSaveData.txt", File.ReadAllBytes(savedataPath)));
                }
            }
            ETGMod.StartGlobalCoroutine(SendTrelloWebRequest(www, filesToAttach.ToArray()));
        }

        public static IEnumerator SendTrelloWebRequest(WWWForm form, Tuple<string, byte[]>[] filesToAttach)
        {
            var webrequest = UnityWebRequest.Post("https://api.trello.com/1/cards", form);
            var operation = webrequest.SendWebRequest();
            while (!operation.isDone)
            {
                yield return null;
            }
            if (webrequest.isHttpError || webrequest.isNetworkError)
            {
                Debug.LogError("ERROR: Failed sending feedback: " + webrequest.error);
                ETGModConsole.Log("ERROR: Failed sending feedback: " + webrequest.error).Foreground = SpecialStuffModule.LogColor;
            }
            else
            {
                var response = JsonUtility.FromJson<TrelloCardResponse>(webrequest.downloadHandler.text);
                foreach (var file in filesToAttach)
                {
                    yield return AddTrelloAttachment(response.id, file.Second, file.First);
                }
            }
            yield break;
        }

        public static IEnumerator AddTrelloAttachment(string cardID, byte[] file, string name)
        {
            WWWForm www = new();
            if (file != null)
            {
                www.AddBinaryData("file", file, name ?? "unknown.file");
            }
            if (name != null)
            {
                www.AddField("name", name);
            }
            string d = "?";
            if (cardID.Contains("?"))
            {
                d = "&";
            }
            string uri = "";
            using (var strem = Assembly.GetExecutingAssembly().GetManifestResourceStream("SpecialStuffPack.GITIGNORE.TrelloToken.txt"))
            {
                byte[] ba = new byte[strem.Length];
                strem.Read(ba, 0, ba.Length);
                uri = $"https://trello.com/1/cards/{cardID}/attachments{d}key=0c27d3ce204b5334c3fb854496f19098&token={Encoding.UTF8.GetString(ba)}";
            }
            var webrequest = UnityWebRequest.Post(uri, www);
            var operation = webrequest.SendWebRequest();
            while (!operation.isDone)
            {
                yield return null;
            }
            if (webrequest.isHttpError || webrequest.isNetworkError)
            {
                Debug.LogError($"ERROR: Failed attaching file {name}: " + webrequest.error);
                ETGModConsole.Log($"ERROR: Failed sending feedback {name}: " + webrequest.error).Foreground = SpecialStuffModule.LogColor;
            }
            yield break;
        }
    }
}
