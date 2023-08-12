using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.春;
    private int monthInSeason = 3;
    public bool gameClockPause;
    private float tikTime;
    //灯光时间差
    private float timeDifference;
    public TimeSpan GameTime => new TimeSpan(gameHour,gameMinute,gameSecond);

    protected override void Awake()
    {
        base.Awake();
        NewGameTime();
    }

    private void Start()
    {
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);
        EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        //切换灯光
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }



    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay,gameSeason);
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            gameHour++;
            EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
            
        }
    }

    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                            gameMonth = 1;

                        monthInSeason--;
                        if (monthInSeason == 0)
                        {
                            monthInSeason = 3;

                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }

                            gameSeason = (Season)seasonNumber;

                            if (gameYear > 9999)
                            {
                                gameYear = 2023;
                            }
                        }
                    }
                    EventHandler.CallGameDayEvent(gameDay,gameSeason);
                }
                EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
            }

            EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }
        //Debug.Log("Second: " + gameSecond + " Minute: " + gameMinute);
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 30;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2023;
        gameSeason = Season.春;
        
    }
    
    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }
    
    /// <summary>
    /// 返回lightshift同时计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }

        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            Debug.Log(timeDifference);
            return LightShift.Night;
        }

        return LightShift.Morning;
    }
}
