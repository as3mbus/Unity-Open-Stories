﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using as3mbus.Story;

public class PhaseController : MonoBehaviour
{
    public Transform kameraRoute, kamera, baloonPos, baloonsizer;
    public Text dName, dText;
    public GameObject dPanel;
    int currentLine = 0, currentChar = 0;
    public SpriteRenderer pageL, pageR;
    public float speed = 5f, routeRadius = 1f, typeDelay = 0.2f;
    public float duration;

    public float times;
    float timeCount;
    Phase activePhase;
    private StorySceneController ssControl;
    Vector3 originPosition;
    float originZoom;
    bool movingCamera;
    public bool pageLR = true;
    public float shake = 0.2f;

    public void startPhase(Phase fase)
    {
        this.activePhase = fase;
        // Debug.Log(fase.toJson());
        currentLine = 0;
        readLine(currentLine);
    }
    // Update is called once per frame
    void Start()
    {
        ssControl = FindObjectOfType<StorySceneController>();
    }

    void Update()
    {
        if (currentLine >= activePhase.messages.Count) return;
        if (Input.GetButtonDown("Fire1"))
        {
            times = duration;
            spriteFade(activePhase.fademode[currentLine]);
            camRoute();
            if (currentChar >= activePhase.messages[currentLine].Length)
            {
                if (currentLine < activePhase.messages.Count - 1)
                {
                    currentLine++;
                    readLine(currentLine);
                }
                else
                {
                    hidePhase();
                }
            }
            else
            {
                showLine(activePhase.messages[currentLine]);
            }
        }
        spriteFade(activePhase.fademode[currentLine]);
        textPerSec(typeDelay);
        if (activePhase.fademode[currentLine] != fadeMode.color)
            camRoute();
        shakeCamera(activePhase.shake[currentLine], shake);
        showBaloon();
    }
    public void showLine(string line)
    {
        dText.text = line;
        currentChar = line.Length;
    }
    public void showBaloon()
    {
        if (times >= duration&&!baloonPos.gameObject.activeSelf)
        {
            baloonPos.gameObject.SetActive(true);
            baloonPos.GetComponent<Animation>().Play();
        }
    }

    public void readLine(int line)
    {
        if (line >= activePhase.messages.Count) return;
        if (activePhase.fademode[line] != fadeMode.none)
        {
            pageLR = !pageLR;
            activePage().color = new Color(1, 1, 1, 0);
        }
        baloonsizer.transform.localScale = new Vector2(activePhase.baloonsize[line], activePhase.baloonsize[line]);
        originPosition = kameraRoute.position;
        originZoom = kamera.GetComponent<Camera>().orthographicSize;
        times = 0;
        duration = activePhase.duration[line];
        currentChar = 0;
        dPanel.SetActive(false);
        dName.text = activePhase.characters[line];
        dText.text = "";
        if (
            Mathf.Abs(activePhase.baloonpos[line].x)
             + Mathf.Abs(activePhase.baloonpos[line].y)
             != 0)
        {
            baloonPos.gameObject.SetActive(true);
            baloonPos.localPosition = activePhase.baloonpos[line];
            baloonPos.gameObject.SetActive(false);
        }
        else
            baloonPos.gameObject.SetActive(false);

    }
    public void textPerSec(float delay)
    {
        if (times < duration)
            return;
        dPanel.SetActive(activePhase.messages[currentLine] != "");
        if (currentChar >= activePhase.messages[currentLine].Length)
            return;

        timeCount += Time.deltaTime;
        if (timeCount > delay)
        {
            dText.text = dText.text + activePhase.messages[currentLine][currentChar];
            currentChar++;
            timeCount = 0;
        }
    }
    void spriteFade(fadeMode fadeM)
    {
        if (times < duration)
            times += Time.deltaTime;
        if (fadeM == fadeMode.color)
            colorFade(Color.white);
        else if (fadeM == fadeMode.transition)
            transitionFade();
    }
    void transitionFade()
    {
        inactivePage().color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), times / duration);
        activePage().color = Color.Lerp(Color.white, new Color(1, 1, 1, 1), times / duration);
    }
    void colorFade(Color color)
    {
        kamera.GetComponent<Camera>().backgroundColor = color;
        if (times < duration / 2)
        {
            inactivePage().color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), (times * 2) / duration);
        }
        else
            camPos();
        activePage().color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, (times * 2 - duration) / duration);
    }
    void shakeCamera(float frequency, float magnitude)
    {
        Vector2 shakeVector;
        float seed = Time.time * frequency;
        // print(Time.time + " * " + frequency + " = " + seed);
        // print("Perlin = " + Mathf.PerlinNoise(seed, 0f));
        shakeVector.x = Mathf.PerlinNoise(seed, 0f) - 0.5f;
        shakeVector.y = Mathf.PerlinNoise(0f, seed) - 0.5f;
        shakeVector = shakeVector * magnitude;
        kamera.localPosition = shakeVector;

    }
    public SpriteRenderer activePage()
    {
        return pageLR ? pageL : pageR;
    }
    public SpriteRenderer inactivePage()
    {
        return !pageLR ? pageL : pageR;
    }

    public void camRoute()
    {
        float distance = Vector3.Distance(activePhase.paths[currentLine], kameraRoute.position);
        float zoomDistance = Mathf.Abs(kamera.GetComponent<Camera>().orthographicSize - activePhase.zooms[currentLine]);
        if (distance != 0)
            kameraRoute.position = Vector3.MoveTowards(originPosition, activePhase.paths[currentLine], times / duration);
        if (zoomDistance != 0)
            kamera.GetComponent<Camera>().orthographicSize = Mathf.Lerp(originZoom, activePhase.zooms[currentLine], times / duration);

        // if (Input.GetButtonDown("Fire1") && currentLine < activePhase.paths.Count)
        // {
        //     currentLine++;
        // }
    }
    public void camPos()
    {
        kameraRoute.position = activePhase.paths[currentLine];
    }

    public void hidePhase()
    {
        pageLR = true;
        pageL.color = Color.white;
        pageR.color = Color.white;
        gameObject.SetActive(false);
        ssControl.nextPhase();
    }
}
