using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace PlagueGUI
{
    public static class PlagueGUI
    {
        /// <summary>
        /// The DropDownState Instance, Holding The Main MetaData For The DropDown, Along With All The Internal Data For The Buttons, IsOpen, SearchText And ScrollPos
        /// </summary>
        public static Dictionary<DropDownData, DropDownInternals> DropDownState = new Dictionary<DropDownData, DropDownInternals>();

        /// <summary>
        /// Creates A GUI.DropDown Menu (Y'Know, That One Thing Unity Never Gave You) - Created By Plague
        /// </summary>
        /// <param name="PositionAndScale">The Position And Scale Of The DropDown Menu</param>
        /// <param name="MainButtonText">The Main Button For The DropDown's Text</param>
        /// <param name="Buttons">Your Buttons/Toggles For The DropDown, A List Of KeyValuePairs With Key Being A Tuple Of The Button Text, The ButtonType And Default Toggle State (If Toggle ButtonType) And Value Being A Delegate To Execute On Selection</param>
        /// <param name="ShowSearch">Whether To Show The Search Bar</param>
        /// <returns>The DropDownData Instance</returns>
        public static DropDownData DropDown(Rect PositionAndScale, string MainButtonText, List<KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>>> Buttons, bool ShowSearch = true, bool Sort = true)
        {
            if (string.IsNullOrEmpty(MainButtonText) || Buttons == null || Buttons.Count == 0)
            {
                return null;
            }

            if (Sort)
            {
                Buttons = Buttons.OrderBy(a => a.Key.Item1).ToList();
            }

            DropDownData DropData = null;

            List<DropDownData> DropDowns = DropDownState.Keys.Where(o => o.DropDownButtonText == MainButtonText).ToList();

            if (DropDowns.Count == 0)
            {
                DropData = new DropDownData()
                {
                    DropDownButtonText = MainButtonText,
                    ButtonsToShow = Buttons
                };
            }
            else
            {
                DropData = DropDowns[0];
            }

            if (!DropDownState.ContainsKey(DropData))
            {
                DropDownState[DropData] = new DropDownInternals();
            }

            if (DropDownState[DropData].IsOpen && ShowSearch)
            {
                DropDownState[DropData].SearchText = GUI.TextArea(new Rect(PositionAndScale.x, PositionAndScale.y + 25, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].SearchText);

                GUI.Label(new Rect(PositionAndScale.x, PositionAndScale.y + 25, PositionAndScale.width, PositionAndScale.height), new GUIContent("", "Search The DropDown List For An Item"));
            }

            if (GUI.Button(PositionAndScale, new GUIContent((DropDownState[DropData].IsOpen ? @"/\ " : @"\/ ") + MainButtonText + (DropDownState[DropData].IsOpen ? @" /\" : @" \/"), (DropDownState[DropData].IsOpen ? "Closes The DropDown" : "Expands The DropDown"))))
            {
                DropDownState[DropData].IsOpen = !DropDownState[DropData].IsOpen;
            }

            if (DropDownState[DropData].IsOpen)
            {
                DropDownState[DropData].ScrollPosition = GUI.BeginScrollView(new Rect(PositionAndScale.x, PositionAndScale.y + 50, PositionAndScale.width + 25, 500), DropDownState[DropData].ScrollPosition, new Rect(0, 0, PositionAndScale.width + 25, (PositionAndScale.height * (string.IsNullOrEmpty(DropDownState[DropData].SearchText) || DropDownState[DropData].SearchText == "Search" ? Buttons.Count : Buttons.Where(b => b.Key.Item1.ToLower().Contains(DropDownState[DropData].SearchText.ToLower())).ToList().Count))));

                int ButtonPosMultiplier = 0;

                int ControlID = 0;

                for (int i = 0; i < Buttons.Count; i++)
                {
                    KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>> Button = Buttons[i];

                    if (!DropDownState[DropData].StringCache.ContainsKey(ControlID))
                    {
                        DropDownState[DropData].StringCache[ControlID] = Button.Key.Item1;
                    }

                    if (!DropDownState[DropData].IntCache.ContainsKey(ControlID))
                    {
                        DropDownState[DropData].IntCache[ControlID] = 0;
                    }

                    if (!DropDownState[DropData].FloatCache.ContainsKey(ControlID))
                    {
                        DropDownState[DropData].FloatCache[ControlID] = 0f;
                    }

                    if (!DropDownState[DropData].BoolCache.ContainsKey(ControlID))
                    {
                        DropDownState[DropData].BoolCache[ControlID] = Button.Key.Item4;
                    }

                    void MakeButton()
                    {
                        switch (Button.Key.Item3)
                        {
                            case ButtonType.Button:
                                if (GUI.Button(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), new GUIContent(Button.Key.Item1, Button.Key.Item2)))
                                {
                                    Button.Value?.Invoke("", 0, 0f, true);
                                }

                                ButtonPosMultiplier++;
                                break;

                            case ButtonType.Toggle:
                                if (GUI.Toggle(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].BoolCache[ControlID], new GUIContent(Button.Key.Item1, Button.Key.Item2)) != DropDownState[DropData].BoolCache[ControlID])
                                {
                                    DropDownState[DropData].BoolCache[ControlID] = !DropDownState[DropData].BoolCache[ControlID];

                                    Button.Value?.Invoke("", 0, 0f, DropDownState[DropData].BoolCache[ControlID]);
                                }

                                ButtonPosMultiplier++;
                                break;

                            case ButtonType.Radial:
                                if (GUI.Toggle(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].BoolCache[ControlID], new GUIContent(Button.Key.Item1, Button.Key.Item2)) != DropDownState[DropData].BoolCache[ControlID])
                                {
                                    DropDownState[DropData].BoolCache[ControlID] = true;

                                    for (int i2 = 0; i2 < DropDownState[DropData].BoolCache.Count; i2++)
                                    {
                                        if (i2 != ControlID)
                                        {
                                            DropDownState[DropData].BoolCache[i2] = false;
                                        }
                                    }

                                    Button.Value?.Invoke("", 0, 0f, DropDownState[DropData].BoolCache[ControlID]);
                                }

                                ButtonPosMultiplier++;
                                break;

                            case ButtonType.Slider:
                                GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), new GUIContent(Button.Key.Item1, Button.Key.Item2));

                                ButtonPosMultiplier++;

                                float NewFloatValue = GUI.HorizontalSlider(new Rect(0, (25 * ButtonPosMultiplier) + 7, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].FloatCache[ControlID], 0, 255);

                                if (NewFloatValue != DropDownState[DropData].FloatCache[ControlID])
                                {
                                    DropDownState[DropData].FloatCache[ControlID] = NewFloatValue;

                                    Button.Value?.Invoke("", 0, DropDownState[DropData].FloatCache[ControlID], true);
                                }

                                GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), new GUIContent("", "Current Value: " + ((int)DropDownState[DropData].FloatCache[ControlID]).ToString()));

                                ButtonPosMultiplier++;
                                break;

                            case ButtonType.Label:
                                GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), new GUIContent(Button.Key.Item1, Button.Key.Item2));

                                ButtonPosMultiplier++;
                                break;

                            case ButtonType.TextArea:
                                string NewStringValue = GUI.TextArea(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].StringCache[ControlID]);

                                GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), new GUIContent("", Button.Key.Item2));

                                if (NewStringValue != DropDownState[DropData].StringCache[ControlID])
                                {
                                    DropDownState[DropData].StringCache[ControlID] = NewStringValue;

                                    Button.Value?.Invoke(DropDownState[DropData].StringCache[ControlID], 0, 0f, true);
                                }

                                ButtonPosMultiplier++;
                                break;
                        }
                    }

                    if (ShowSearch)
                    {
                        if (string.IsNullOrEmpty(DropDownState[DropData].SearchText) || DropDownState[DropData].SearchText == "Search")
                        {
                            MakeButton();
                        }
                        else if (Button.Key.Item1.ToLower().Contains(DropDownState[DropData].SearchText.ToLower()))
                        {
                            MakeButton();
                        }
                    }
                    else
                    {
                        MakeButton();
                    }

                    ControlID++;
                }

                GUI.EndScrollView();
            }

            if (!string.IsNullOrEmpty(GUI.tooltip))
            {
                Vector2 SizeOfText = GUI.tooltip.SizeOfText();

                GUI.Box(new Rect(Input.mousePosition.x, (Screen.height - Input.mousePosition.y) - 25, SizeOfText.x, 25), GUI.tooltip);
            }

            return DropData;
        }

        public static GUIStyle Style = new GUIStyle();

        /// <summary>
        /// Helper For Getting How Big Text Is
        /// </summary>
        /// <param name="text">The Text To Calculate The Size Of</param>
        /// <returns></returns>
        public static Vector2 SizeOfText(this string text)
        {
            Vector2 size = Style.CalcSize(new GUIContent(text));

            size.x += 10; //Padding

            if (size.y < size.x)
            {
                size.y = size.x;
            }

            return size;
        }
    }

    /// <summary>
    /// MetaData For The DropDown
    /// </summary>
    public class DropDownData
    {
        public string DropDownButtonText;
        public List<KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>>> ButtonsToShow;
    }

    /// <summary>
    /// Internal Data For The DropDown
    /// </summary>
    public class DropDownInternals
    {
        public bool IsOpen = false;
        public string SearchText = "Search";
        public Vector2 ScrollPosition = Vector2.zero;

        //Caching For ButtonType Variation Data Types
        public Dictionary<int, string> StringCache = new Dictionary<int, string>();
        public Dictionary<int, int> IntCache = new Dictionary<int, int>();
        public Dictionary<int, float> FloatCache = new Dictionary<int, float>();
        public Dictionary<int, bool> BoolCache = new Dictionary<int, bool>();
    }

    /// <summary>
    /// The Type Of Button Being Added To The DropDown
    /// </summary>
    public enum ButtonType
    {
        Button,
        Toggle,
        Radial,
        TextArea,
        Slider,
        Label
    }

    //To Do: Yes/No & Okay Dialog With GUI Class
}
