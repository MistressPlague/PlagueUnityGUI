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
        private static Dictionary<DropDownData, DropDownInternals> DropDownState = new Dictionary<DropDownData, DropDownInternals>();

        /// <summary>
        /// Creates A GUI.DropDown Menu - Created By Plague
        /// </summary>
        /// <param name="PositionAndScale">The Position And Scale Of The DropDown Menu</param>
        /// <param name="MainButtonText">The Main Button For The DropDown's Text</param>
        /// <param name="Buttons">Your Buttons/Toggles For The DropDown, A List Of KeyValuePairs With Key Being A Tuple Of The Button Text, The ButtonType And Default Toggle State (If Toggle ButtonType) And Value Being A Delegate To Execute On Selection</param>
        /// <param name="ShowSearch">Whether To Show The Search Bar</param>
        /// <returns>The DropDownData Instance</returns>
        public static DropDownData DropDown(Rect PositionAndScale, string MainButtonText, List<KeyValuePair<Tuple<string, ButtonType, bool>, Action<bool>>> Buttons, bool ShowSearch = true)
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

                    foreach (KeyValuePair<Tuple<string, ButtonType, bool>, Action<bool>> Button in Buttons)
                    {
                        if (!DropDownState[DropData].ToggleStates.ContainsKey(Button.Key.Item1))
                        {
                            DropDownState[DropData].ToggleStates[Button.Key.Item1] = Button.Key.Item3;
                        }

                        void MakeButton()
                        {
                            if (Button.Key.Item2 == ButtonType.Button)
                            {
                                if (GUI.Button(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key.Item1))
                                {
                                    Button.Value?.Invoke(true);
                                }
                            }
                            else
                            {
                                if (GUI.Toggle(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].ToggleStates[Button.Key.Item1], Button.Key.Item1) != DropDownState[DropData].ToggleStates[Button.Key.Item1])
                                {
                                    DropDownState[DropData].ToggleStates[Button.Key.Item1] = !DropDownState[DropData].ToggleStates[Button.Key.Item1];

                                    Button.Value?.Invoke(DropDownState[DropData].ToggleStates[Button.Key.Item1]);
                                }
                            }
                        }

                        if (ShowSearch)
                        {
                            if (string.IsNullOrEmpty(DropDownState[DropData].SearchText) || DropDownState[DropData].SearchText == "Search")
                            {
                                MakeButton();

                                ButtonPosMultiplier++;
                            }
                            else if (Button.Key.Item1.ToLower().Contains(DropDownState[DropData].SearchText.ToLower()))
                            {
                                MakeButton();

                                ButtonPosMultiplier++;
                            }
                        }
                        else
                        {
                            MakeButton();

                            ButtonPosMultiplier++;
                        }
                    }

                    GUI.EndScrollView();
                }
            }

            return DropData;
        }

        /// <summary>
        /// Internal Data For The DropDown
        /// </summary>
        class DropDownInternals
        {
            public bool IsOpen = false;
            public string SearchText = "Search";
            public Vector2 ScrollPosition = Vector2.zero;
            public Dictionary<string, bool> ToggleStates = new Dictionary<string, bool>();
        }
    }

    /// <summary>
    /// MetaData For The DropDown
    /// </summary>
    public class DropDownData
    {
        public string DropDownButtonText;
        public List<KeyValuePair<Tuple<string, ButtonType, bool>, Action<bool>>> ButtonsToShow;
    }

    /// <summary>
    /// The Type Of Button Being Added To The DropDown
    /// </summary>
    public enum ButtonType
    {
        Button,
        Toggle
    }
}
