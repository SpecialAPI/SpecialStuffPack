using SGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack.Feedback
{
    public class FeedbackForm : ETGModMenu
    {
        public override void Start() { }

        public void Setup()
        {
            GUI = new()
            {
                Visible = false,
                OnUpdateStyle = x => x.Fill(),
                Children =
                {
                    new SButton() //0
                    {
                        Text = "Name (click to focus on the text field)",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = 0f;
                            x.Position.x = 0f;
                        },
                        Background = Color.clear,
                        OnClick = x => { nameField.Focus(); AkSoundEngine.PostEvent("EverhoodMenuConfirm", GameManager.Instance.gameObject); }
                    },
                    (nameField = new() //1
                    {
                        OnUpdateStyle = x =>
                        {
                            x.Size.x = x.Parent.InnerSize.x;
                            x.Position.x = 0f;
                            x.Position.y = x.Parent.Children[0].Size.y;//elem.Parent.InnerSize.y - elem.Size.y;
                        },
                    }),
                    new SButton() //2
                    {
                        Text = "Description (click to focus on the text field)",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.Children[1].Position.y + x.Parent.Children[1].Size.y * 1.5f;
                            x.Position.x = 0f;
                        },
                        Background = Color.clear,
                        OnClick = x => { descriptionField.Focus(); AkSoundEngine.PostEvent("EverhoodMenuConfirm", GameManager.Instance.gameObject); }
                    },
                    (descriptionField = new STextField() //3
                    {
                        OnUpdateStyle = x =>
                        {
                            x.Position.x = 0f;
                            x.Position.y = x.Parent.Children[2].Position.y + x.Parent.Children[2].Size.y;
                            x.Size.x = x.Parent.InnerSize.x;
                        },
                    }),
                    new SButton() //4
                    {
                        Text = "Priority: Low",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.Children[3].Position.y + x.Parent.Children[3].Size.y * 1.5f;
                            x.Position.x = 0f;
                        },
                        Background = Color.clear,
                        OnClick = x =>
                        {
                            priority++;
                            if(priority > Label.HighPriority)
                            {
                                priority = Label.LowPriority;
                            }
                            x.Text = "Priority: " + priority switch
                            {
                                Label.LowPriority => "Low",
                                Label.MediumPriority => "Medium",
                                _ => "High"
                            };
                            AkSoundEngine.PostEvent("EverhoodMenuConfirm", GameManager.Instance.gameObject);
                        }
                    },
                    new SButton() //5
                    {
                        Text = "Bug Report",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.Children[3].Position.y + x.Parent.Children[3].Size.y * 1.5f;
                            x.Position.x = x.Parent.InnerSize.x / 2f - x.Size.x / 2f;
                        },
                        Background = Color.clear,
                        OnClick = x =>
                        {
                            if(mode == FeedbackType.BugReport)
                            {
                                mode = FeedbackType.Feedback;
                                x.Text = "Feedback";
                            }
                            else
                            {
                                mode = FeedbackType.BugReport;
                                x.Text = "Bug Report";
                            }
                            AkSoundEngine.PostEvent("EverhoodMenuConfirm", GameManager.Instance.gameObject);
                        }
                    },
                    new SButton() //6
                    {
                        Text = "Submit",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.InnerSize.y * 0.9f - x.Size.y;
                            x.Position.x = x.Parent.InnerSize.x * 0.9f - x.Size.x;
                        },
                        Background = Color.clear,
                        OnClick = x =>
                        {
                            if(!string.IsNullOrEmpty(nameField.Text) && !string.IsNullOrEmpty(descriptionField.Text))
                            {
                                TrelloAPI.PostToTrello(nameField.Text, descriptionField.Text, mode, new() { priority });
                                nameField.Text = "";
                                descriptionField.Text = "";
                                mode = FeedbackType.BugReport;
                                priority = Label.LowPriority;
                                (GUI.Children[9] as SLabel).Text = "";
                                (GUI.Children[8] as SLabel).Text = "";
                                (GUI.Children[4] as SButton).Text = "Priority: Low";
                                (GUI.Children[5] as SButton).Text = "Bug Report";
                                GUI.Visible = false;
                                AkSoundEngine.PostEvent("EverhoodSave", GameManager.Instance.gameObject);
                                ETGModGUI.UpdateTimeScale();
                                ETGModGUI.UpdatePlayerState();
                            }
                            else
                            {
                                if(string.IsNullOrEmpty(nameField.Text) && string.IsNullOrEmpty(descriptionField.Text))
                                {
                                    (GUI.Children[9] as SLabel).Text = "Name not given!";
                                    (GUI.Children[8] as SLabel).Text = "Description not given!";
                                }
                                else if (string.IsNullOrEmpty(nameField.Text))
                                {
                                    (GUI.Children[8] as SLabel).Text = "Name not given!";
                                    (GUI.Children[9] as SLabel).Text = "";
                                }
                                else
                                {
                                    (GUI.Children[8] as SLabel).Text = "Description not given!";
                                    (GUI.Children[9] as SLabel).Text = "";
                                }
                                AkSoundEngine.PostEvent("EverhoodUIError", GameManager.Instance.gameObject);
                            }
                        }
                    },
                    new SButton() //7
                    {
                        Text = "Cancel",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.InnerSize.y * 0.9f - x.Size.y;
                            x.Position.x = x.Parent.InnerSize.x * 0.9f - x.Size.x - x.Parent.Children[6].Size.x;
                        },
                        Background = Color.clear,
                        OnClick = x =>
                        {
                            nameField.Text = "";
                            descriptionField.Text = "";
                            mode = FeedbackType.BugReport;
                            priority = Label.LowPriority;
                            (GUI.Children[9] as SLabel).Text = "";
                            (GUI.Children[8] as SLabel).Text = "";
                            (GUI.Children[4] as SButton).Text = "Priority: Low";
                            (GUI.Children[5] as SButton).Text = "Bug Report";
                            AkSoundEngine.PostEvent("EverhoodMenuConfirm", GameManager.Instance.gameObject);
                            GUI.Visible = false;
                            ETGModGUI.UpdateTimeScale();
                            ETGModGUI.UpdatePlayerState();
                        }
                    },
                    new SLabel() //8
                    {
                        Text = "",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.InnerSize.y * 0.75f - x.Size.y - x.Parent.Children[6].Size.y;
                            x.Position.x = x.Parent.InnerSize.x * 0.75f - x.Size.x;
                        },
                        Background = Color.clear,
                        Foreground = Color.red
                    },
                    new SLabel() //9
                    {
                        Text = "",
                        OnUpdateStyle = x =>
                        {
                            x.Position.y = x.Parent.InnerSize.y * 0.75f - x.Size.y - x.Parent.Children[6].Size.y * 2;
                            x.Position.x = x.Parent.InnerSize.x * 0.75f - x.Size.x;
                        },
                        Background = Color.clear,
                        Foreground = Color.red
                    },
                }
            };
        }

        public override void OnClose()
        {
            GUI.Visible = false;
            base.OnClose();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            GUI.Visible = true;
        }

        public FeedbackType mode = FeedbackType.BugReport;
        public Label priority;
        public STextField nameField;
        public STextField descriptionField;
        public string textLastFrame;
    }
}
