using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRules
{
    public static readonly int MIN_ROW_TO_DESTROY = 3;
    public static readonly int SCORE_PER_BALL = 1;
    public static readonly int ADDITIONAL_SCORE_PER_BALL = 1;

    public static readonly int BALL_DAMAGE = 1;

    public static readonly float MOVE_BACK_ON_PATH_SPEED_MULTIPLIER = 3f;

    public static readonly float MAX_DELTA_POS = 0.05f;
    public static readonly float MAX_RANGE_BTW_BALLS = 1f;
}
