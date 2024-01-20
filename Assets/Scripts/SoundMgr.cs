using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMgr : MonoBehaviour
{

    public static SoundMgr g_inst = null;


    public AudioSource UISound_Audio;
    public AudioSource Background_Audio;
    
    public AudioClip Touch_Sound;
    public AudioClip Button1_Sound;
    public AudioClip Button2_Sound;
    public AudioClip OpenWindow_Sound;
    public AudioClip CloseWindow_Sound;
    public AudioClip InfoWindow_Sound;
    public AudioClip SceneChange_Sound;

    public AudioClip Install_Sound;
    public AudioClip PageFlip_1_Sound;
    public AudioClip PageFlip_2_Sound;
    public AudioClip PageFlip_3_Sound;
    public AudioClip Select_Sound;

    public AudioClip Reload_Sound;
    public AudioClip Error_Sound;
    public AudioClip LevelUp_Sound;
    public AudioClip LostHeart_Sound;
    public AudioClip Clear_Sound;
    public AudioClip GameOver_Sound;
    public AudioClip Alarm_Sound;


    public AudioClip BGMTitle_Sound;
    public AudioClip BGMGoldBeach_Sound;
    public AudioClip BGMBlueWorld_Sound;
    public AudioClip BGMNautilus_Sound;
    public AudioClip BGMAquarium_Sound;
    public AudioClip[] BGMPlayList_Sound;



    public AudioClip CrabAttack_Sound;
    public AudioClip ElectricAttack_Sound;
    public AudioClip PufferAttack_Sound;

    public AudioClip ClamHit_Sound;

    public AudioClip MonsterHit_Sound;
    public AudioClip MonsterDie_Sound;

    public AudioSource[] EffectArray_Audio;




    public AudioMixer MasterMixer;

    public bool ListPlay_flag = false;

    public bool SoundOnOff = true;

    void ResourceLoad()
    {
        Touch_Sound = Resources.Load<AudioClip>("Sound/UI/Touch");
        Button1_Sound = Resources.Load<AudioClip>("Sound/UI/Button_1");
        Button2_Sound = Resources.Load<AudioClip>("Sound/UI/Button_2");
        OpenWindow_Sound = Resources.Load<AudioClip>("Sound/UI/OpenWindow");
        CloseWindow_Sound = Resources.Load<AudioClip>("Sound/UI/CloseWindow");
        InfoWindow_Sound = Resources.Load<AudioClip>("Sound/UI/InfoWindow");
        SceneChange_Sound = Resources.Load<AudioClip>("Sound/UI/SceneChange");

        Install_Sound = Resources.Load<AudioClip>("Sound/Install");
        PageFlip_1_Sound = Resources.Load<AudioClip>("Sound/UI/PageFlip_1");
        PageFlip_2_Sound = Resources.Load<AudioClip>("Sound/UI/PageFlip_2");
        PageFlip_3_Sound = Resources.Load<AudioClip>("Sound/UI/PageFlip_3");
        Select_Sound = Resources.Load<AudioClip>("Sound/UI/Select");

        Reload_Sound = Resources.Load<AudioClip>("Sound/ReLoad");
        Error_Sound = Resources.Load<AudioClip>("Sound/Error");
        LevelUp_Sound = Resources.Load<AudioClip>("Sound/Upgrade");
        LostHeart_Sound = Resources.Load<AudioClip>("Sound/LostHeart");
        Clear_Sound = Resources.Load<AudioClip>("Sound/Clear");
        GameOver_Sound = Resources.Load<AudioClip>("Sound/GameOver");
        Alarm_Sound = Resources.Load<AudioClip>("Sound/Alarm");

        BGMTitle_Sound = Resources.Load<AudioClip>("Sound/BackGround/Title");
        BGMGoldBeach_Sound = Resources.Load<AudioClip>("Sound/BackGround/GoldBeach");
        BGMBlueWorld_Sound = Resources.Load<AudioClip>("Sound/BackGround/BlueWorld");
        BGMNautilus_Sound = Resources.Load<AudioClip>("Sound/BackGround/Nautilus");
        BGMAquarium_Sound = Resources.Load<AudioClip>("Sound/BackGround/Aquarium");

        BGMPlayList_Sound = new AudioClip[3];
        BGMPlayList_Sound[0] = BGMBlueWorld_Sound;
        BGMPlayList_Sound[1] = BGMNautilus_Sound;
        BGMPlayList_Sound[2] = BGMAquarium_Sound;



        CrabAttack_Sound = Resources.Load<AudioClip>("Sound/Effect/Crab_Attack");
        ElectricAttack_Sound = Resources.Load<AudioClip>("Sound/Effect/Electric_Attack");
        PufferAttack_Sound = Resources.Load<AudioClip>("Sound/Effect/Puffer_Attack");

        ClamHit_Sound = Resources.Load<AudioClip>("Sound/Effect/Clam_Attack");


        MonsterHit_Sound = Resources.Load<AudioClip>("Sound/Effect/Hit");
        MonsterDie_Sound = Resources.Load<AudioClip>("Sound/Effect/Monster_Die");

        


        MasterMixer = Resources.Load<AudioMixer>("Sound/MasterMixer");

    }

    void ObjectLoad()
    {
        UISound_Audio = GameObject.Find("UISound").transform.GetComponent<AudioSource>();
        Background_Audio = GameObject.Find("BackGroundMusic").transform.GetComponent<AudioSource>();

        EffectArray_Audio = new AudioSource[10];
        EffectArray_Audio = GameObject.Find("EffectSoundGroup").transform.GetComponentsInChildren<AudioSource>();
    }

    private void Awake()
    {
        ResourceLoad();
        ObjectLoad();

        g_inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
        


    }

    // Update is called once per frame
    void Update()
    {

        if (ListPlay_flag == true && !Background_Audio.isPlaying)
            RandomPlay();

    }


    public void RandomPlay()
    {
        if(Background_Audio.loop == true)
            Background_Audio.loop = false;

        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks));

        Background_Audio.clip = BGMPlayList_Sound[UnityEngine.Random.Range(0, BGMPlayList_Sound.Length)];
        Background_Audio.Play();
        ListPlay_flag = true;
    }

    public void RandomStop()
    {
        Background_Audio.loop = true;
        ListPlay_flag = false;

    }


    public void EffectPlay(AudioClip _clip)
    {
        for(int i = 0; i < EffectArray_Audio.Length; i++)
        {
            if(EffectArray_Audio[i].isPlaying == false || EffectArray_Audio[i].clip == null)
            {
                EffectArray_Audio[i].PlayOneShot(_clip, 1.0f);
                break;
            }
        }
    }

}
