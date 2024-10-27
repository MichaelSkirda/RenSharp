using Assets.Scripts.RenSharp.Core;
using RenSharp.Core;
using TMPro;
using UnityEngine;

public class UserBalance : MonoBehaviour
{
    private RenSharpCore RSCore;

    [SerializeField]
    private CommandProccessor CommandProccessor;

    [SerializeField]
    private TextMeshProUGUI TextField;

    private ReactiveRS<int> Balance;

    void Start()
    {
        RSCore = CommandProccessor.RenSharp;
        Balance = RSCore.CreateReactive<int>("balance", 100);
    }

    void Update()
    {
        TextField.text = Balance.ToString();
        Balance.Value++;
    }
}
