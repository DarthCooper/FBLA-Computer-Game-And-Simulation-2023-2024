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

    public event Action onMushroomCollected;

    public void MushroomCollected()
    {
        if(onMushroomCollected != null)
        {
            onMushroomCollected();
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

    public event Action onPositionReached;

    public void NpcPositionReached()
    {
        if(onPositionReached != null)
        {
            onPositionReached();
        }
    }
}
