# PlagueUnityGUI
A DropDown Addon For The GUI That Is Used In Mods To Put Buttons On Screen. (Realtime)

# Example Usage
```csharp
//Example
var ListOfButtonsx = new List<KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>>>();/*Cache The List So You Can Append Numerous Things To It First, And Keep Your Code Clean*/

ListOfButtonsx.Add(new KeyValuePair<Tuple<string, string, ButtonType, bool>, Action<string, int, float, bool>>(new Tuple<string, string, ButtonType, bool>(/*Button Text*/"Button Text", /*ToolTip Text*/"ToolTip Text", /*Button Type*/ButtonType.Button, /*Default Toggle State*/false), /*Delegate To Execute On Button Select/Toggle*/delegate (string a, int b, float c, bool d)
{
    //Code
}));

PlagueGUI.PlagueGUI.DropDown(/*The Position And Scale Of The DropDown*/new Rect(1000, 25, 300, 25), /*The Main DropDown Expand Button Text*/"Main Button Text", ListOfButtonsx);
