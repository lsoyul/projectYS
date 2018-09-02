using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEnum {
    
    public enum E_GAMESTATE
    {
        LOADING,
        TITLE,
        CUSTOMIZE,
        GAMEPLAY
    }

    public enum E_PLAYER_BEHAVIOUR_STATE
    {
        IDLE,
        WALK,
        RUN,
        ATTACK1,
        ATTACK2,
        ATTACK3
    }
    
    public enum E_OBJECT_TYPE
    {
        // WALL
        BREAKABLE_WALL,
        UNBREAKABLE_WALL,
        PASSASPOSSIBLE_WALL
    }
}
