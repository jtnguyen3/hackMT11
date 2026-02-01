namespace HackMT2026
{
    public record PitchRequest(
        int PitcherId,
        int GameId,
        string PitchType,
        int CoachCall,
        int ActualCall,
        string Outcome,
        int BatterId
    );
}