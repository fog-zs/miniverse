using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

/// <summary>
/// UI設定画面
/// </summary>
[RequireComponent(typeof(UI_AnimFade))]
public class UI_Settings : UI_SequentialMenu, GM_Msg
{
    [SerializeField] DB_Settings db;
    [SerializeField] Slider sliderCamera;
    [SerializeField] Slider sliderBgmVolume;
    [SerializeField] Slider sliderSeVolume;
    [SerializeField] Toggle screenMode;
    [SerializeField] Dropdown screenRes;
    UI_AnimFade ui;

    string path = "/setting.dat";

    void Awake()
    {
        ui = GetComponent<UI_AnimFade>();
    }

    private void Start()
    {
        // 設定情報読み込み
        path = Application.dataPath + path;
        ReadSettings();

        GM.Add("settings", this);
        for (int i = 0; i < fg.screenH.Length; i++)
        {
            screenRes.options.Add(new Dropdown.OptionData(name = $"{fg.screenW[i]} x {fg.screenH[i]}"));
        }

        // 設定をUIに適用
        SetUI();
    }

    /// <summary>
    /// UIに設定情報を反映
    /// </summary>
    private void SetUI()
    {
        sliderCamera.value = db.cameraSpeed;
        screenMode.isOn = db.fullScreen;
        screenRes.value = db.screenRes;
        sliderBgmVolume.value = db.bgmVolume;
        sliderSeVolume.value = db.seVolume;
        screenRes.RefreshShownValue();
    }

    /// <summary>
    /// 設定を読み込み
    /// </summary>
    void ReadSettings()
    {
        if (!File.Exists(path)) return;
        var text = File.ReadAllText(path);
        try
        {
            db = JsonUtility.FromJson<DB_Settings>(text);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 設定を書き込み
    /// </summary>
    void WriteSettings()
    {
        File.WriteAllText(path, JsonUtility.ToJson(db));
    }

    void GM_Msg.Receive(string data1, params object[] data2)
    {
        // UIの開閉
        ui.active = ui.active ? false: true;
        if (ui.active) Begin();
        else EndUI();        
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (!ui.active) return;
        if (InputF.GetButtonDown(eInputMap.data.Menu))
        {
            // 画面を閉じる
            GM.Msg("ui.menu", "close");
            End();
        }
    }

    /// <summary>
    /// UIを閉じる
    /// </summary>
    void EndUI()
    {
        GM.Msg("camera.settings");
        GM.Msg("ui.menu", "restart");
        WriteSettings();
        ui.active = false;
    }

    public override void Begin()
    {
        base.Begin();
        ui.active = true;
    }

    public override void ToMainMenu()
    {
        base.ToMainMenu();
        EndUI();
    }

    public override void End()
    {
        base.End();
        EndUI();
    }

    public void ScreenResChanged()
    {
        db.screenRes = screenRes.value;
    }

    public void ScreenModeChanged()
    {
        db.fullScreen = screenMode.isOn;
    }

    public void CameraSpeedChanged()
    {
        db.cameraSpeed = sliderCamera.value;
    }

    public void ChangedBGMVolume()
    {
        db.bgmVolume = sliderBgmVolume.value;
    }

    public void ChangedSEVolume()
    {
        db.seVolume = sliderSeVolume.value;
    }
}
