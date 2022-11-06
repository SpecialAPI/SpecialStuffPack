using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Feedback
{
    [HarmonyPatch]
    public class FeedbackCore : MonoBehaviour
    {
        [HarmonyPatch(typeof(ETGModGUI), nameof(ETGModGUI.CurrentMenuInstance), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool FeedbackMenu(ref IETGModMenu __result)
        {
            if(ETGModGUI.CurrentMenu == MenuOpenedE.FeedbackForm)
            {
                __result = (IETGModMenu)form ?? ETGModGUI.NullMenu;
                return false;
            }
            return true;
        }

        public void Awake()
        {
            var group = ETGModConsole.Commands.GetGroup(SpecialStuffModule.globalPrefix);
            group.AddGroup("feedback", x =>
            {
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback commands:").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"Use \"{SpecialStuffModule.globalPrefix} feedback bugreport\" for reporting this mod's bugs.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"Please do not use \"{SpecialStuffModule.globalPrefix} feedback bugreport\" for everything that is not reporting this mod's bugs, especially reporting bugs of other mods.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log("");
                ETGModConsole.Log($"Use \"{SpecialStuffModule.globalPrefix} feedback feedback\" for sharing your non-bug related feedback.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"Please do not use \"{SpecialStuffModule.globalPrefix} feedback feedback\" for everything that is not sharing your non-bug related feedback.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log("");
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback bugreport name - sets the name of your bug report.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback bugreport description - sets the description of your bug report.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback bugreport priority {"{low/medium/high}"} - sets the priority of your bug report (default - low).").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback bugreport submit - submits your bug report.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log("");
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback feedback name - sets the name of your feedback.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback feedback description - sets the description of your feedback.").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback feedback priority {"{low/medium/high}"} - sets the priority of your feedback (default - low).").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{SpecialStuffModule.globalPrefix} feedback feedback submit - submits your feedback.").Foreground = SpecialStuffModule.LogColor;
            });
            var fb = group.GetGroup("feedback");
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback", "The group used for in-game bug reporting/feedback. Just using it as a command will show help on how to use the commands.");

            fb.AddGroup("bugreport");
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback bugreport", "The group used for in-game bug reporting specifically.");
            var bugreport = fb.GetGroup("bugreport");
            bugreport.AddUnit("name", x =>
            {
                if (x.Length <= 0)
                {
                    ETGModConsole.Log("Name not given!").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                if (BugReportCurrentSubmitTimer != null)
                {
                    StopCoroutine(BugReportCurrentSubmitTimer);
                    BugReportCurrentSubmitTimer = null;
                }
                BugReportAreYouSure = false;
                BugReportName = string.Join(" ", x);
                ETGModConsole.Log($"Set bug report name to \"{BugReportName}\"").Foreground = SpecialStuffModule.LogColor;
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback bugreport name", "Sets the name of the bug report.");
            bugreport.AddUnit("description", x =>
            {
                if (x.Length <= 0)
                {
                    ETGModConsole.Log("Descrpition not given!").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                if (BugReportCurrentSubmitTimer != null)
                {
                    StopCoroutine(BugReportCurrentSubmitTimer);
                    BugReportCurrentSubmitTimer = null;
                }
                BugReportAreYouSure = false;
                BugReportDescription = string.Join(" ", x);
                ETGModConsole.Log($"Set bug report description to \"{BugReportDescription}\"").Foreground = SpecialStuffModule.LogColor;
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback bugreport description", "Sets the description of the bug report.");
            bugreport.AddUnit("priority", x =>
            {
                if(x.Length <= 0)
                {
                    ETGModConsole.Log("Priority not given!").Foreground = SpecialStuffModule.LogColor;
                }
                if(x[0] == "low")
                {
                    BugReportPriority = Label.LowPriority;
                }
                else if(x[0] == "medium")
                {
                    BugReportPriority = Label.MediumPriority;
                }
                else if(x[0] == "high")
                {
                    BugReportPriority = Label.HighPriority;
                }
                else
                {
                    ETGModConsole.Log($"Invalid priority \"{x[0]}\"!").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                ETGModConsole.Log($"Set bug report priority to {GetPriorityString(BugReportPriority)}").Foreground = SpecialStuffModule.LogColor;
            }, ETGModConsole.AutocompletionFromCollection(new List<string>() { "low", "medium", "high" }));
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback bugreport priority", "Sets the priority of the bug report. Default - Low Priority.");
            bugreport.AddGroup("submit", x =>
            {
                if (string.IsNullOrEmpty(BugReportName) || string.IsNullOrEmpty(BugReportDescription))
                {
                    ETGModConsole.Log("Your bug report is currently unfinished:").Foreground = SpecialStuffModule.LogColor;
                    if (string.IsNullOrEmpty(BugReportName))
                    {
                        ETGModConsole.Log("Bug report name not set!").Foreground = SpecialStuffModule.LogColor;
                    }
                    if (string.IsNullOrEmpty(BugReportDescription))
                    {
                        ETGModConsole.Log("Bug report description not set!").Foreground = SpecialStuffModule.LogColor;
                    }
                    ETGModConsole.Log("Your unfinished bug report looks like this:").Foreground = SpecialStuffModule.LogColor;
                    if (!string.IsNullOrEmpty(BugReportName))
                    {
                        ETGModConsole.Log($"\"{BugReportName}\"").Foreground = SpecialStuffModule.LogColor;
                    }
                    if (!string.IsNullOrEmpty(BugReportDescription))
                    {
                        ETGModConsole.Log($"\"{BugReportDescription}\"").Foreground = SpecialStuffModule.LogColor;
                    }
                    ETGModConsole.Log($"{GetPriorityString(BugReportPriority)}").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                ETGModConsole.Log("Your bug report currently looks like this:").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"\"{BugReportName}\"").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"\"{BugReportDescription}\"").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{GetPriorityString(BugReportPriority)}").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"If you are absolutely sure you want to submit this bug report, use the command \"{SpecialStuffModule.globalPrefix} feedback bugreport submit yesimsure\". Your submit request will timeout in " +
                    $"{BugReportTimeBeforeTimeout} seconds, clearing the name and description and disabling the \"{SpecialStuffModule.globalPrefix} feedback bugreport submit yesimsure\" command.").Foreground = SpecialStuffModule.LogColor;
                BugReportAreYouSure = true;
                BugReportCurrentSubmitTimer = StartCoroutine(ClearBugReport());
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback bugreport submit", $"Asks for confirmation to submit the bug report, if confirmation is not given in {BugReportTimeBeforeTimeout} seconds the bug " +
                $"report is cleared and the {SpecialStuffModule.globalPrefix} feedback bugreport submit yesimsure command is disabled.");
            bugreport.GetGroup("submit").AddUnit("yesimsure", x =>
            {
                if (BugReportAreYouSure)
                {
                    if (BugReportCurrentSubmitTimer != null)
                    {
                        StopCoroutine(BugReportCurrentSubmitTimer);
                        BugReportCurrentSubmitTimer = null;
                    }
                    BugReportAreYouSure = false;
                    TrelloAPI.PostToTrello(BugReportName ?? "[null]", BugReportDescription ?? "[null]", FeedbackType.BugReport, new() { BugReportPriority });
                    BugReportName = null;
                    BugReportDescription = null;
                    ETGModConsole.Log("Bug report successfully submitted!").Foreground = SpecialStuffModule.LogColor;
                }
                else
                {
                    ETGModConsole.Log("No bug report submit currently pending.").Foreground = SpecialStuffModule.LogColor;
                }
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback bugreport submit yesimsure", $"Submits the bug report. Can only be used after the {SpecialStuffModule.globalPrefix} feedback bugreport submit" +
                $" command has been used and {BugReportTimeBeforeTimeout} seconds haven't passed since then.");

            fb.AddGroup("feedback");
            var feedback = fb.GetGroup("feedback");
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback feedback", "The group used for in-game feedback specifically.");
            feedback.AddUnit("name", x =>
            {
                if (x.Length <= 0)
                {
                    ETGModConsole.Log("Name not given!").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                if (FeedbackCurrentSubmitTimer != null)
                {
                    StopCoroutine(FeedbackCurrentSubmitTimer);
                    FeedbackCurrentSubmitTimer = null;
                }
                FeedbackAreYouSure = false;
                FeedbackName = string.Join(" ", x);
                ETGModConsole.Log($"Set feedback name to \"{FeedbackName}\"").Foreground = SpecialStuffModule.LogColor;
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback feedback name", "Sets the name of the feedback.");
            feedback.AddUnit("description", x =>
            {
                if (x.Length <= 0)
                {
                    ETGModConsole.Log("Descrpition not given!").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                if (FeedbackCurrentSubmitTimer != null)
                {
                    StopCoroutine(FeedbackCurrentSubmitTimer);
                    FeedbackCurrentSubmitTimer = null;
                }
                FeedbackAreYouSure = false;
                FeedbackDescription = string.Join(" ", x);
                ETGModConsole.Log($"Set feedback description to \"{FeedbackDescription}\"").Foreground = SpecialStuffModule.LogColor;
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback feedback description", "Sets the description of the feedback.");
            feedback.AddUnit("priority", x =>
            {
                if (x.Length <= 0)
                {
                    ETGModConsole.Log("Priority not given!").Foreground = SpecialStuffModule.LogColor;
                }
                if (x[0] == "low")
                {
                    FeedbackPriority = Label.LowPriority;
                }
                else if (x[0] == "medium")
                {
                    FeedbackPriority = Label.MediumPriority;
                }
                else if (x[0] == "high")
                {
                    FeedbackPriority = Label.HighPriority;
                }
                else
                {
                    ETGModConsole.Log($"Invalid priority \"{x[0]}\"!").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                ETGModConsole.Log($"Set feedback priority to {GetPriorityString(FeedbackPriority)}").Foreground = SpecialStuffModule.LogColor;
            }, ETGModConsole.AutocompletionFromCollection(new List<string>() { "low", "medium", "high" }));
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback feedback priority", "Sets the priority of the feedback. Default - Low Priority");
            feedback.AddGroup("submit", x =>
            {
                if (string.IsNullOrEmpty(FeedbackName) || string.IsNullOrEmpty(FeedbackDescription))
                {
                    ETGModConsole.Log("Your feedback is currently unfinished:").Foreground = SpecialStuffModule.LogColor;
                    if (string.IsNullOrEmpty(FeedbackName))
                    {
                        ETGModConsole.Log("Feedback name not set!").Foreground = SpecialStuffModule.LogColor;
                    }
                    if (string.IsNullOrEmpty(FeedbackDescription))
                    {
                        ETGModConsole.Log("Feedback description not set!").Foreground = SpecialStuffModule.LogColor;
                    }
                    ETGModConsole.Log("Your unfinished feedback looks like this:").Foreground = SpecialStuffModule.LogColor;
                    if (!string.IsNullOrEmpty(FeedbackName))
                    {
                        ETGModConsole.Log($"\"{FeedbackName}\"").Foreground = SpecialStuffModule.LogColor;
                    }
                    if (!string.IsNullOrEmpty(FeedbackDescription))
                    {
                        ETGModConsole.Log($"\"{FeedbackDescription}\"").Foreground = SpecialStuffModule.LogColor;
                    }
                    ETGModConsole.Log($"{GetPriorityString(FeedbackPriority)}").Foreground = SpecialStuffModule.LogColor;
                    return;
                }
                ETGModConsole.Log("Your feedback currently looks like this:").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"\"{FeedbackName}\"").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"\"{FeedbackDescription}\"").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"{GetPriorityString(FeedbackPriority)}").Foreground = SpecialStuffModule.LogColor;
                ETGModConsole.Log($"If you are absolutely sure you want to submit this feedback, use the command \"{SpecialStuffModule.globalPrefix} feedback feedback submit yesimsure\". Your submit request will timeout in " +
                    $"{FeedbackTimeBeforeTimeout} seconds, clearing the name and description and disabling the \"{SpecialStuffModule.globalPrefix} feedback feedback submit yesimsure\" command.").Foreground = SpecialStuffModule.LogColor;
                FeedbackAreYouSure = true;
                FeedbackCurrentSubmitTimer = StartCoroutine(ClearFeedback());
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback feedback submit", $"Asks for confirmation to submit the feedback, if confirmation is not given in {FeedbackTimeBeforeTimeout} seconds the " +
                $"feedback is cleared and the {SpecialStuffModule.globalPrefix} feedback feedback submit yesimsure command is disabled.");
            feedback.GetGroup("submit").AddUnit("yesimsure", x =>
            {
                if (FeedbackAreYouSure)
                {
                    if (FeedbackCurrentSubmitTimer != null)
                    {
                        StopCoroutine(FeedbackCurrentSubmitTimer);
                        FeedbackCurrentSubmitTimer = null;
                    }
                    FeedbackAreYouSure = false;
                    TrelloAPI.PostToTrello(FeedbackName ?? "[null]", FeedbackDescription ?? "[null]", FeedbackType.Feedback, new() { FeedbackPriority });
                    FeedbackName = null;
                    FeedbackDescription = null;
                    ETGModConsole.Log("Feedback successfully submitted!").Foreground = SpecialStuffModule.LogColor;
                }
                else
                {
                    ETGModConsole.Log("No feedback submit currently pending.").Foreground = SpecialStuffModule.LogColor;
                }
            });
            ETGModConsole.CommandDescriptions.Add($"{SpecialStuffModule.globalPrefix} feedback feedback submit yesimsure", $"Submits the feedback. Can only be used after the {SpecialStuffModule.globalPrefix} feedback feedback submit" +
                $" command has been used and {FeedbackTimeBeforeTimeout} seconds haven't passed since then.");
            form = new();
            form.Setup();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (ETGModGUI.CurrentMenu == MenuOpenedE.FeedbackForm)
                {
                    ETGModGUI.CurrentMenu = ETGModGUI.MenuOpened.None;
                }
                else
                {
                    ETGModGUI.CurrentMenu = MenuOpenedE.FeedbackForm;
                }

                ETGModGUI.UpdateTimeScale();
                ETGModGUI.UpdatePlayerState();
            }
        }

        public static IEnumerator ClearBugReport()
        {
            float dura = BugReportTimeBeforeTimeout;
            float ela = 0f;
            while (ela < dura)
            {
                if (!BugReportDontCountTimeUnfocused || Application.isFocused)
                {
                    ela += Time.unscaledDeltaTime;
                }
                yield return null;
            }
            ETGModConsole.Log("Bug report submit timed out, name and description are cleared.").Foreground = SpecialStuffModule.LogColor;
            BugReportName = null;
            BugReportDescription = null;
            BugReportAreYouSure = false;
            BugReportPriority = Label.LowPriority;
            yield break;
        }

        public static IEnumerator ClearFeedback()
        {
            float dura = FeedbackTimeBeforeTimeout;
            float ela = 0f;
            while (ela < dura)
            {
                if (!FeedbackDontCountTimeUnfocused || Application.isFocused)
                {
                    ela += Time.unscaledDeltaTime;
                }
                yield return null;
            }
            ETGModConsole.Log("Feedback submit timed out, name and description are cleared.").Foreground = SpecialStuffModule.LogColor;
            FeedbackName = null;
            FeedbackDescription = null;
            FeedbackAreYouSure = false;
            FeedbackPriority = Label.LowPriority;
            yield break;
        }

        public static string GetPriorityString(Label priority)
        {
            if (priority == Label.LowPriority)
            {
                return "Low Priority";
            }
            if (priority == Label.MediumPriority)
            {
                return "Medium Priority";
            }
            if (priority == Label.HighPriority)
            {
                return "High Priority";
            }
            return "";
        }

        public static readonly float BugReportTimeBeforeTimeout = 30f;
        public static readonly bool BugReportDontCountTimeUnfocused = true;
        public static string BugReportName;
        public static string BugReportDescription;
        public static Label BugReportPriority;
        public static bool BugReportAreYouSure;
        public static Coroutine BugReportCurrentSubmitTimer;

        public static readonly float FeedbackTimeBeforeTimeout = 30f;
        public static readonly bool FeedbackDontCountTimeUnfocused = true;
        public static string FeedbackName;
        public static string FeedbackDescription;
        public static Label FeedbackPriority;
        public static bool FeedbackAreYouSure;
        public static Coroutine FeedbackCurrentSubmitTimer;

        public static FeedbackForm form;
    }
}
