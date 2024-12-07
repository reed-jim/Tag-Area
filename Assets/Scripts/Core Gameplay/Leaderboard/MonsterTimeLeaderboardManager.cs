using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class MonsterTimeLeaderboardManager : MonoBehaviour
{
    private Dictionary<ulong, int> _playersMonsterTime;

    #region ACTION
    public static event Action<CharacterMonsterTimeData[]> updateMonsterTimeUIEvent;
    #endregion

    private void Awake()
    {
        CharacterMonsterTimeCounter.updateMonsterTimeEvent += UpdateMonsterTime;

        _playersMonsterTime = new Dictionary<ulong, int>();
    }

    private void OnDestroy()
    {
        CharacterMonsterTimeCounter.updateMonsterTimeEvent -= UpdateMonsterTime;
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

        CharacterMonsterTimeData[] characterMonsterTimesData =
            _playersMonsterTime
                .OrderBy(item => item.Value)
                .Select(item => new CharacterMonsterTimeData(item.Key, item.Value))
                .ToArray();

        updateMonsterTimeUIEvent?.Invoke(characterMonsterTimesData);
    }
}
