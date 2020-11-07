using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GUIAddons
{
    public static class PlagueGUI
    {
        private static Dictionary<DropDownData, DropDownInternals> DropDownState = new Dictionary<DropDownData, DropDownInternals>();

        public static DropDownData DropDown(Rect PositionAndScale, string MainButtonText, List<KeyValuePair<string, Action>> Buttons, bool ShowSearch = true)
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
                DropDownState[DropData].SearchText = GUI.TextArea(new Rect(PositionAndScale.x, PositionAndScale.y - 25, PositionAndScale.width, PositionAndScale.height), DropDownState[DropData].SearchText);
            }

            if (GUI.Button(PositionAndScale, (DropDownState[DropData].IsOpen ? @"/\ " : @"\/ ") + MainButtonText + (DropDownState[DropData].IsOpen ? @" /\" : @" \/")))
            {
                DropDownState[DropData].IsOpen = !DropDownState[DropData].IsOpen;
            }

            if (DropDownState.ContainsKey(DropData))
            {
                if (DropDownState[DropData].IsOpen)
                {
                    DropDownState[DropData].ScrollPosition = GUI.BeginScrollView(new Rect(PositionAndScale.x, PositionAndScale.y + 50, PositionAndScale.width + 25, 500), DropDownState[DropData].ScrollPosition, new Rect(0, 0, PositionAndScale.width + 25, (PositionAndScale.height * (Buttons.Count + 1)) + 25));

                    int ButtonPosMultiplier = 0;

                    foreach (KeyValuePair<string, Action> Button in Buttons)
                    {
                        if (ShowSearch)
                        {
                            if (string.IsNullOrEmpty(DropDownState[DropData].SearchText) || DropDownState[DropData].SearchText == "Search")
                            {
                                if (GUI.Button(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key))
                                {
                                    Button.Value?.Invoke();
                                }

                                ButtonPosMultiplier++;
                            }
                            else if (Button.Key.ToLower().Contains(DropDownState[DropData].SearchText.ToLower()))
                            {
                                if (GUI.Button(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key))
                                {
                                    Button.Value?.Invoke();
                                }

                                ButtonPosMultiplier++;
                            }
                        }
                        else
                        {
                            if (GUI.Button(new Rect(0, 25 * ButtonPosMultiplier, PositionAndScale.width, PositionAndScale.height), Button.Key))
                            {
                                Button.Value?.Invoke();
                            }

                            ButtonPosMultiplier++;
                        }
                    }

                    GUI.EndScrollView();
                }
            }

            return DropData;
        }

        class DropDownInternals
        {
            public bool IsOpen = false;
            public string SearchText = "Search";
            public Vector2 ScrollPosition = Vector2.zero;
        }
    }

    public class DropDownData
    {
        public string DropDownButtonText;
        public List<KeyValuePair<string, Action>> ButtonsToShow;
    }
}
