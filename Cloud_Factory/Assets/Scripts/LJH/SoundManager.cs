using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 각 씬별로 BGM은 인스펙터에서 할당
// 슬라이더의 값이 변경될 때 마다 볼륨 조절 및 저장
public class SoundManager : MonoBehaviour
{   
    private SeasonDateCalc mSeason; // 계절

    // BGM, 효과음 오디오 소스
    private AudioSource mBGM;
    private AudioSource mSFx; // 기본 클릭음
    private AudioSource mSubSFx; // 여분 클릭음

    public AudioClip[] mSeasonBGMArray = new AudioClip[4]; // 4계절별로 달라지는 BGM
    public AudioClip[] mBGMArray = new AudioClip[2]; // [0] 테마 [1] 응접실
    public enum SFx {
        SFx_CloudeDecorEnd,
        SFx_CloudMaking,
        SFx_CloudMonitor,
        SFx_HarvestDone,
        SFx_Hint,
        SFx_MonitorLoading,
        SFx_StickerPeel,
        SFx_StickerStick,
        SFx_TalkFemale,
        SFx_TalkMale,
        SFx_TalkNarr,
        SFx_UseIngredient,
        SFx_END,
    };
    public AudioClip[] mSubSFxArray = new AudioClip[(int)SFx.SFx_END];

    public bool[] isOneTime = new bool[4]; // 노래 한번만 틀기

    // To Use Audio Fade In Effect
    public bool         fadeIn = true;
    private float       timer = 0.0f;

    const int MAX_VALUE = 2;
    private bool[] bFirstPlay = new bool[MAX_VALUE];

    void Awake()
    {   
        // 오디오 소스 찾기
        mBGM = GameObject.Find("mBGM").GetComponent<AudioSource>();
        mSFx = GameObject.Find("mSFx").GetComponent<AudioSource>();
        mSubSFx = GameObject.Find("mSubSFx").GetComponent<AudioSource>();

        mSeason = GameObject.Find("Season Date Calc").GetComponent<SeasonDateCalc>();
    }
    // 유니티 생명주기 활용
    void Start()
    {
        if (SceneData.Instance) // null check
        {
            // 씬이 변경될 때 저장된 값으로 새로 업데이트
            mBGM.volume = SceneData.Instance.BGMValue;
            mSFx.volume = SceneData.Instance.SFxValue;
            mSubSFx.volume = SceneData.Instance.SFxValue; 
        }

        if ((SceneManager.GetActiveScene().name == "Lobby"))
        {
            mBGM.clip = mBGMArray[0];
            mBGM.Play();
        }
        else if (SceneManager.GetActiveScene().name == "Drawing Room" )
        {
            mBGM.clip = mBGMArray[1];
            mBGM.Play();
        }
        
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Space Of Weather")
        { // 날씨의 공간 사운드 변경 불값으로 중복 플레이 제한
            switch (mSeason.mSeason)
            {
                case 1:
                    UpdateAudio(0);
                    break;
                case 2:
                    UpdateAudio(1);
                    break;
                case 3:
                    UpdateAudio(2);
                    break;
                case 4:
                    UpdateAudio(3);
                    break;
                default:
                    break;
            }           
        }

        if(fadeIn == true)
        {
            AudioFadeIn(1.0f);
        }
    }

    void AudioFadeIn(float duration)
    {
        float bgmValue = SceneData.Instance.BGMValue;

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer = 0.0f;
            fadeIn = false;
            return;
        }
        mBGM.volume = bgmValue * timer / duration;

    }

    void UpdateAudio(int _iIndex)
    {
        if (!isOneTime[_iIndex])
        {
            mBGM.clip = mSeasonBGMArray[_iIndex];
            mBGM.Play();
            for (int i = 0; i < 4; i++)
            {
                if (_iIndex == i) continue;

                isOneTime[i] = false;
            }
            isOneTime[_iIndex] = true;
        }
    }

    // BGM 볼륨 조정
    public void SetBGMVolume(float volume)
    {
        mBGM.volume = volume * 0.5f;
        // 값을 변경할 때 마다 저장
        SceneData.Instance.BGMValue = volume;
        if (false == bFirstPlay[0])
        {
            bFirstPlay[0] = true;
        }
        else
        {
            mSFx.Play();
        }
        
    }

    // SFx 볼륨 조정
    public void SetSFxVolume(float volume)
    {
        mSFx.volume = volume;
        mSubSFx.volume = volume;
        // 값을 변경할 때 마다 저장
        SceneData.Instance.SFxValue = volume;
        if (false == bFirstPlay[1])
        {
            bFirstPlay[1] = true;
        }
        else
        {
            mSFx.Play();
        }
    }
}
