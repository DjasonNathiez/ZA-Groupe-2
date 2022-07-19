using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyptString : MonoBehaviour
{
    public string toEncrypt = "Test";
    private string key = "d0in2rf209c22jsd031";
    private string crypted = "";
    private string decrypted = "";

    string Encrypt(string data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            crypted = crypted + (char) (data[i] ^ key[i % key.Length]);
        }

        return crypted;
    }

    string Decrypted(string data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            decrypted = decrypted + (char) (data[i] ^ key[i % key.Length]);
        }

        return decrypted;
    }
}
