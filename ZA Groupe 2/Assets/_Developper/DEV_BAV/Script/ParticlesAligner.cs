using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAligner : MonoBehaviour {
 
    [SerializeField] private ParticleSystem ps;
 
    // Start is called before the first frame update
    void Start() {
        ps = GetComponent<ParticleSystem>();
    }
 
    // Update is called once per frame
    void Update() {
        var main = ps.main;
        main.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
