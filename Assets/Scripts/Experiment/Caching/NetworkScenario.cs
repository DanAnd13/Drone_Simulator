using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NetworkScenario : MonoBehaviour
{
    public bool IsDisconnected { get; private set; }

    [Header("Scenario")]
    [SerializeField] private float _disconnectTime = 10f;
    [SerializeField] private float _reconnectDelay = 5f;

    [Header("Windows")]
    [SerializeField] private string _adapterName = "Ethernet";

    private Coroutine _scenarioRoutine;

    public void StartScenario()
    {
        if (_scenarioRoutine != null)
            StopCoroutine(_scenarioRoutine);

        _scenarioRoutine = StartCoroutine(RunScenario());
    }

    private IEnumerator RunScenario()
    {
        yield return new WaitForSeconds(_disconnectTime);
        IsDisconnected = true;
        Debug.Log("[NETWORK] Disable Ethernet");
        ExecuteNetsh($"interface set interface \"{_adapterName}\" admin=disable");

        yield return new WaitForSeconds(_reconnectDelay);

        Debug.Log("[NETWORK] Enable Ethernet");
        ExecuteNetsh($"interface set interface \"{_adapterName}\" admin=enable");
        IsDisconnected = false;
    }

    private void ExecuteNetsh(string arguments)
    {
        Process process = new Process();

        process.StartInfo.FileName = "netsh";
        process.StartInfo.Arguments = arguments;

        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Verb = "runas";

        process.Start();

        process.WaitForExit();

        Debug.Log(process.ExitCode);
    }
}