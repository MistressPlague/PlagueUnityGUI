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
        /// Creates A GUI.DropDown Menu - Created By Plague
        /// </summary>
        /// <param name="PositionAndScale">The Position And Scale Of The DropDown Menu</param>
        /// <param name="MainButtonText">The Main Button For The DropDown's Text</param>
        /// <param name="Buttons">Your Buttons/Toggles For The DropDown, A List Of KeyValuePairs With Key Being A Tuple Of The Button Text, The ButtonType And Default Toggle State (If Toggle ButtonType) And Value Being A Delegate To Execute On Selection</param>
        /// <param name="ShowSearch">Whether To Show The Search Bar</param>
        /// <returns>The DropDownData Instance</returns>
        public static DropDownData DropDown(Rect PositionAndScale, string MainButtonText, List<KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>>> Buttons, bool ShowSearch = true)
        {
            if (string.IsNullOrEmpty(MainButtonText) || Buttons == null || Buttons.Count == 0)
            {
                return null;
            }

            Buttons = Buttons.OrderBy(a => a.Key).ToList();

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
            }

            if (GUI.Button(PositionAndScale, (DropDownState[DropData].IsOpen ? @"/\ " : @"\/ ") + MainButtonText + (DropDownState[DropData].IsOpen ? @" /\" : @" \/")))
            {
                DropDownState[DropData].IsOpen = !DropDownState[DropData].IsOpen;
            }

            if (DropDownState.ContainsKey(DropData))
            {
                if (DropDownState[DropData].IsOpen)
                {
                    DropDownState[DropData].ScrollPosition = GUI.BeginScrollView(new Rect(PositionAndScale.x, PositionAndScale.y + 50, PositionAndScale.width + 25, 500), DropDownState[DropData].ScrollPosition, new Rect(0, 0, PositionAndScale.width + 25, (PositionAndScale.height * (string.IsNullOrEmpty(DropDownState[DropData].SearchText) || DropDownState[DropData].SearchText == "Search" ? Buttons.Count : Buttons.Where(b => b.Key.Item1.ToLower().Contains(DropDownState[DropData].SearchText.ToLower())).ToList().Count))));

                    int ButtonPosMultiplier = 0;

                    foreach (KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>> Button in Buttons)
                    {
                        if (!DropDownState[DropData].StringCache.ContainsKey(Button.Value))
                        {
                            DropDownState[DropData].StringCache[Button.Value] = Button.Key.Item1;
                        }

                        if (!DropDownState[DropData].IntCache.ContainsKey(Button.Value))
                        {
                            DropDownState[DropData].IntCache[Button.Value] = 0;
                        }

                        if (!DropDownState[DropData].FloatCache.ContainsKey(Button.Value))
                        {
                            DropDownState[DropData].FloatCache[Button.Value] = 0f;
                        }

                        if (!DropDownState[DropData].BoolCache.ContainsKey(Button.Value))
                        {
                            DropDownState[DropData].BoolCache[Button.Value] = Button.Key.Item4;
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
                                    if (GUI.Toggle(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].BoolCache[Button.Value], new GUIContent(Button.Key.Item1, Button.Key.Item2)) != DropDownState[DropData].BoolCache[Button.Value])
                                    {
                                        DropDownState[DropData].BoolCache[Button.Value] = !DropDownState[DropData].BoolCache[Button.Value];

                                        Button.Value?.Invoke("", 0, 0f, DropDownState[DropData].BoolCache[Button.Value]);
                                    }

                                    ButtonPosMultiplier++;
                                    break;

                                case ButtonType.Slider:
                                    GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key.Item1);

                                    ButtonPosMultiplier++;

                                    float NewFloatValue = GUI.HorizontalSlider(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width - 25, PositionAndScale.height), DropDownState[DropData].FloatCache[Button.Value], 0, 255);

                                    if (NewFloatValue != DropDownState[DropData].FloatCache[Button.Value])
                                    {
                                        DropDownState[DropData].FloatCache[Button.Value] = NewFloatValue;

                                        Button.Value?.Invoke("", 0, DropDownState[DropData].FloatCache[Button.Value], true);
                                    }

                                    GUI.Label(new Rect(PositionAndScale.width - 12, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].FloatCache[Button.Value].ToString());

                                    ButtonPosMultiplier++;
                                    break;

                                case ButtonType.Label:
                                    GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key.Item1);

                                    ButtonPosMultiplier++;
                                    break;

                                case ButtonType.TextArea:
                                    GUI.Label(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key.Item2);

                                    ButtonPosMultiplier++;

                                    string NewStringValue = GUI.TextArea(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].StringCache[Button.Value]);

                                    if (NewStringValue != DropDownState[DropData].StringCache[Button.Value])
                                    {
                                        DropDownState[DropData].StringCache[Button.Value] = NewStringValue;

                                        Button.Value?.Invoke(DropDownState[DropData].StringCache[Button.Value], 0, 0f, true);
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
                    }

                    GUI.EndScrollView();
                }
            }

            return DropData;
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
        public Dictionary<Action<string, int, float, bool>, string> StringCache = new Dictionary<Action<string, int, float, bool>, string>();
        public Dictionary<Action<string, int, float, bool>, int> IntCache = new Dictionary<Action<string, int, float, bool>, int>();
        public Dictionary<Action<string, int, float, bool>, float> FloatCache = new Dictionary<Action<string, int, float, bool>, float>();
        public Dictionary<Action<string, int, float, bool>, bool> BoolCache = new Dictionary<Action<string, int, float, bool>, bool>();
    }

    /// <summary>
    /// The Type Of Button Being Added To The DropDown
    /// </summary>
    public enum ButtonType
    {
        Button,
        Toggle,
        TextArea,
        Slider,
        Label
    }
}
