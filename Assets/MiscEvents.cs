using System;

public class MiscEvents
{
    public event Action onArrowCollected;

    public void ArrowCollected()
    {
        if(onArrowCollected != null)
        {
            onArrowCollected();
        }
    }

    public event Action onWaveEnemyKilled;

    public void WaveEnemyKilled()
    {
        if(onWaveEnemyKilled != null)
        {
            onWaveEnemyKilled();
        }
    }
}
