﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FlashText : MonoBehaviour {

    public float fadeTime = 1;

    protected static FlashText instance;
    protected Queue<string> messageQueue;
    protected Queue<Color> colorQueue;
    protected float time = 0;
    protected Text text;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
        messageQueue = new Queue<string>();
        colorQueue = new Queue<Color>();
        text = GetComponent<Text>();
    }

    public static void Flash(string message, Color c)
    {
        if (instance.gameObject.activeSelf)
        {
            instance.messageQueue.Enqueue(message);
            instance.colorQueue.Enqueue(c);
        }
        else
        {
            instance.text.text = message;
            c.a = 0;
            instance.text.color = c;
            instance.time = 0;
            instance.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime;
        if (time > fadeTime*2)
        {
            if (messageQueue.Count == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                text.text = messageQueue.Dequeue();
                text.color = colorQueue.Dequeue();
                time = 0f;
            }
        }
        Color c = text.color;
        float f = (fadeTime - time) / fadeTime;
        c.a = 1f - f*f;
        text.color = c;
    }
}