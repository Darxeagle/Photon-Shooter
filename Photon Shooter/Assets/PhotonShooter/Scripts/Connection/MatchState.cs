namespace PhotonShooter.Scripts.Connection
{
    public enum MatchState
    {
        Disconnected,
        WaitingForRoom,
        WaitingForEnemy,
        CancellingWait,
        CancelledWait,
        Running,
        LeavingMatch,
        LeftMatch,
        EnemyLeavingMatch,
        EnemyLeftMatch,
        MatchLeavingWinPlayer,
        MatchLeavingWinEnemy,
        MatchOverWinPlayer,
        MatchOverWinEnemy
    }

    public static class MatchStateExtensions
    {
        public static bool IsPlayingState(this MatchState matchState)
        {
            return matchState == MatchState.Running;
        }
        
        public static bool IsLeftMatchState(this MatchState matchState)
        {
            return
                matchState == MatchState.LeftMatch ||
                matchState == MatchState.EnemyLeftMatch ||
                matchState == MatchState.MatchOverWinPlayer ||
                matchState == MatchState.MatchOverWinEnemy;
        }
        
        public static bool IsCanCreateMatchState(this MatchState matchState)
        {
            return
                matchState == MatchState.Disconnected ||
                matchState == MatchState.CancelledWait ||
                matchState.IsLeftMatchState();
        }
    }
}