using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
/// 
public class PythonManager : MonoBehaviour
{
    public void RunQuery(Text text)
    {
        //blocks main thread
        var runnable = new QueryRunnable(text.text);
        runnable.Start();
    }
}


//TBD not used at the moment
public class QueryRunnable : RunnableThread
{
    internal QueryRunnable(string @in)
    {
        this.@in = @in;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");
            Debug.Log("Sending query for " + @in + "_csv");
            client.SendFrame(@in + "_csv");
            string message = "";
            bool gotMessage = false;
            while (Running)
            {
                gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                if (gotMessage) break;
            }

            if (gotMessage) Debug.Log(message);
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }

    private readonly string @in;
}