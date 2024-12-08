using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class MonsterTimeLeaderboardManager : MonoBehaviour
{
    private Dictionary<ulong, int> _playersMonsterTime;
    CharacterMonsterTimeData[] _sortedCharacterMonsterTimesData;

    #region ACTION
    public static event Action<CharacterMonsterTimeData[]> updateMonsterTimeUIEvent;
    public static event Action<CharacterMonsterTimeData> setGameResultEvent;
    #endregion

    private void Awake()
    {
        CharacterMonsterTimeCounter.updateMonsterTimeEvent += UpdateMonsterTime;
        GameTimeCounterUI.endGameEvent += OnGameEnded;

        _playersMonsterTime = new Dictionary<ulong, int>();
    }

    private void OnDestroy()
    {
        CharacterMonsterTimeCounter.updateMonsterTimeEvent -= UpdateMonsterTime;
        GameTimeCounterUI.endGameEvent -= OnGameEnded;
    }

    private void UpdateMonsterTime(ulong networkObjectId, int time)
    {
        if (!_playersMonsterTime.ContainsKey(networkObjectId))
        {
            _playersMonsterTime.Add(networkObjectId, time);
        }
        else
        {
            _playersMonsterTime[networkObjectId] = time;
        }

        _sortedCharacterMonsterTimesData =
            _playersMonsterTime
                .OrderBy(item => item.Value)
                .Select(item => new CharacterMonsterTimeData(item.Key, item.Value))
                .ToArray();

        updateMonsterTimeUIEvent?.Invoke(_sortedCharacterMonsterTimesData);
    }

    private void OnGameEnded()
    {
        setGameResultEvent?.Invoke(_sortedCharacterMonsterTimesData[0]);
    }
}
