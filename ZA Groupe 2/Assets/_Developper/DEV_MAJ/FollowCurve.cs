using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FollowCurve : MonoBehaviour
{
    public float speed;
    public WayPoints[] points;
    public int currentPoint;
    public Quaternion currentRotation;
    public float step;
    public bool loop;
    public bool moving;
    public CameraController cam;
    public Vector3 pos;
    public bool control;
    public bool check;
    public GameObject canon;
    public float timer;
    public float delay;
    public GameObject bullet;
    public Transform canonTir;
    public Transform visee;
    public int unpackNode = 606;
    public bool reUsable;
    public float bulletSpeed = 10;
    public BossRollerCoaster BossRollerCoaster;

    private void FixedUpdate()
    {
        if (moving)
        {
            if (control)
            {
                float angle = Vector2.SignedAngle(new Vector2(PlayerManager.instance.move.x,PlayerManager.instance.move.z), new Vector2(transform.forward.x, transform.forward.z).normalized);
                angle += PlayerManager.instance.cameraController.transform.eulerAngles.y;
                canon.transform.localEulerAngles = new Vector3(-90,0,angle);

                if (PlayerManager.instance.buttonAPressed && !check && timer <= 0)
                {
                    Shoot();
                    timer = delay;
                }

                if (!PlayerManager.instance.buttonAPressed && check) check = false;

                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }



            }
            step += speed;

            while (step >= 1)
            {
                step = step - 1;
                if (currentPoint < points.Length-2)
                {
                    currentRotation = Quaternion.LookRotation(
                        (points[currentPoint + 1].point - points[currentPoint].point).normalized,
                        points[currentPoint + 1].up);
                    currentPoint++;
                }
                else if (loop)
                {
                    currentRotation = Quaternion.LookRotation(
                        (points[currentPoint + 1].point - points[currentPoint].point).normalized,
                        points[currentPoint + 1].up);
                    currentPoint = 0;
                }
            }

            if (currentPoint < points.Length - 2)
            {
                transform.position = transform.position = Vector3.Lerp(points[currentPoint].point, points[currentPoint + 1].point, step);
                transform.rotation = Quaternion.Lerp(currentRotation,
                    Quaternion.LookRotation((points[currentPoint + 1].point - points[currentPoint].point).normalized,points[currentPoint + 1].up),
                    step);
                speed = Mathf.Lerp(speed, points[currentPoint + 1].speed, 5 * Time.deltaTime);
                cam.cameraPos.position = transform.position + Vector3.Lerp(points[currentPoint].camPos, points[currentPoint + 1].camPos, step);
                cam.cameraPos.rotation = Quaternion.Lerp(points[currentPoint].camAngle, points[currentPoint + 1].camAngle, step);
                cam.cameraZoom = Mathf.Lerp(points[currentPoint].camZoom, points[currentPoint + 1].camZoom, step);
            }
            else if (loop)
            {
                transform.position = Vector3.Lerp(points[currentPoint].point, points[0].point, step);
                transform.rotation = Quaternion.Lerp(currentRotation,
                    Quaternion.LookRotation((points[0].point - points[currentPoint].point).normalized,points[0].up),
                    step);
                speed = Mathf.Lerp(speed, points[0].speed, 5 * Time.deltaTime);
                cam.cameraPos.position = transform.position + Vector3.Lerp(points[currentPoint].camPos, points[0].camPos, step);
                cam.cameraPos.rotation = Quaternion.Lerp(points[currentPoint].camAngle, points[0].camAngle, step);
                cam.cameraZoom = Mathf.Lerp(points[currentPoint].camZoom, points[0].camZoom, step);
            }

            if (currentPoint > unpackNode)
            {
                moving = false;
                control = false;
                cam.playerFocused = true;
                PlayerManager.instance.ExitDialogue();
                PlayerManager.instance.transform.position = pos;
                GameManager.instance.CheckScene();
                if (reUsable) currentPoint = 0;
            }
        }
    }

    void Shoot()
    {
        AudioManager.instance.PlayEnvironment("FireworkShoot");
        GameObject newbullet = Instantiate(bullet,canonTir.position,quaternion.identity);
        newbullet.GetComponent<Rigidbody>().AddForce((canonTir.position - visee.position).normalized * bulletSpeed,ForceMode.Impulse);
    }

    private void Start()
    {
        cam = PlayerManager.instance.cameraController;
    }
}
