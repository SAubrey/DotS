 /*
 using UnityEngine;
     
     namespace DebugStuff
     {
         public class ConsoleToGUI : MonoBehaviour
         {
     //#if !UNITY_EDITOR
             static string myLog = "";
             private string output;
             private string stack;
     
             void OnEnable()
             {
                 Application.logMessageReceived += Log;
             }
     
             void OnDisable()
             {
                 Application.logMessageReceived -= Log;
             }
     
             public void Log(string logString, string stackTrace, LogType type)
             {
                 output = logString;
                 stack = stackTrace;
                 myLog = output + "\n" + myLog;
                 if (myLog.Length > 5000)
                 {
                     myLog = myLog.Substring(0, 4000);
                 }
             }
     
             void OnGUI()
             {
                 //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
                 {
                     myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
                 }
             }
     //#endif
         }
     }

*/
     using UnityEngine;
 
 public class ConsoleToGUI : MonoBehaviour
 {
     string myLog = "*begin log";
     string filename = "";
     bool doShow = true;
     int kChars = 700;
     void OnEnable() { Application.logMessageReceived += Log; }
     void OnDisable() { Application.logMessageReceived -= Log; }
     void Update() { if (Input.GetKeyDown(KeyCode.Space)) { doShow = !doShow; } }
     public void Log(string logString, string stackTrace, LogType type)
     {
        // for onscreen...
         myLog = myLog + "\n" + logString;
         if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }
 
         // for the file ...
         if (filename == "")
         {
             string d = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
             System.IO.Directory.CreateDirectory(d);
             string r = Random.Range(1000, 9999).ToString();
             filename = d + "/log-" + r + ".txt";
         }
         try { System.IO.File.AppendAllText(filename, logString + "\n"); }
         catch { }
     }
 
     void OnGUI()
     {
         if (!doShow) { return; }
         GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
            new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
         GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
     }
 }